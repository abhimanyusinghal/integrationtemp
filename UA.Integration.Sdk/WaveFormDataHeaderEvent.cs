using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UA.Integration.SDK
{
    public class Rootobject
    {
        public string sensorId { get; set; }
        public string traceParentId { get; set; }
        public string traceId { get; set; }
        public string spanId { get; set; }
        public int timestamp { get; set; }
        public string sensorType { get; set; }
        public string scope { get; set; }
        public string measurementType { get; set; }
        public string gatewayId { get; set; }
        public int gwTime { get; set; }
        public string tenantId { get; set; }
        public string objectClass { get; set; }
        public string objectType { get; set; }
        public string objectSubType { get; set; }
        public string previousHealthStatus { get; set; }
        public string schemaVersion { get; set; }
        public string firmwareVer { get; set; }
        public int sensorSignalStrength { get; set; }
        public string unifiedMessageId { get; set; }
    }

}
