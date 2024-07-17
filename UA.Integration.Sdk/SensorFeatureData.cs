namespace UA.Integration.SDK
{
    // Define a detailed structure for sensor data messages
    public class SensorFeatureData
    {
        public string Id { get; set; }
        public string SensorId { get; set; }
        public long Timestamp { get; set; }
        public string TraceParentId { get; set; }
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string SensorType { get; set; }
        public string Scope { get; set; }
        public string GatewayId { get; set; }
        public long GwTime { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public Dictionary<string, object> Config { get; set; } = new Dictionary<string, object>();
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
        public Dictionary<string, object> SensorParameters { get; set; } = new Dictionary<string, object>();
    }
}
