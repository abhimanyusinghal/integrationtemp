using UA.Integration.SDK;

public static class SensorDataClientFactory
{
    public static ISensorDataClient Create(string apiBaseUrl,string eventHubConnectionString, string eventHubName)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl)
        };

        // Optionally configure other HTTP client settings here, like request headers, timeouts, etc.
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

#if MOCK
        return new MockSensorDataClient(httpClient,eventHubConnectionString,eventHubName);
#else
        return new SensorDataClient(httpClient);
#endif
    }
}
