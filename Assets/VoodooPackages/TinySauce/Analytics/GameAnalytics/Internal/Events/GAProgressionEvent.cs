using System.Collections.Generic;
using GameAnalyticsSDK;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
        internal class GAProgressionEvent : GameAnalyticsEvent
        {
            // Progression status : Start, Complete or Failed
            private readonly GAProgressionStatus status;
            
            // Progressions dimensions. The 2nd and 3rd can be null
            private readonly string progression;
            private readonly string progression2 = null;
            private readonly string progression3 = null;
            
            // Score
            private readonly int? score;
            
            // Custom Fields
            private readonly Dictionary<string, object> customFields;

            
            public GAProgressionEvent( GAProgressionStatus status, string progression, string progression2 = null, string progression3 = null, int? score = null, Dictionary<string, object> customFields = null) : base(status.ToString())
            {
                this.status = status;
                this.progression = progression;
                this.progression2 = progression2;
                this.progression3 = progression3;
                this.score = score;
                this.customFields = customFields;
            }
            
            protected override void PerformTrackEvent()
            {
                if (score != null) 
                {
                    if (customFields != null)
                    {
                        GameAnalytics.NewProgressionEvent(status, progression, progression2, progression3, (int) score, customFields);
                    }
                    else
                    {
                        GameAnalytics.NewProgressionEvent(status, progression, progression2, progression3, (int) score);
                    }
                    
                }
                else 
                {
                    if (customFields != null)
                    {
                        GameAnalytics.NewProgressionEvent(status, progression, progression2, progression3, customFields);
                    }
                    else
                    {
                        GameAnalytics.NewProgressionEvent(status, progression, progression2, progression3);
                    }
                    
                }
            }
            
        }
}