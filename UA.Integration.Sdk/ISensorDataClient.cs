namespace UA.Integration.SDK
{
    public interface ISensorDataClient
    {
        Task<string> GenerateSingleSasUrl(string sensorSerialNumber, long unixEpochTimestamp, MeasurementType measurementType);
        Task<List<string>> GenerateMultipleSasUrls(string sensorSerialNumber, MeasurementType measurementType, List<long> timestamps);
        Task<List<string>> GenerateSasUrlsForDateRange(string sensorSerialNumber, MeasurementType measurementType, long startDate, long endDate);

        Task<SensorDataMessage> FetchSingleSensorMessage(string sensorSerialNumber, long unixEpochTimestamp);
        Task<List<SensorDataMessage>> FetchMultipleSensorMessages(string sensorSerialNumber, List<long> timestamps);
        Task<List<SensorDataMessage>> FetchSensorMessagesForDateRange(string sensorSerialNumber, long startDate, long endDate);
    }
}
