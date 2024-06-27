using GameAnalyticsSDK;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    internal class GAResourceEvent : GameAnalyticsEvent
    {
        private readonly GAResourceFlowType flowType;
        private readonly string currency;
        private readonly int amount;
        private readonly string itemType;
        private readonly string itemName;
        
        
        public GAResourceEvent(GAResourceFlowType flowType, string currency, int amount, string itemType, string itemName, string eventName = "ResourceEvent") : base(eventName)
        {
            this.flowType = flowType;
            this.currency = currency;
            this.amount = amount;
            this.itemType = itemType;
            this.itemName = itemName;
        }
        
        protected override void PerformTrackEvent()
        {
            GameAnalytics.NewResourceEvent(flowType, currency, amount, itemType, itemName);
                
        }
    }
}