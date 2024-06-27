namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public class VoodooAnalyticsParameters
    {
        
        private const string TAG = "VoodooAnalyticsParameters";
        public bool UseVoodooTune { get;}
        public bool UseVoodooAnalytics { get;}
        public string LegacyABTestName { get; }
        public string EditorIdfa { get;}
        public string ProxyServer { get;}

        public VoodooAnalyticsParameters(bool useVoodooTune, bool useVoodooAnalytics, string legacyAbTestName, string proxyServer = "")
        {
            UseVoodooTune = useVoodooTune;
            UseVoodooAnalytics = useVoodooAnalytics;
            LegacyABTestName = legacyAbTestName;
            ProxyServer = proxyServer;
        }
    }
}