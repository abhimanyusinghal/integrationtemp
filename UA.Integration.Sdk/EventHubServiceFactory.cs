using UA.Integration.SDK;

public static class EventHubServiceFactory
{
    public static IEventHubService Create(string blobStorageConnectionString, string blobContainerName, string eventHubsConnectionString, string eventHubName, string consumerGroup)
    {
#if MOCK
        return new MockEventHubService(blobStorageConnectionString, blobContainerName, eventHubsConnectionString, eventHubName, consumerGroup);
#else
        return new EventHubService(blobStorageConnectionString, blobContainerName, eventHubsConnectionString, eventHubName, consumerGroup);
#endif
    }
}