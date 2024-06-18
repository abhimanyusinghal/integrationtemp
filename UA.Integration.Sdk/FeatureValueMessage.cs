using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UA.Integration.SDK
{
    public class FeatureValueMessage
    {
        public string SerialNumber { get; set; }
        public long Timestamp { get; set; }
        public List<FeatureValue> Vert { get; set; }
        public List<FeatureValue> Horz { get; set; }
        public List<FeatureValue> Axial { get; set; }
    }

    public class FeatureValue
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public float Value { get; set; }
        public bool Invalid { get; set; }
    }
}
