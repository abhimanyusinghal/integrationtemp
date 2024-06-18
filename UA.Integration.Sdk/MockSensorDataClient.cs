using System.Net.Http.Json;

using static System.Net.WebRequestMethods;

namespace UA.Integration.SDK
{
    public class MockSensorDataClient : ISensorDataClient
    {
        private readonly HttpClient _httpClient;
        private static string _sasUrl = "https://dlsperceptiv01t.blob.core.windows.net/s7100/1F24150001000A/LowFrequencySpectrum/2024/05/31/16/10_00_1717171800.json?sp=r&st=2024-06-18T11:41:34Z&se=2024-12-31T19:41:34Z&spr=https&sv=2022-11-02&sr=b&sig=nHVUl2AkV2Qyul2hQM%2F%2BzVsWBhq3M2r0kEc%2Bd3bUVMg%3D";
        private static string _sasUrl2 = "https://dlsperceptiv01t.blob.core.windows.net/s7100/1F24150001000A/RawFullSpectrum/2024/05/28/05/57_00_1716875820.json?sp=r&st=2024-06-18T11:42:39Z&se=2024-12-31T19:42:39Z&spr=https&sv=2022-11-02&sr=b&sig=VvQPXub5OhUDb78Ny6m7FRmw79M7swzjANvfv4ac2fU%3D";
        private readonly string _eventHubConnectionString;
        private readonly string _eventHubName;
        private int _no;

        public MockSensorDataClient(HttpClient httpClient, string eventHubConnectionString, string eventHubName)
        {
            _httpClient = httpClient;
            _eventHubConnectionString = eventHubConnectionString;
            _eventHubName = eventHubName;
            _no = 1;
        }

        public async Task<string> GenerateSingleSasUrl(string sensorSerialNumber, long unixEpochTimestamp, MeasurementType measurementType)
        {
            _no = _no + 1;
            if (_no%2==1)
            {
                return _sasUrl;
            }
            else
            {
                return _sasUrl2;
            }
        }

        public async Task<List<string>> GenerateMultipleSasUrls(string sensorSerialNumber, MeasurementType measurementType, List<long> timestamps)
        {
            return new List<string> { _sasUrl,_sasUrl2 };
        }

        public async Task<List<string>> GenerateSasUrlsForDateRange(string sensorSerialNumber, MeasurementType measurementType, long startDate, long endDate)
        {
            return new List<string> { _sasUrl,_sasUrl2 };
        }

        public async Task<SensorFeatureData> FetchSingleSensorMessage(string sensorSerialNumber, long unixEpochTimestamp)
        {
            return await Task.FromResult(CreateMockSensorDataMessage(sensorSerialNumber, unixEpochTimestamp));
        }

        public async Task<List<SensorFeatureData>> FetchMultipleSensorMessages(string sensorSerialNumber,  List<long> timestamps)
        {
            var messages = new List<SensorFeatureData>();
            foreach (var timestamp in timestamps)
            {
                messages.Add(CreateMockSensorDataMessage(sensorSerialNumber, timestamp));
            }
            return await Task.FromResult(messages);
        }

        public async Task<List<SensorFeatureData>> FetchSensorMessagesForDateRange(string sensorSerialNumber, long startDate, long endDate)
        {
            var messages = new List<SensorFeatureData>();
            for (long date = startDate; date <= endDate; date += 86400) // Assume daily granularity
            {
                messages.Add(CreateMockSensorDataMessage(sensorSerialNumber, date));
            }
            return await Task.FromResult(messages);
        }

        private SensorFeatureData CreateMockSensorDataMessage(string sensorSerialNumber, long timestamp)
        {
            return new SensorFeatureData
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
                config = new Config()
                { AccelRange_g = 32f,SampleRate_Hz = 400.0f},

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

        public async Task<bool> PostFeaturesAsync(FeatureValueMessage featureValueMessage)
        {
            //Code to Store / Post the Feature Value Message to Event Hub
            Console.WriteLine("Received " + featureValueMessage);
            return  true;
        }
    }
}