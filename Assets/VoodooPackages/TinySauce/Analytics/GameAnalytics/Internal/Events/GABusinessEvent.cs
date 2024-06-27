using GameAnalyticsSDK;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    internal class GABusinessEvent : GameAnalyticsEvent
    {
        private string currency;
        private int amount;
        private string itemType;
        private string itemId;
        private string cartType;

        public GABusinessEvent(string currency, int amount, string itemType, string itemId, string cartType, string eventName = "BusinessEvent") : base(eventName)
        {
            this.currency = currency;
            this.amount = amount;
            this.itemType = itemType;
            this.itemId = itemId;
            this.cartType = cartType;
        }

        protected override void PerformTrackEvent()
        {
            GameAnalytics.NewBusinessEvent(currency, amount, itemType, itemId, cartType);
        }
    }
}