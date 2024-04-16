using System.Net.Http.Json;

using static System.Net.WebRequestMethods;

namespace UA.Integration.SDK
{
    public class MockSensorDataClient : ISensorDataClient
    {
        private readonly HttpClient _httpClient;
        private static string _sasUrl = "https://dlsperceptiv01t.blob.core.windows.net/s7100/6124017100203D/LowFrequencySpectrum/2024/01/25/19/15_00_1706210100.json?sp=r&st=2024-04-16T04:00:27Z&se=2025-04-16T12:00:27Z&spr=https&sv=2022-11-02&sr=b&sig=CKYdtntm1unOsV2bBfRvTI2TQsf7OMOS9JjHnyWXi8s%3D";


        public MockSensorDataClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateSingleSasUrl(string sensorSerialNumber, long unixEpochTimestamp, MeasurementType measurementType)
        {
            return _sasUrl;
        }

        public async Task<List<string>> GenerateMultipleSasUrls(string sensorSerialNumber, MeasurementType measurementType, List<long> timestamps)
        {
            return new List<string> { _sasUrl };
        }

        public async Task<List<string>> GenerateSasUrlsForDateRange(string sensorSerialNumber, MeasurementType measurementType, long startDate, long endDate)
        {
            return new List<string> { _sasUrl };
        }

    }
}
