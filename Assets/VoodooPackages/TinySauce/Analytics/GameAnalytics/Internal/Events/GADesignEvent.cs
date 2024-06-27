using System.Collections.Generic;
using GameAnalyticsSDK;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    internal class GADesignEvent : GameAnalyticsEvent
    {
        private readonly string eventName;
        private readonly float? eventValue;
        
        
        // Custom Fields
        private readonly Dictionary<string, object> customFields;

        public GADesignEvent(string eventName, float? eventValue = default, Dictionary<string, object> customFields = null) : base(eventName)
        {
            this.eventName = eventName;
            this.eventValue = eventValue;
            this.customFields = customFields;
        }

        protected override void PerformTrackEvent()
        {
            if (eventValue != null) 
            {
                if (customFields != null)
                {
                    GameAnalytics.NewDesignEvent(eventName, (float)eventValue, customFields);
                }
                else
                {
                    GameAnalytics.NewDesignEvent(eventName, (float)eventValue);
                }
            } 
            else 
            {
                if (customFields != null)
                {
                    GameAnalytics.NewDesignEvent(eventName, customFields);
                }
                else
                {
                    GameAnalytics.NewDesignEvent(eventName);
                }
                
            }
        }
    }
}