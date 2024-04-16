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

        public async Task<SensorDataMessage> FetchSingleSensorMessage(string sensorSerialNumber, long unixEpochTimestamp)
        {
            return await Task.FromResult(CreateMockSensorDataMessage(sensorSerialNumber, unixEpochTimestamp));
        }

        public async Task<List<SensorDataMessage>> FetchMultipleSensorMessages(string sensorSerialNumber,  List<long> timestamps)
        {
            var messages = new List<SensorDataMessage>();
            foreach (var timestamp in timestamps)
            {
                messages.Add(CreateMockSensorDataMessage(sensorSerialNumber, timestamp));
            }
            return await Task.FromResult(messages);
        }

        public async Task<List<SensorDataMessage>> FetchSensorMessagesForDateRange(string sensorSerialNumber, long startDate, long endDate)
        {
            var messages = new List<SensorDataMessage>();
            for (long date = startDate; date <= endDate; date += 86400) // Assume daily granularity
            {
                messages.Add(CreateMockSensorDataMessage(sensorSerialNumber, date));
            }
            return await Task.FromResult(messages);
        }

        private SensorDataMessage CreateMockSensorDataMessage(string sensorSerialNumber, long timestamp)
        {
            return new SensorDataMessage
            {
                Id = Guid.NewGuid(),
                SensorId = sensorSerialNumber,
                Timestamp = timestamp,
                TraceParentId = "00-09778b236aa7b413c765d3103a5b044e-fcf192e379e9f393-00",
                TraceId = "09778b236aa7b413c765d3103a5b044e",
                SpanId = "ff000e21ac2eb59f",
                SensorType = "s7100",
                Scope = "ScopeDetails",
                GatewayId = "11240400030006",
                GwTime = timestamp + 100,
                CreatedDate = timestamp + 200,
                CoRelationId = Guid.NewGuid().ToString(),
                CloudTimestamp = timestamp + 300,
                TenantId = "TenantDetails",
                ObjectClass = "Device",
                ObjectType = "sensor",
                ObjectSubType = "S7100",
                PreviousHealthStatus = "Healthy",
                SchemaVersion = "1.0",
                FirmwareVer = "1.0.3",
                SensorParameters = new SensorParameters
                {
                    BatteryStatus_V = 3.7,
                    BatteryVoltage_V = 4.2,
                    Temperature_C = 25,
                    RmsHoriz_g = 0.05,
                    RmsVert_g = 0.06,
                    RmsAxial_g = 0.07,
                    PeakHoriz_g = 0.1,
                    PeakVert_g = 0.2,
                    PeakAxial_g = 0.3,
                    PeakToPeakHoriz_g = 0.4,
                    PeakToPeakVert_g = 0.5,
                    PeakToPeakAxial_g = 0.6,
                    CrestFactorHoriz_g = 4,
                    CrestFactorVert_g = 5,
                    CrestFactorAxial_g = 4.5,
                    SensorSignalStrength_dBm = -80
                }
            };
        }
    }
}
