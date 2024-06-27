using JetBrains.Annotations;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public class AdEventAnalyticsInfo
    {
        [CanBeNull]
        public string AdTag;
        [CanBeNull]
        public string AdUnit;
        [CanBeNull]
        public string AdNetworkName;
        public int? AdLoadingTime;
        public int? GameCount;
        [CanBeNull] 
        public string adPlacement;
    }
    
    
    public class AdShownEventAnalyticsInfo: AdEventAnalyticsInfo
    {
        public int AdCount;
    }
    
    public class AdClickEventAnalyticsInfo: AdEventAnalyticsInfo
    {
        public int AdCount;
    }
    

}