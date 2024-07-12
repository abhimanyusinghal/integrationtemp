using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Producer;
using System.Text.Json;
using Serilog.Context;

namespace UA.Integration.SDK
{
    // Internal implementation of the Event Hub consumer
    internal class EventHubService : IEventHubService
    {
        private readonly EventProcessorClient _processorClient;
        private readonly EventHubProducerClient _producerClient;
        private readonly ILogger<EventHubService> _logger;

        public event EventHandler<WaveformDataAvailableEvent> NewMessage;

        public EventHubService(string blobStorageConnectionString, string blobContainerName, string consumerConnectionString, string consumerEventHubName, string consumerGroup, string producerConnectionString, string producerEventHubName, ILogger<EventHubService> logger)
        {
            var blobContainerClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);
            _processorClient = new EventProcessorClient(blobContainerClient, consumerGroup, consumerConnectionString, consumerEventHubName);
            _processorClient.ProcessEventAsync += ProcessEventHandler;
            _processorClient.ProcessErrorAsync += ProcessErrorHandler;

            _producerClient = new EventHubProducerClient(producerConnectionString, producerEventHubName);
            _logger = logger;
        }

        public async void Start()
        {
            try
            {
                await _processorClient.StartProcessingAsync();
                _logger.LogInformation("Event Hub processing started.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Event Hub processing.");
                throw; // Re-throw to ensure the failure is not suppressed
            }
        }

        public async void Stop()
        {
            try
            {
                await _processorClient.StopProcessingAsync();
                _logger.LogInformation("Event Hub processing stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop Event Hub processing.");
                throw; // Re-throw to ensure the failure is not suppressed
            }
        }

        private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            try
            {
                var eventData = eventArgs.Data;
                var rawData = System.Text.Json.JsonSerializer.Deserialize<Rootobject>(eventData.Body.ToArray());

                // Use LogContext to enrich all logs made during this request
                using (LogContext.PushProperty("MessageId", rawData.unifiedMessageId))
                using (LogContext.PushProperty("tenantId", rawData.tenantId))
                using (LogContext.PushProperty("messageTimestamp",rawData.timestamp))
                using (LogContext.PushProperty("sensorSerialNumber", rawData.sensorId))
                {
                    var traceParent = rawData.traceParentId ?? rawData.traceId;
                    if (!string.IsNullOrEmpty(traceParent))
                    {
                        using (var activity = new Activity("ProcessEvent"))
                        {
                            activity.SetParentId(traceParent);
                            activity.Start();

                            try
                            {
                                var waveFormDataAvailableEvent = new WaveformDataAvailableEvent()
                                {
                                    // properties setup
                                };

                                // Raise the NewMessage event
                                NewMessage?.Invoke(this, waveFormDataAvailableEvent);

                                // Update the checkpoint
                                await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
                                _logger.LogInformation("Processed and checkpointed message");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing message.");
                                throw;
                            }
                            finally
                            {
                                activity.Stop();
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Missing traceParentId in message");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing message.");
                throw;
            }
        }


        private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Handle any errors that occur during the processing of events
            _logger.LogError(eventArgs.Exception, "Error in Event Hub processing.");
            return Task.CompletedTask;
        }



        public async Task SendFeatureValueMessageAsync(FeatureValueMessage message)
        {
            var eventData = new EventData(JsonSerializer.SerializeToUtf8Bytes(message));

            using (LogContext.PushProperty("sensorSerialNumber", message.SerialNumber))
            using (LogContext.PushProperty("messageTimestamp", message.Timestamp))
            using (var eventBatch = await _producerClient.CreateBatchAsync())
            {
                if (!eventBatch.TryAdd(eventData))
                {
                    _logger.LogError("Failed to add message to the batch.");
                    throw new Exception("Failed to add message to the batch.");
                }

                var activitySource = new ActivitySource("UA.Integration.SDK.EventHubService");
                using (var activity = activitySource.StartActivity("SendFeatureValueMessage", ActivityKind.Producer))
                {
                    activity?.SetTag("message.sensorSerialNumber", message.SerialNumber);
                    activity?.SetTag("message.timestamp", message.Timestamp);
                    activity?.SetTag("eventhub.name", _producerClient.EventHubName);

                    if (activity != null)
                    {
                        eventData.Properties["traceparent"] = activity.Id;
                        eventData.Properties["tracestate"] = activity.TraceStateString;
                    }

                    bool sent = false;
                    int retryCount = 0;
                    while (!sent && retryCount < 5)
                    {
                        try
                        {
                            await _producerClient.SendAsync(eventBatch);
                            _logger.LogInformation("Message sent successfully to producer Event Hub.");
                            sent = true;

                            // Record the successful send as a dependency
                            if (activity != null)
                            {
                                activity.SetStatus(ActivityStatusCode.Ok);
                                activity.AddTag("status", "success");
                            }
                        }
                        catch (Exception ex)
                        {
                            retryCount++;
                            _logger.LogError(ex, "Failed to send message. Retrying... Attempt {RetryCount}", retryCount);
                            await Task.Delay(TimeSpan.FromSeconds(2));

                            // Record the failure in the activity
                            if (activity != null)
                            {
                                activity.SetStatus(ActivityStatusCode.Error, ex.Message);
                                activity.AddTag("status", "failure");
                            }
                        }
                    }

                    if (!sent)
                    {
                        _logger.LogError("Failed to send message after multiple attempts.");
                        throw new Exception("Failed to send message after multiple attempts.");
                    }

                    activity?.Stop();
                }
            }
        }

    }
}
