using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UA.Integration.SDK
{
    // Define the data schema as a class
    public class WaveformDataAvailableEvent
    {
        public string GatewaySerialNumber { get; set; }
        public string SensorSerialNumber { get; set; }
        public string SensorType { get; set; }
        public string SensorScope { get; set; }
        public long Timestamp { get; set; }
        public MeasurementType MeasurementType { get; set; }
        public string TenantId { get; internal set; }
        public string Id { get; internal set; }
        public string TraceParentId { get; internal set; }
    }
}
