using GameAnalyticsSDK;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    internal class GAAdEvent : GameAnalyticsEvent
    {
        private readonly GAAdAction adAction;
        private readonly GAAdType adType;
        private readonly string adSdkName;
        private readonly string adPlacement;

        public GAAdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement, string eventName = "adEvent") : base(eventName)
        {
            this.adAction = adAction;
            this.adType = adType;
            this.adSdkName = adSdkName;
            this.adPlacement = adPlacement;
        }

        protected override void PerformTrackEvent()
        {
            GameAnalytics.NewAdEvent(adAction, adType, adSdkName, adPlacement);
                
        }
    }
}