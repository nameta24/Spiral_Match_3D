using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    internal abstract class BaseAnalyticsEvent
    {
        protected string EventName { get; set; }
        protected string EventId { get; }
        [CanBeNull] protected Dictionary<string, object> EventData { get; set; }
        protected abstract void PerformTrackEvent();
        protected abstract string GetAnalyticsProviderName();

        protected BaseAnalyticsEvent(string eventName, [CanBeNull] Dictionary<string, object> eventData = null)
        {
            EventName = eventName;
            EventData = eventData;
            EventId = Guid.NewGuid().ToString();
        } 

        internal void Track()
        {
            try {
                PerformTrackEvent();
                LogEventInDebugger();
            } catch (Exception e) {
                LogEventExceptionInDebugger(e);
            }
        }
        
        private void LogEventInDebugger()
        {
            if (EventName == null) EventName = GetType().Name;
            AnalyticsEventLogger.GetInstance().LogEventSentTo3rdParty(GetAnalyticsProviderName(), EventName, EventId, EventData);
        }
        private void LogEventExceptionInDebugger(Exception e)
        {
            if (EventName == null) EventName = GetType().Name;
            AnalyticsEventLogger.GetInstance().LogEventException(GetAnalyticsProviderName(), EventName, EventId, EventData, e);
        }
    }
}