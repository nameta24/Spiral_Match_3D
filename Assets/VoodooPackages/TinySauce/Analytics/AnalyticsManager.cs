using System;
using System.Collections.Generic;
using GameAnalyticsSDK;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    internal static class AnalyticsManager
    {
        private const string TAG = "AnalyticsManager";
        private const string NO_GAME_LEVEL = "game";

        internal static bool HasGameStarted { get; private set; }
        private static readonly AnalyticsEventTimer _gameTimer = new AnalyticsEventTimer();

        // Voodoo sauce additional events
        
        #region Progression Events Declaration
        internal static event Action<int, bool> OnGamePlayed;
        internal static event Action<GameStartedParameters> OnGameStartedEvent;
        internal static event Action<GameFinishedParameters> OnGameFinishedEvent;
        
        #endregion
        
        #region Resource Event Declaration
        
        internal static event Action<string> OnDeclareResourceCurrency;
        internal static event Action<string> OnDeclareResourceItemType;
        internal static event Action<ResourceEventParameters> OnResourceSinkEvent;
        internal static event Action<ResourceEventParameters> OnResourceSourceEvent;
        
        #endregion
        
        
        internal static event Action<BusinessEventParameters> OnBusinessEvent;

        #region Metrics Event Declaration

        
        private const float PerfMetricsPeriod = 30f;
        internal static event Action<PerformanceMetricsAnalyticsInfo> OnTrackPerformanceMetricsEvent;
        

        #endregion

        #region Application Launch Events Declaration

        internal static event Action OnApplicationFirstLaunchEvent;
        internal static event Action OnApplicationLaunchEvent;
        

        #endregion

        #region Interstitial Events Declaration
        
        internal static event Action<AdShownEventAnalyticsInfo> OnInterstitialShowEvent;
        internal static event Action<AdClickEventAnalyticsInfo> OnInterstitialClickedEvent;

        #endregion
        
        #region Rewarded Events Declaration
        
        internal static event Action<AdShownEventAnalyticsInfo> OnRewardedShowEvent;
        internal static event Action<AdClickEventAnalyticsInfo> OnRewardedClickedEvent;

        #endregion

        #region Custom Event Declaration
        
        internal static event Action<string, Dictionary<string, object>, string, List<TinySauce.AnalyticsProvider>> OnTrackCustomEvent;

        #endregion

        #region Unusued Event Declaration, TO CLEAN
        internal static event Action OnApplicationResumeEvent;
        
        #endregion

        
        private static readonly List<TinySauce.AnalyticsProvider> DefaultAnalyticsProvider = new List<TinySauce.AnalyticsProvider>
            {TinySauce.AnalyticsProvider.GameAnalytics};

        private static List<IAnalyticsProvider> _analyticsProviders;

        internal static void Initialize(TinySauceSettings sauceSettings, bool consent)
        {
            _analyticsProviders = new List<IAnalyticsProvider>()
            {
                new GameAnalyticsProvider(), 
                new VoodooAnalyticsProvider(new VoodooAnalyticsParameters(false, true, ""))
            };
            
            // Initialize providers
            _analyticsProviders.ForEach(provider => provider.Initialize(consent));
            
            PerformanceMetricsManager.Initialize(PerfMetricsPeriod);
        }

        internal static void OnApplicationResume()
        {
            OnApplicationResumeEvent?.Invoke();
        }

        #region Track Game Events

        internal static void TrackApplicationLaunch()
        {
            AnalyticsStorageHelper.IncrementAppLaunchCount();
            //fire app launch events
            if (AnalyticsStorageHelper.IsFirstAppLaunch())
            {
                OnApplicationFirstLaunchEvent?.Invoke();
            }

            OnApplicationLaunchEvent?.Invoke();
        }
        
        #region Track Progression Events
        
        internal static void TrackGameStarted(string level, Dictionary<string, object> eventProperties = null)
        {
            HasGameStarted = true;
            _gameTimer.Start();
            AnalyticsStorageHelper.IncrementGameCount();
            GameStartedParameters gameStartedParameters = new GameStartedParameters
            {
                levelDimension1 = level ?? NO_GAME_LEVEL,
                eventProperties = eventProperties,
            };
            OnGameStartedEvent?.Invoke(gameStartedParameters);
        }
        
        internal static void TrackGameStarted(string dimension1 = null, string dimension2 = null, string dimension3 = null, Dictionary<string, object> eventProperties = null)
        {
            HasGameStarted = true;
            _gameTimer.Start();
            AnalyticsStorageHelper.IncrementGameCount();
            GameStartedParameters gameStartedParameters = new GameStartedParameters
            {
                levelDimension1 = dimension1 ?? NO_GAME_LEVEL,
                levelDimension2 = dimension2,
                levelDimension3 = dimension3,
                eventProperties = eventProperties,
            };
            OnGameStartedEvent?.Invoke(gameStartedParameters);
        }

        internal static void TrackGameFinished(bool levelComplete, float score, string level, Dictionary<string, object> eventProperties)
        {
            HasGameStarted = false;
            _gameTimer.Stop();
            AnalyticsStorageHelper.UpdateLevel(level);
            if (levelComplete)
            {
                // used to calculate the win rate (for RemoteConfig)
                AnalyticsStorageHelper.IncrementSuccessfulGameCount();
            }

            GameFinishedParameters gameFinishedParameters = new GameFinishedParameters
            {
                levelDimension1 = level ?? NO_GAME_LEVEL,
                status = levelComplete,
                score = score,
                eventProperties = eventProperties,
            };
            gameFinishedParameters.gameDuration = _gameTimer.GetDuration();
            OnGamePlayed?.Invoke(AnalyticsStorageHelper.GetGameCount(), AnalyticsStorageHelper.UpdateGameHighestScore(score));
            OnGameFinishedEvent?.Invoke(gameFinishedParameters);
        }
        internal static void TrackGameFinished(bool levelComplete, float score, string dimension1 = null, string dimension2 = null, string dimension3 = null, Dictionary<string, object> eventProperties = null)
        {
            HasGameStarted = false;
            _gameTimer.Stop();
            AnalyticsStorageHelper.UpdateLevel(dimension1);
            if (levelComplete)
            {
                // used to calculate the win rate (for RemoteConfig)
                AnalyticsStorageHelper.IncrementSuccessfulGameCount();
            }

            GameFinishedParameters gameFinishedParameters = new GameFinishedParameters
            {
                levelDimension1 = dimension1 ?? NO_GAME_LEVEL,
                levelDimension2 = dimension2,
                levelDimension3 = dimension3,
                status = levelComplete,
                score = score,
                eventProperties = eventProperties,
            };
            gameFinishedParameters.gameDuration = _gameTimer.GetDuration();
            OnGamePlayed?.Invoke(AnalyticsStorageHelper.GetGameCount(), AnalyticsStorageHelper.UpdateGameHighestScore(score));
            OnGameFinishedEvent?.Invoke(gameFinishedParameters);
        }
        #endregion

        #region Upgrade Event
        
        internal static void TrackUpgradeEvent( string upgradeCategory, string upgradeName, int upgradeLevel,
                                                Dictionary<string, object> eventProperties,
                                                string type = null,
                                                List<TinySauce.AnalyticsProvider> analyticsProviders = null)
        {
            if (analyticsProviders == null || analyticsProviders.Count == 0)
            {
                analyticsProviders = DefaultAnalyticsProvider;
            }

            string eventName = "Upgrade:" + (upgradeCategory != null ? upgradeCategory + ":" + upgradeName : upgradeName) + ":" + upgradeLevel;

            OnTrackCustomEvent?.Invoke(eventName, eventProperties, type, analyticsProviders);
        }
        

        #endregion

        #region Resource Event

        internal static void DeclareItemType(string itemType)
        {
            //OnDeclareResourceItemType?.Invoke(itemType);
            GameAnalytics.SettingsGA.ResourceItemTypes.Add(itemType);
        }       
        internal static void DeclareCurrencyType(string currency)
        {
            //OnDeclareResourceCurrency?.Invoke(currency);
            GameAnalytics.SettingsGA.ResourceCurrencies.Add(currency);
        }

        internal static void TrackResourceSinkEvent(string currency, int amount, string itemType, string itemName)
        {
            ResourceEventParameters resourceEventParameters = new ResourceEventParameters
            {
                currency = currency,
                amount = amount,
                itemType = itemType,
                itemName = itemName
            };
            OnResourceSinkEvent?.Invoke(resourceEventParameters);
        }
        internal static void TrackResourceSourceEvent(string currency, int amount, string itemType, string itemName)
        {
            ResourceEventParameters resourceEventParameters = new ResourceEventParameters
            {
                currency = currency,
                amount = amount,
                itemType = itemType,
                itemName = itemName
            };
            OnResourceSourceEvent?.Invoke(resourceEventParameters);
        }
        
        internal static void TrackIAPEvent(string currency, int amount, string itemType, string itemId, string cartType)
        {
            BusinessEventParameters businessEventParameters = new BusinessEventParameters
            {
                currency = currency,
                amount = amount,
                itemType = itemType,
                itemId = itemId,
                cartType = cartType
            };
            OnBusinessEvent?.Invoke(businessEventParameters);
        }

        #endregion

        #region Track Custom Event
        
        internal static void TrackCustomEvent(string eventName,
                                              Dictionary<string, object> eventProperties,
                                              string type = null,
                                              List<TinySauce.AnalyticsProvider> analyticsProviders = null)
        {
            if (analyticsProviders == null || analyticsProviders.Count == 0)
            {
                analyticsProviders = DefaultAnalyticsProvider;
            }

            OnTrackCustomEvent?.Invoke(eventName, eventProperties, type, analyticsProviders);
        }
        
        #endregion
        
        #region Track Metrics Event

        internal static void TrackPerformanceMetrics(PerformanceMetricsAnalyticsInfo info)
        {
            OnTrackPerformanceMetricsEvent?.Invoke(info);
        }
        #endregion
        
        #region Track Interstitial Event
        
        /*
        *
        * AnalyticsManager.TrackInterstitialShow(new AdShownEventAnalyticsInfo {
              AdTag = tag,
              AdNetworkName = MediationAdapter.GetRewardedVideoAdNetworkName(),
              AdLoadingTime = (int) MediationAdapter.GetRewardedVideoLoadingTime().TotalMilliseconds,
              AdCount = AnalyticsStorageHelper.GetShowRewardedVideoCount()
          });
        */
    
        internal static void TrackInterstitialShow(AdShownEventAnalyticsInfo adAnalyticsInfo)
        {
            adAnalyticsInfo.GameCount = AnalyticsStorageHelper.GetGameCount();
            OnInterstitialShowEvent?.Invoke(adAnalyticsInfo);
        }
  
        
        internal static void TrackInterstitialClick(AdClickEventAnalyticsInfo adAnalyticsInfo)
        {
            adAnalyticsInfo.GameCount = AnalyticsStorageHelper.GetGameCount();
            OnInterstitialClickedEvent?.Invoke(adAnalyticsInfo);
        }

        #endregion
        
        #region Track Rewarded Event
        
        internal static void TrackRewardedShow(AdShownEventAnalyticsInfo adAnalyticsInfo)
        {
            adAnalyticsInfo.GameCount = AnalyticsStorageHelper.GetGameCount();
            OnRewardedShowEvent?.Invoke(adAnalyticsInfo);
        }
        
        internal static void TrackRewardedClick(AdClickEventAnalyticsInfo adAnalyticsInfo)
        {
            adAnalyticsInfo.GameCount = AnalyticsStorageHelper.GetGameCount();
            OnRewardedClickedEvent?.Invoke(adAnalyticsInfo);
        }

        #endregion
        
        #endregion
    }
}