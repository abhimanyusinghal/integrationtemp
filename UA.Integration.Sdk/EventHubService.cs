using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;

namespace UA.Integration.SDK
{
    // Internal implementation of the Event Hub consumer
    internal class EventHubService : IEventHubService
    {
        private EventProcessorClient _processorClient;

        public event EventHandler<WaveformDataAvailableEvent> NewMessage;

        public EventHubService(string blobStorageConnectionString, string blobContainerName, string eventHubsConnectionString, string eventHubName, string consumerGroup)
        {
            var blobContainerClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);
            _processorClient = new EventProcessorClient(blobContainerClient, consumerGroup, eventHubsConnectionString, eventHubName);
            _processorClient.ProcessEventAsync += ProcessEventHandler;
            _processorClient.ProcessErrorAsync += ProcessErrorHandler;
        }

        public async void Start()
        {
            await _processorClient.StartProcessingAsync();
        }
        
        public async void Stop()
        {
            await _processorClient.StopProcessingAsync();
        }

        private Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            var eventData = eventArgs.Data;
            var rawData = System.Text.Json.JsonSerializer.Deserialize<WaveformDataAvailableEvent>(eventData.Body.ToString());

            // Raise the NewMessage event
            NewMessage?.Invoke(this, rawData);
            return Task.CompletedTask;
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Handle any errors that occur during the processing of events
            Console.WriteLine($"Error: {eventArgs.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}