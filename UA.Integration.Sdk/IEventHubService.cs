namespace UA.Integration.SDK
{
    // Define the public interface
    public interface IEventHubService
    {
        event EventHandler<WaveformDataAvailableEvent> NewMessage;
        void Start();
        void Stop();
    }
}
