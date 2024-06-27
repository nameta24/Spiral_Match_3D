namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public struct PerformanceMetricsAnalyticsInfo
    {
        public float BatteryLevel;
        public double AverageMemoryUsagePercentage;
        public FloatMetric Fps;
        public FloatMetric MemoryUsage;
        public int BadFrames;
        public int TerribleFrames;
        
        public string GetBatteryLevelAsString()
        {
            if (BatteryLevel >= 0f && BatteryLevel <= 1f) {
                return $"{BatteryLevel * 100:0}%";
            }
            return "";
        }
    }

    public struct FloatMetric
    {
        public FloatMetric(float min, float max, float average)
        {
            Min = min;
            Max = max;
            Average = average;
        }
        
        public float Min;
        public float Max;
        public float Average;
    }
}