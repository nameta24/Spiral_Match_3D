﻿using System.Collections.Generic;
 using JetBrains.Annotations;
 using Voodoo.Tiny.Sauce.Internal.Analytics;

 namespace Voodoo.Analytics
{
    public static class VoodooAnalyticsManager
    {
        private const string TAG = "VoodooAnalyticsManager";

        private static bool _isInitialized;

        private static readonly List<QueuedEvent> QueuedEvents = new List<QueuedEvent>();

        private static readonly Dictionary<AnalyticParameters, object> AnalyticsParameters = new Dictionary<AnalyticParameters, object>();

        public static readonly GlobalContext GlobalContext = new GlobalContext();

        /// <summary>
        ///   <param>Add parameters to Analytics before init it.</param>
        /// </summary>
        /// <param name="sessionParameters">List of parameters</param>
        public static void AddSessionParameters(Dictionary<AnalyticParameters, object> sessionParameters)
        {
            foreach (KeyValuePair<AnalyticParameters, object> parameter in sessionParameters) {
                if (!AnalyticsParameters.ContainsKey(parameter.Key)) {
                    AnalyticsParameters.Add(parameter.Key, parameter.Value);
                }
            }
        }

        /// <summary>
        ///   <param>Add parameter to Analytics before init it.</param>
        /// </summary>
        /// <param name="sessionParameter">Parameter key</param>
        /// <param name="value">Value of this key</param>
        public static void AddSessionParameter(AnalyticParameters sessionParameter, object value)
        {
            if (!AnalyticsParameters.ContainsKey(sessionParameter)) {
                AnalyticsParameters.Add(sessionParameter, value);
            }
        }

        /// <summary>
        ///   <param>Init Analytics Manager.</param>
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="proxyServer">Server proxy</param>
        public static void Init(IConfig config, string proxyServer)
        {
            if (_isInitialized) {
                return;
            }

            _isInitialized = true;

            Tracker.Instance.Init(config, proxyServer);

            SendQueuedEvents();
        }

        private static void SendQueuedEvents()
        {
            QueuedEvents.ForEach(queuedEvent => {
                InternalTrackEvent(queuedEvent.EventName,
                    queuedEvent.EventDataJson,
                    queuedEvent.EventType,
                    queuedEvent.EventCustomVariablesJson,
                    queuedEvent.EventContextVariablesJson,
                    queuedEvent.EventId);
            });

            QueuedEvents.Clear();
        }

        /// <summary>
        ///   <param>Track an event (list in EventName)</param>
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="data">Internal data</param>
        /// <param name="eventType">event type</param>
        /// <param name="customVariables">External data (set in game)</param>
        /// <param name="contextVariables">External data (set in game)</param>
        /// <param name="eventId">event identifier</param>
        public static void TrackEvent(EventName eventName,
                                      Dictionary<string, object> data,
                                      string eventType = null,
                                      Dictionary<string, object> customVariables = null,
                                      string eventId = null,
                                      Dictionary<string, object> contextVariables = null)
        {
            TrackEvent(eventName.ToString(), data, eventType, customVariables, contextVariables, eventId);
        }

        /// <summary>
        ///   <param>Track custom  event (not list in EventName)</param>
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="eventType">event type</param>
        /// <param name="eventId">event identifier</param>
        /// <param name="customVariables">External data (set in game)</param>
        /// <param name="contextVariables">External data (set in game)</param>
        public static void TrackCustomEvent(string eventName,
                                            Dictionary<string, object> customVariables,
                                            string eventType = null,
                                            string eventId = null,
                                            Dictionary<string, object> contextVariables = null)
        {
            TrackEvent(eventName, null, eventType, customVariables, contextVariables, eventId);
        }

        /// <summary>
        ///   <param>Track an event</param>
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="data">Internal data</param>
        /// <param name="eventType">event type</param>
        /// <param name="customVariables">External data (set in game)</param>
        /// <param name="contextVariables">External data (set in game)</param>
        /// <param name="eventId">event identifier</param>
        private static void TrackEvent(string eventName,
                                       Dictionary<string, object> data,
                                       string eventType = null,
                                       Dictionary<string, object> customVariables = null,
                                       Dictionary<string, object> contextVariables = null,
                                       string eventId = null)
        {
            string dataJson = null;
            if (data != null) {
                dataJson = AnalyticsUtil.ConvertDictionaryToJson(data);
            }

            string customVariablesJson = null;
            if (customVariables != null) {
                customVariablesJson = AnalyticsUtil.ConvertDictionaryToCustomVarJson(customVariables);
            }

            string contextVariablesJson = CreateContextVariablesJson(contextVariables);
            
            InternalTrackEvent(eventName, dataJson, eventType, customVariablesJson, contextVariablesJson, eventId);
        }

        /// <summary>
        ///   <param>Track an event (in EventName list)</param>
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="data">Internal data</param>
        /// <param name="eventType">event type</param>
        /// <param name="customVariables">External data (set in game)</param>
        /// <param name="contextVariables">External data (set in game)</param>
        /// <param name="eventId">event identifier</param>
        public static void TrackEvent(EventName eventName,
                                      string data = null,
                                      string eventType = null,
                                      Dictionary<string, object> customVariables = null,
                                      string eventId = null,
                                      Dictionary<string, object> contextVariables = null)
        {
            string customVariablesJson = null;
            if (customVariables != null) {
                customVariablesJson = AnalyticsUtil.ConvertDictionaryToCustomVarJson(customVariables);
            }

            string contextVariablesJson = CreateContextVariablesJson(contextVariables);

            InternalTrackEvent(eventName.ToString(), data, eventType, customVariablesJson, contextVariablesJson, eventId);
        }

        private static void InternalTrackEvent(string eventName,
                                               string dataJson,
                                               string eventType,
                                               string customVariablesJson,
                                               string contextVariablesJson,
                                               [CanBeNull] string eventId)
        {
            if (!_isInitialized) {
                var queuedEvent = new QueuedEvent {
                    EventName = eventName,
                    EventDataJson = dataJson,
                    EventType = eventType,
                    EventCustomVariablesJson = customVariablesJson,
                    EventContextVariablesJson = contextVariablesJson,
                    EventId = eventId
                };
                AnalyticsLog.Log(TAG, "Add event " + eventName + " to the queue (" + dataJson + ")");
                QueuedEvents.Add(queuedEvent);
                return;
            }

            AnalyticsSessionHelper.DefaultHelper().OnNewEvent();

            AnalyticsLog.Log(TAG, "Create event " + eventName + " (" + dataJson + ")");
            Event.Create(eventName,
                AnalyticsParameters,
                dataJson,
                customVariablesJson,
                contextVariablesJson,
                eventType,
                AnalyticsSessionHelper.DefaultHelper().SessionId,
                AnalyticsSessionHelper.DefaultHelper().SessionLength,
                AnalyticsSessionHelper.DefaultHelper().SessionCount,
                async e => { await Tracker.Instance.TrackEvent(e); },
                eventId);
        }

        private static string CreateContextVariablesJson(Dictionary<string, object> contextVariables)
        {
            string contextVariablesJson = null;
            FillGlobalContextVariables(ref contextVariables);
            if (contextVariables != null) {
                contextVariablesJson = AnalyticsUtil.ConvertDictionaryToContextVarJson(contextVariables);
            }
            return contextVariablesJson;
        }
        
        private static void FillGlobalContextVariables(ref Dictionary<string, object> contextVariables)
        {
            var parameters = GlobalContext.GetParameters();
            if (parameters.Count == 0) {
                return;
            }
            if (contextVariables == null) {
                contextVariables = new Dictionary<string, object>();
            }
            foreach (KeyValuePair<string, string> pair in parameters) {
                contextVariables.Add(pair.Key, pair.Value);
            }
        }

#if UNITY_EDITOR
        
        /// <summary>
        ///   <param>Replace Session Parameters with other values</param>
        /// </summary>
        /// <param name="sessionParameters">List of parameters</param>
        internal static void ReplaceSessionParameter(Dictionary<AnalyticParameters, object> sessionParameters)
        {
            AnalyticsParameters.Clear();
            foreach (KeyValuePair<AnalyticParameters, object> parameter in sessionParameters) {
                if (!AnalyticsParameters.ContainsKey(parameter.Key)) {
                    AnalyticsParameters.Add(parameter.Key, parameter.Value);
                }
            }
        }
        
        /// <summary>
        ///   <param>Get session parameters</param>
        /// </summary>
        internal static Dictionary<AnalyticParameters, object> GetSessionParameter() => AnalyticsParameters;
#endif

        private struct QueuedEvent
        {
            public string EventName;
            public string EventDataJson;
            public string EventType;
            public string EventCustomVariablesJson;
            public string EventContextVariablesJson;
            [CanBeNull] public string EventId;
        }
    }
}