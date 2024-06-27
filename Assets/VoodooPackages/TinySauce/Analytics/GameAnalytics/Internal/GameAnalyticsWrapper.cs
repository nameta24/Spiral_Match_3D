using System;
using System.Collections.Generic;
using System.Reflection;
using GameAnalyticsSDK;
using UnityEngine;
using Voodoo.Sauce.Internal;
using Object = UnityEngine.Object;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public static class GameAnalyticsWrapper
    {
        private const string TAG = "GameAnalyticsWrapper";

        private static bool _isInitialized;
        private static bool _isDisabled;

        private static readonly Queue<GameAnalyticsEvent> QueuedEvents = new Queue<GameAnalyticsEvent>();


        #region Initialisation
        
        internal static bool Initialize(bool consent)
        {
            if (!consent) {
                Disable();
                return _isInitialized;
            }

            InstantiateGameAnalytics();
            VoodooLog.Log(TAG, "GameAnalytics initialized, tracking pending events: " + QueuedEvents.Count);
            while (QueuedEvents.Count > 0) {
                QueuedEvents.Dequeue().Track();
            }

            _isInitialized = true;
            return _isInitialized;
        }
        
        private static void SetBuildVersion(string buildVersion)
        {
            int platformIndex = -1;
#if UNITY_ANDROID
            platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(RuntimePlatform.Android);
#elif UNITY_IOS
            platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(RuntimePlatform.IPhonePlayer);
#endif
            if (platformIndex >= 0)
            {
                GameAnalytics.SettingsGA.Build[platformIndex] = buildVersion;
            }
        }
        

        private static void InstantiateGameAnalytics()
        {
            var gameAnalyticsComponent = Object.FindObjectOfType<GameAnalytics>();
            if (gameAnalyticsComponent == null) {
                var gameAnalyticsGameObject = new GameObject("GameAnalytics");
                gameAnalyticsGameObject.AddComponent<GameAnalytics>();
                gameAnalyticsGameObject.SetActive(true);
            } else {
                gameAnalyticsComponent.gameObject.name = "GameAnalytics";
            }
            

            string appVersionName = Application.version;
            
            if (Type.GetType("Voodoo.Sauce.Internal.Ads.TSAdsManager") != null)
                if ((bool) Type.GetType("Voodoo.Sauce.Internal.Ads.TSAdsManager").GetField("_areAdsEnabled", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null))
                    appVersionName += "-Ads";

            GameAnalytics.SettingsGA.SubmitFpsAverage = true;
            GameAnalytics.SettingsGA.SubmitFpsCritical = true;
            

            if (!string.IsNullOrEmpty(TinySauce.GetABTestCohort()))
            {
                SetBuildVersion($"{appVersionName}-ABCohort:{TinySauce.GetABTestCohort() ?? "Default"}");
            }
            else
            {
                SetBuildVersion($"{appVersionName}");
            }
            
            SetCustomFields();
            
            GameAnalytics.Initialize();
        }

        private static void SetCustomFields()
        {
            Dictionary<string, object> customFields = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(TinySauce.GetABTestCohort()))
            {
                customFields.Add("ABCohort", TinySauce.GetABTestCohort());
            }
            customFields.Add("TSVersion", TinySauce.Version);
            GameAnalytics.SetGlobalCustomEventFields(customFields);
        }

        private static void Disable()
        {
            VoodooLog.Log(TAG, "Disabling GameAnalytics No User Consent.");
            _isDisabled = true;
            QueuedEvents.Clear();
        }

        #endregion

        #region ProgressionEvents
            
       
        internal static void TrackProgressEvent(GAProgressionStatus status, string progress1, string progress2, string progress3, int? score)
        {
            if (_isDisabled) return;

            GAProgressionEvent progressEvent = null;
            progressEvent = new GAProgressionEvent(status, progress1, progress2, progress3, score);
            
            if (!_isInitialized) {
                VoodooLog.Log(TAG, "GameAnalytics NOT initialized queuing event..." + status);
                QueuedEvents.Enqueue(progressEvent);
            } else {
                VoodooLog.Log(TAG, "Sending event \"" + status + "\" to GameAnalytics");
                progressEvent.Track();
            }
        }

        #endregion

        #region DesignEvents

        internal static void TrackDesignEvent(string eventName, float? eventValue)
        {
            if (_isDisabled) return;

            var designEvent = new GADesignEvent(eventName, eventValue);
            
            if (!_isInitialized) {
                VoodooLog.Log(TAG, "GameAnalytics NOT initialized queuing event..." + eventName);
                QueuedEvents.Enqueue(designEvent);
            } else {
                VoodooLog.Log(TAG, "Sending event \"" + eventName + "\" to GameAnalytics");
                designEvent.Track();
            }
        }
            
        #endregion

        #region AdEvents
        
        internal static void TrackAdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement)
        {
            if (_isDisabled) return;

            var adEvent = new GAAdEvent(adAction, adType, adSdkName, adPlacement);

            if (!_isInitialized) {
                VoodooLog.Log(TAG, "GameAnalytics NOT initialized queuing event..." + adType);
                QueuedEvents.Enqueue(adEvent);
            } else {
                VoodooLog.Log(TAG, "Sending event " + adType + " to GameAnalytics");
                adEvent.Track();
            }
        }

        #endregion

        #region ResourceEvents

        internal static void DeclareCurrency(string currency)
        {
            GameAnalytics.SettingsGA.ResourceCurrencies.Add(currency);
        }

        internal static void DeclareItemType(string itemType)
        {
            GameAnalytics.SettingsGA.ResourceItemTypes.Add(itemType);
        }

        internal static void TrackResourceEvent(GAResourceFlowType flowType, string currency, int amount, string itemType, string itemName)
        {
            if (_isDisabled) return;

            var resourceEvent = new GAResourceEvent(flowType, currency, amount, itemType, itemName);
            if (!_isInitialized) {
                VoodooLog.Log(TAG, "GameAnalytics NOT initialized queuing event..." + flowType);
                QueuedEvents.Enqueue(resourceEvent);
            } else {
                VoodooLog.Log(TAG, "Sending event " + flowType + " to GameAnalytics");
                resourceEvent.Track();
            }
        }

        #endregion

        #region Business Events

        internal static void TrackBusinessEvent(string currency, int amount, string itemType, string itemID, string cartType)
        {
            
            if (_isDisabled) return;

            var businessEvent = new GABusinessEvent(currency, amount, itemType, itemID, cartType);
            if (!_isInitialized) {
                VoodooLog.Log(TAG, "GameAnalytics NOT initialized queuing event... Business Event : " + itemType + ", "+ itemID + ":" + amount);
                QueuedEvents.Enqueue(businessEvent);
            } else {
                VoodooLog.Log(TAG, "Sending event Business Event : " + itemType + ", "+ itemID + ":" + amount + " to GameAnalytics");
                businessEvent.Track();
            }
        }
        
        /*
        internal static void TrackDesignEvent(string eventName, float? eventValue)
        {
            if (_isDisabled) return;

            var designEvent = new GADesignEvent(eventName, eventValue);
            
            if (!_isInitialized) {
                VoodooLog.Log(TAG, "GameAnalytics NOT initialized queuing event..." + eventName);
                QueuedEvents.Enqueue(designEvent);
            } else {
                VoodooLog.Log(TAG, "Sending event \"" + eventName + "\" to GameAnalytics");
                designEvent.Track();
            }
        }*/

        #endregion

        

    }
}