using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;
using System.Timers;

namespace UA.Integration.SDK
{
    // Internal implementation of the Event Hub consumer
    internal class MockEventHubService : IEventHubService
    {

        public event EventHandler<WaveformDataAvailableEvent> NewMessage;

        private readonly System.Timers.Timer _timer;

        public MockEventHubService(string blobStorageConnectionString, string blobContainerName, string eventHubsConnectionString, string eventHubName, string consumerGroup)
        {
         
            _timer = new System.Timers.Timer(10000); // Set the interval to 10 seconds (10000 milliseconds
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            // Create and send a mock sensor data message
            var rawDataAvailableEvent = new WaveformDataAvailableEvent
            {
                SensorSerialNumber = "6124017100203D",
                Timestamp = 1706210100,
                SensorType = "S7100",
                MeasurementType = MeasurementType.LowFrequencyWaveform,
                GatewaySerialNumber = "022401UGW4201E",
                SensorScope = "/"
            };

            // Raise the event
            NewMessage?.Invoke(this, rawDataAvailableEvent);
        }
        public async void Start()
        {
            _timer.Start();

        }

        public async void Stop()
        {
            _timer.Stop();
        }
    }
}
