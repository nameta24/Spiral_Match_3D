using System.Collections.Generic;
using UnityEngine;
using Voodoo.Analytics;
using Voodoo.Sauce.Internal;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public static class VoodooAnalyticsWrapper
    {
        private const string TAG = "VoodooAnalyticsWrapper";
        public static void TrackEvent(EventName eventName,
                                      Dictionary<string, object> data,
                                      string eventType = null,
                                      Dictionary<string, object> customVariables = null,
                                      Dictionary<string, object> contextVariables = null)
        {
            new VoodooAnalyticsLoggerEvent(eventName, data, eventType, customVariables, contextVariables).Track();
        }
        
        public static void TrackCustomEvent(string eventName,
                                            Dictionary<string, object> customVariables,
                                            string eventType = null,
                                            Dictionary<string, object> contextVariables = null)
        {
            new VoodooAnalyticsLoggerEvent(eventName, customVariables, eventType, contextVariables).Track();
        }
        
        public static void TrackEvent(EventName eventName,
                                      string data = null,
                                      string eventType = null,
                                      Dictionary<string, object> customVariables = null,
                                      Dictionary<string, object> contextVariables = null)
        {
            new VoodooAnalyticsLoggerEvent(eventName, data, eventType, customVariables, contextVariables).Track();
        }
        
        public static void Instantiate(AnalyticsConfig analyticsConfig,
                                       bool useVoodooTune,
                                       string legacyAbTestName,
                                       string mediation,
                                       string proxyServer,
                                       string editorIdfa)
        {
            VoodooAnalyticsManager.AddSessionParameters(new Dictionary<AnalyticParameters, object> {
                {AnalyticParameters.VoodooSauceVersion, TinySauce.Version + "_TS"},
                {AnalyticParameters.AppVersion, Application.version},
                {AnalyticParameters.SegmentationUuid, null},
                {AnalyticParameters.AbTestUuid, legacyAbTestName},
                {AnalyticParameters.AbTestCohortUuid, TinySauce.GetABTestCohort()},
                {AnalyticParameters.AbTestVersionUuid,  null},
                {AnalyticParameters.Mediation, mediation?? VoodooAnalyticsConstants.DEFAULT_MEDIATION},
                {AnalyticParameters.AppBundleId, Application.identifier},
#if UNITY_EDITOR
                 {AnalyticParameters.EditorIdfa, editorIdfa}
#endif
            });
            VoodooAnalyticsManager.Init(analyticsConfig, proxyServer);
        }

        public static void AddSessionParameter(AnalyticParameters sessionParameter, object value)
        {
            VoodooAnalyticsManager.AddSessionParameter(sessionParameter, value);
        } 
    }
}