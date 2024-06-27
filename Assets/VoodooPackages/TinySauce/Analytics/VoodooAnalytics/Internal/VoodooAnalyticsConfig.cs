using JetBrains.Annotations;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public static class VoodooAnalyticsConfig
    { 
        private const string TAG = "VoodooAnalyticsConfig";
        [CanBeNull] public static AnalyticsConfig AnalyticsConfig { get; set; }
    }
}