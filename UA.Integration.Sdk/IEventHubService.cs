namespace UA.Integration.SDK
{
    // Define the public interface
    public interface IEventHubService
    {
        event EventHandler<WaveformDataAvailableEvent> NewMessage;

        Task SendFeatureValueMessageAsync(FeatureValueMessage message);
        void Start();
        void Stop();
    }
}
