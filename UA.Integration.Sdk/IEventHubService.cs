namespace UA.Integration.SDK
{
    // Define the public interface
    public interface IEventHubService
    {
        event EventHandler<RawDataAvailableEvent> NewMessage;
        void Start();
        void Stop();
    }
}
