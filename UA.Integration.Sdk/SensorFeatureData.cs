namespace UA.Integration.SDK
{
    // Define a detailed structure for sensor data messages
    public class SensorFeatureData
    {
        public Guid Id { get; set; }
        public string SensorId { get; set; }
        public long Timestamp { get; set; }
        public string TraceParentId { get; set; }
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string SensorType { get; set; }
        public string Scope { get; set; }
        public string GatewayId { get; set; }
        public long GwTime { get; set; }
        public long CreatedDate { get; set; }
        public string CoRelationId { get; set; }
        public long CloudTimestamp { get; set; }
        public string TenantId { get; set; }
        public string ObjectClass { get; set; }
        public string ObjectType { get; set; }
        public string ObjectSubType { get; set; }
        public string PreviousHealthStatus { get; set; }
        public string SchemaVersion { get; set; }
        public string FirmwareVer { get; set; }
        public Config config { get; set; }
        public SensorParameters SensorParameters { get; set; }
    }

    // Define a nested structure for sensor parameters
    public class SensorParameters
    {
        public double BatteryStatus_V { get; set; }
        public double BatteryVoltage_V { get; set; }
        public int Temperature_C { get; set; }
        public double RmsHoriz_g { get; set; }
        public double RmsVert_g { get; set; }
        public double RmsAxial_g { get; set; }
        public double PeakHoriz_g { get; set; }
        public double PeakVert_g { get; set; }
        public double PeakAxial_g { get; set; }
        public double PeakToPeakHoriz_g { get; set; }
        public double PeakToPeakVert_g { get; set; }
        public double PeakToPeakAxial_g { get; set; }
        public double CrestFactorHoriz_g { get; set; }
        public double CrestFactorVert_g { get; set; }
        public double CrestFactorAxial_g { get; set; }
        public int SensorSignalStrength_dBm { get; set; }
    }

    public class Config
    {
        public double SampleRate_Hz { get; set; }
        public double AccelRange_g { get; set; }
    }
}
