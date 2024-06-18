using System.Net.Http.Json;

namespace UA.Integration.SDK
{
    public class SensorDataClient : ISensorDataClient
    {
        private readonly HttpClient _httpClient;

        public SensorDataClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateSingleSasUrl(string sensorSerialNumber, long unixEpochTimestamp, MeasurementType measurementType)
        {
            var response = await _httpClient.GetAsync($"https://example.com/api/download/single?sensorSerialNumber={sensorSerialNumber}&unixEpochTimestamp={unixEpochTimestamp}&measurementType={measurementType}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<SasUrlResponse>();
            return result?.SasUrl;
        }

        public async Task<List<string>> GenerateMultipleSasUrls(string sensorSerialNumber, MeasurementType measurementType, List<long> timestamps)
        {
            var requestBody = new
            {
                SensorSerialNumber = sensorSerialNumber,
                MeasurementType = measurementType,
                Timestamps = timestamps
            };

            var response = await _httpClient.PostAsJsonAsync("https://example.com/api/download/multiple", requestBody);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<SasUrlsResponse>();
            return result?.SasUrls;
        }

        public async Task<List<string>> GenerateSasUrlsForDateRange(string sensorSerialNumber, MeasurementType measurementType, long startDate, long endDate)
        {
            var requestBody = new
            {
                SensorSerialNumber = sensorSerialNumber,
                MeasurementType = measurementType,
                StartDate = startDate,
                EndDate = endDate
            };

            var response = await _httpClient.PostAsJsonAsync("https://example.com/api/download/range", requestBody);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<SasUrlsResponse>();
            return result?.SasUrls;
        }

        public Task<SensorFeatureData> FetchSingleSensorMessage(string sensorSerialNumber, long unixEpochTimestamp)
        {
            throw new NotImplementedException();
        }

        public Task<List<SensorFeatureData>> FetchMultipleSensorMessages(string sensorSerialNumber, List<long> timestamps)
        {
            throw new NotImplementedException();
        }

        public Task<List<SensorFeatureData>> FetchSensorMessagesForDateRange(string sensorSerialNumber, long startDate, long endDate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PostFeaturesAsync(FeatureValueMessage featureValueMessage)
        {
            throw new NotImplementedException();
        }

        private class SasUrlResponse
        {
            public string SasUrl { get; set; }
        }

        private class SasUrlsResponse
        {
            public List<string> SasUrls { get; set; }
        }
    }
}
