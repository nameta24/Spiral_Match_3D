using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Voodoo.Sauce.Internal.Utils;
using Voodoo.Tiny.Sauce.Internal.Analytics;
#if UNITY_IOS
using UnityEngine.iOS;
using Voodoo.Sauce.Internal.IdfaAuthorization;
#endif

namespace Voodoo.Analytics
{
    internal class Event
    {

        private const string EventTypeImpression = "impression";
        private const string EventTypeApp = "app";
        private const string EventTypeCustom = "custom";

        private Dictionary<string, object> _values;
        private string _name;
        private string _jsonData;
        private string _customVariablesData;
        private string _contextVariablesData;

        internal string GetName() => _name;

        private static readonly string[] ImpressionEvents = {
            EventName.fs_shown.ToString(),
            EventName.fs_click.ToString(),
            EventName.fs_watched.ToString(),
            EventName.fs_trigger.ToString(),
            EventName.rv_shown.ToString(),
            EventName.rv_click.ToString(),
            EventName.rv_watched.ToString(),
            EventName.rv_trigger.ToString(),
            EventName.banner_shown.ToString(),
            EventName.banner_click.ToString(),
            EventName.ad_revenue.ToString(),
            EventName.cp_impression.ToString(),
            EventName.cp_click.ToString(),
            EventName.cp_response_status.ToString(),
            EventName.attribution_changed.ToString()
        };

        private Event() { }
        
        
        
        

        // UnityEngine's classes shouldn't be used here because this method may be called in a background thread 
        internal static void Create(string name,
                                  Dictionary<AnalyticParameters, object> parameters,
                                  string dataJson,
                                  string customVariablesJson,
                                  string contextVariablesJson,
                                  string type,
                                  string sessionId,
                                  int sessionLength,
                                  int sessionCount,
                                  Action<Event> complete,
                                  [CanBeNull] string eventId)
        {
            var eventValues = new Dictionary<string, object> {
                {AnalyticsConstant.VS_VERSION, parameters[AnalyticParameters.VoodooSauceVersion]?.ToString()},
                {AnalyticsConstant.ID, eventId ?? Guid.NewGuid().ToString()},
                {AnalyticsConstant.USER_ID, AnalyticsUserIdHelper.GetUserId()},
                {AnalyticsConstant.NAME, name},
                {AnalyticsConstant.TYPE, type ?? GetType(name)},
                {AnalyticsConstant.CREATED_AT, DateTime.UtcNow.ToIsoFormat()},
                {AnalyticsConstant.BUNDLE_ID, parameters[AnalyticParameters.AppBundleId]},
                {AnalyticsConstant.APP_VERSION, parameters[AnalyticParameters.AppVersion]},
                {AnalyticsConstant.SESSION_ID, sessionId},
                {AnalyticsConstant.SESSION_LENGTH, sessionLength},
                {AnalyticsConstant.SESSION_COUNT, sessionCount},
                {AnalyticsConstant.APP_OPEN_COUNT, AnalyticsStorageHelper.GetAppLaunchCount()},
                {AnalyticsConstant.USER_GAME_COUNT, AnalyticsStorageHelper.GetGameCount()}, 
                
                // TODO : Ask if Advertising id is important and if so connect it <ith the privacy settings
                {AnalyticsConstant.ADVERTISING_ID, "00000000-0000-0000-0000-000000000000"},
                
                
                // UNUSED VARIABLES 
                
                // {AnalyticsConstant.SCREEN_RESOLUTION, DeviceUtils.GetResolution()},
                 {AnalyticsConstant.OS_VERSION, DeviceUtils.OperatingSystemVersion},
                // {AnalyticsConstant.MANUFACTURER, DeviceUtils.Manufacturer},
                // {AnalyticsConstant.MODEL, DeviceUtils.Model},
                // {AnalyticsConstant.DEVELOPER_DEVICE_ID, privacy.GetVendorId().Replace("-", "").ToLower()},
                // {AnalyticsConstant.ADS_CONSENT_GIVEN, privacy.HasAdsConsent()},
                // {AnalyticsConstant.ANALYTICS_CONSENT_GIVEN, privacy.HasAnalyticsConsent()},
                // {AnalyticsConstant.LOCALE, DeviceUtils.GetLocale()},
                // {AnalyticsConstant.CONNECTIVITY, DeviceUtils.GetConnectivity()},
                
                
#if UNITY_EDITOR
                {AnalyticsConstant.PLATFORM, "editor"},
                {AnalyticsConstant.ADVERTISING_ID, parameters[AnalyticParameters.EditorIdfa]?.ToString()},
                {AnalyticsConstant.LIMIT_AD_TRACKING, true},
#elif UNITY_ANDROID
                {AnalyticsConstant.PLATFORM, "android"},
                {AnalyticsConstant.DEVELOPER_DEVICE_ID, ""},
#elif UNITY_IOS
                {AnalyticsConstant.DEVELOPER_DEVICE_ID, UnityEngine.iOS.Device.vendorIdentifier},
                {AnalyticsConstant.PLATFORM, "ios"},
                {AnalyticsConstant.IDFA_AUTHORIZATION_STATUS, NativeWrapper.GetAuthorizationStatus().ToString()},
#endif
            };

            if (ParameterHasValue(parameters, AnalyticParameters.SegmentationUuid))
            {
                eventValues.Add(AnalyticsConstant.SEGMENT_UUID, parameters[AnalyticParameters.SegmentationUuid].ToString());                
            }
            
            if (ParameterHasValue(parameters, AnalyticParameters.AbTestUuid))
            {
                eventValues.Add(AnalyticsConstant.AB_TEST_UUID, parameters[AnalyticParameters.AbTestUuid].ToString());                
            }
            
            if (ParameterHasValue(parameters, AnalyticParameters.AbTestCohortUuid))
            {
                eventValues.Add(AnalyticsConstant.COHORT_UUID, parameters[AnalyticParameters.AbTestCohortUuid].ToString());                
            }
            
            if (ParameterHasValue(parameters, AnalyticParameters.InstallStore))
            {
                eventValues.Add(AnalyticsConstant.INSTALL_STORE, parameters[AnalyticParameters.InstallStore].ToString());
            }   
            
            if (ParameterHasValue(parameters, AnalyticParameters.FirstAppLaunchDate))
            {
                eventValues.Add(AnalyticsConstant.FIRST_APP_LAUNCH_DATE, parameters[AnalyticParameters.FirstAppLaunchDate].ToString());
            }

            if (ParameterHasValue(parameters, AnalyticParameters.AbTestVersionUuid))
            {
                eventValues.Add(AnalyticsConstant.VERSION_UUID, parameters[AnalyticParameters.AbTestVersionUuid].ToString());                
            }
            
            if (parameters.ContainsKey(AnalyticParameters.Mediation) && !string.IsNullOrEmpty(parameters[AnalyticParameters.Mediation].ToString())) {
                eventValues.Add(AnalyticsConstant.MEDIATION, parameters[AnalyticParameters.Mediation].ToString());
            }

//            if (!string.IsNullOrEmpty(AnalyticsManager.GetAttributionData()?.Name)) {
//                eventValues.Add(AnalyticsConstant.ATTRIBUTION_PROVIDER_NAME, AnalyticsManager.GetAttributionData().Name);
//            }
//
//            if (!string.IsNullOrEmpty(AnalyticsManager.GetAttributionData()?.UserId)) {
//                eventValues.Add(AnalyticsConstant.ATTRIBUTION_USER_ID, AnalyticsManager.GetAttributionData().UserId);
//            }
            
#if !UNITY_EDITOR
            complete(new Event {_values = eventValues, _name = name, _jsonData = dataJson, _customVariablesData = customVariablesJson, _contextVariablesData = contextVariablesJson});
#else
            // eventValues.Add(AnalyticsConstant.ADVERTISING_ID, privacy.GetAdvertisingId());
            // eventValues.Add(AnalyticsConstant.LIMIT_AD_TRACKING, privacy.HasLimitAdTrackingEnabled());
#if UNITY_ANDROID
//            eventValues.Add(AnalyticsConstant.IDFA_AUTHORIZATION_STATUS, privacy.HasLimitAdTrackingEnabled() ? IdfaAuthorizationStatus.Denied.ToString() : IdfaAuthorizationStatus.Authorized.ToString());
#endif 
            complete(new Event {_values = eventValues, _name = name, _jsonData = dataJson, _customVariablesData = customVariablesJson, _contextVariablesData = contextVariablesJson});
#endif
        }

        private static bool ParameterHasValue(Dictionary<AnalyticParameters, object> parameters, AnalyticParameters parameterKey) =>
            parameters.ContainsKey(parameterKey) && !string.IsNullOrEmpty(parameters[parameterKey]?.ToString());

        private static string GetType(string name)
        {
            string type;
            if (!Enum.IsDefined(typeof(EventName), name)) {
                type = EventTypeCustom;
            } else if (ImpressionEvents.Contains(name)) {
                type = EventTypeImpression;
            } else {
                type = EventTypeApp;
            }

            return type;
        }

        internal string ToJson() => AnalyticsUtil.ConvertDictionaryToJson(_values, _jsonData, _customVariablesData, _contextVariablesData);

        public override string ToString() => _values.ToString();
    }
}