namespace UA.Integration.SDK
{
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
}
