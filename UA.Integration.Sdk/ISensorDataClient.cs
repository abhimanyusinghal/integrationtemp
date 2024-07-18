namespace UA.Integration.SDK
{
    public interface ISensorDataClient
    {
        Task<string> GenerateSingleSasUrl(string sensorSerialNumber, MeasurementType measurementType, long unixEpochTimestamp);
        Task<List<string>> GenerateMultipleSasUrls(string sensorSerialNumber, MeasurementType measurementType, List<long> timestamps);
        Task<List<string>> GenerateSasUrlsForDateRange(string sensorSerialNumber, MeasurementType measurementType, long startDate, long endDate);

        Task<SensorFeatureData> FetchSingleSensorMessage(string sensorSerialNumber, long unixEpochTimestamp);
        Task<List<SensorFeatureData>> FetchMultipleSensorMessages(string sensorSerialNumber, List<long> timestamps);
        Task<List<SensorFeatureData>> FetchSensorMessagesForDateRange(string sensorSerialNumber, long startDate, long endDate);
        IAsyncEnumerable<string> GenerateSasUrlsForDateRangeAsync(string sensorSerialNumber, MeasurementType measurementType, long startDate, long endDate);
    }
}
