using System;
using System.Collections.Generic;
using UnityEngine;
using Voodoo.Tiny.Sauce.Internal.Analytics;
using Voodoo.Tiny.Sauce.Privacy;

public static class TinySauce
{
    private const string TAG = "TinySauce";
    public const string Version = "6.6.3";
    private const string ABCohortKey = "ABCohort";
    private const string DebugCohortKey = "DebugCohortKey";

    public static event Action<bool, bool> ConsentGiven;

    #region GameStart Methods

    /// <summary>
    ///  Method to call whenever the user starts a game.
    /// </summary>
    public static void OnGameStarted(int levelNumber, Dictionary<string, object> eventProperties = null)
    {
        OnGameStarted(levelNumber.ToString(), eventProperties);
    }

    /// <summary>
    ///  Method to call whenever the user starts a game.
    /// </summary>
    /// <param name="levelName">The game Level, this parameter is optional for game without level</param>
    public static void OnGameStarted(string levelName, Dictionary<string, object> eventProperties = null)
    {
        AnalyticsManager.TrackGameStarted(levelName, eventProperties);
    }
    
    /// <summary>
    ///  Method to call whenever the user starts a game using the dim1:dim2:dim3 format.
    /// </summary>
    /// <param name="dimension1">The first dimension of the level</param>
    /// <param name="dimension2">The second dimension of the level</param>
    /// <param name="dimension3">The third dimension of the level, this parameter is optional</param>
    public static void OnGameStarted(string dimension1, string dimension2, string dimension3 = null, Dictionary<string, object> eventProperties = null)
    {
        AnalyticsManager.TrackGameStarted(dimension1, dimension2, dimension3, eventProperties);
    }

    #endregion

    #region GameFinish Methods

    /// <summary>
    /// Method to call whenever the user completes a game with levels.
    /// </summary>
    /// <param name="score">The score of the game</param>
    public static void OnGameFinished(float score)
    {
        OnGameFinished(true, score, null, null);
    }

    /// <summary>
    /// Method to call whenever the user finishes a game, even when leaving a game.
    /// </summary>
    /// <param name="levelComplete">Whether the user finished the game</param>
    /// <param name="score">The score of the game</param>
    public static void OnGameFinished(bool levelComplete, float score, Dictionary<string, object> eventProperties = null)
    {
        OnGameFinished(levelComplete, score, null, eventProperties);
    }

    /// <summary>
    /// Method to call whenever the user finishes a game, even when leaving a game.
    /// </summary>
    /// <param name="levelComplete">Whether the user finished the game</param>
    /// <param name="score">The score of the game</param>
    /// <param name="levelNumber">The level number</param>
    public static void OnGameFinished(bool levelComplete, float score, int levelNumber, Dictionary<string, object> eventProperties = null)
    {
        OnGameFinished(levelComplete, score, levelNumber.ToString(), eventProperties);
    }

    /// <summary>
    /// Method to call whenever the user finishes a game, even when leaving a game.
    /// </summary>
    /// <param name="levelComplete">Whether the user finished the game</param>
    /// <param name="score">The score of the game</param>
    /// <param name="levelName">The level name</param>
    public static void OnGameFinished(bool levelComplete, float score, string levelName, Dictionary<string, object> eventProperties = null)
    {
        AnalyticsManager.TrackGameFinished(levelComplete, score, levelName, eventProperties);
    }
    
    /// <summary>
    /// Method to call whenever the user finishes a game, even when leaving a game.
    /// </summary>
    /// <param name="levelComplete">Whether the user finished the game</param>
    /// <param name="score">The score of the game</param>
    /// <param name="dimension1">The first dimension of the level</param>
    /// <param name="dimension2">The second dimension of the level</param>
    /// <param name="dimension3">The third dimension of the level, this parameter is optional</param>
    public static void OnGameFinished(bool levelComplete, float score, string dimension1, string dimension2, string dimension3 = null, Dictionary<string, object> eventProperties = null)
    {
        AnalyticsManager.TrackGameFinished(levelComplete, score, dimension1, dimension2, dimension3, eventProperties);
    }
    

    #endregion

    #region Upgrade Methods

    /// <summary>
    /// Method to call whenever the user makes an upgrade on the game.
    /// </summary>
    /// <param name="upgradeName">The name of the upgrade</param>
    /// <param name="upgradeLevel">The level of the upgrade</param>
    public static void OnUpgradeEvent(string upgradeName, int upgradeLevel, Dictionary<string, object> eventProperties = null,
        string type = null, List<AnalyticsProvider> analyticsProviders = null)
    {
        AnalyticsManager.TrackUpgradeEvent(null, upgradeName, upgradeLevel, eventProperties, type, analyticsProviders);
    }

    /// <summary>
    /// Method to call whenever the user makes an upgrade on the game.
    /// </summary>
    /// <param name="upgradeCategory">The category of the upgrade</param>
    /// <param name="upgradeName">The name of the upgrade</param>
    /// <param name="upgradeLevel">The level of the upgrade</param>
    public static void OnUpgradeEvent(string upgradeCategory, string upgradeName, int upgradeLevel, Dictionary<string, object> eventProperties = null,
        string type = null, List<AnalyticsProvider> analyticsProviders = null)
    {
        AnalyticsManager.TrackUpgradeEvent(upgradeCategory, upgradeName, upgradeLevel, eventProperties, type, analyticsProviders);
    }

    #endregion


    [Obsolete("This method is deprecated, please check the TinySauce Optional Event documentation")]
    public static void DeclareCurrency(string currencyName)
    {
        Debug.LogError("DeclareCurrency method is deprecated, please check the \"TinySauce Optional Event\" documentation");
    }
    
    [Obsolete("This method is deprecated, please check the TinySauce Optional Event documentation")]
    public static void DeclareItemType(string itemTypeName)
    {
        Debug.LogError("DeclareItemType method is deprecated, please check the \"TinySauce Optional Event\" documentation");
    }

    /// <summary>
    /// Track when the player receive some currency
    /// </summary>
    /// <param name="currency">The name of the currency</param>
    /// <param name="currencyAmount">The amount of the currency</param>
    /// <param name="itemType">The name of the item type</param>
    /// <param name="itemId">The id of the item transaction</param>
    public static void OnCurrencyGiven(string currency, int currencyAmount, string itemType, string itemId)
    {
        AnalyticsManager.TrackResourceSourceEvent(currency, currencyAmount, itemType, itemId);
    }
    
    /// <summary>
    /// Track when the player loses some currency
    /// </summary>
    /// <param name="currency">The name of the currency</param>
    /// <param name="currencyAmount">The amount of the currency</param>
    /// <param name="itemType">The name of the item type</param>
    /// <param name="itemId">The id of the item transaction</param>
    public static void OnCurrencyTaken(string currency, int currencyAmount, string itemType, string itemId)
    {
        AnalyticsManager.TrackResourceSinkEvent(currency, currencyAmount, itemType, itemId);
    }
    
    
    /// <summary>
    /// Track any real money transaction in-game.
    /// </summary>
    /// <param name="currency">Currency code in ISO 4217 format. (e.g. USD).</param>
    /// <param name="amount">Amount in cents (int). (e.g. 99).</param>
    /// <param name="itemType">Item Type bought. (e.g. Gold Pack).</param>
    /// <param name="itemId">Item bought. (e.g. 1000 gold).</param>
    /// <param name="cartType">Cart type.</param>
    public static void OnIAPPurchase(string currency, int amount, string itemType, string itemId, string cartType)
    {
        AnalyticsManager.TrackIAPEvent(currency, amount, itemType, itemId, cartType);
    }
    
    #region CustomEvent Method

    /// <summary>
    /// Call this method to track any custom event you want.
    /// </summary>
    /// <param name="eventName">The name of the event to track</param>
    /// <param name="eventProperties">An optional list of properties to send along with the event</param>
    /// <param name="type">type of the event</param>
    /// <param name="analyticsProviders">The list of analytics provider you want to track your custom event to. If this list is null or empty, the event will be tracked in GameAnalytics and Mixpanel (if the user is in a cohort)</param>
    public static void TrackCustomEvent(string eventName, Dictionary<string, object> eventProperties = null,
        string type = null, List<AnalyticsProvider> analyticsProviders = null)
    {
        AnalyticsManager.TrackCustomEvent(eventName, eventProperties, type, analyticsProviders);
    }

    #endregion

    
    [Obsolete("This function will be deprecated soon, please use SubscribeOnInitFinishedEvent instead")]
    public static void SubscribeToConsentGiven(Action<bool, bool> onConsentGiven)
    {
       SubscribeOnInitFinishedEvent(onConsentGiven);
    }
    
    public static void SubscribeOnInitFinishedEvent(Action<bool, bool> onConsentGiven)
    {
        if (!PrivacyManager.ConsentReady)
            PrivacyManager.OnConsentGiven += onConsentGiven;
        else
            onConsentGiven?.Invoke(PrivacyManager.AdConsent, PrivacyManager.AnalyticsConsent);
    }

    [Obsolete("This function will be deprecated soon, please use UnsubscribeOnInitFinishedEvent instead")]
    public static void UnsubscribeToConsentGiven(Action<bool, bool> onConsentGiven)
    {
        UnsubscribeOnInitFinishedEvent(onConsentGiven);
    }    
    
    public static void UnsubscribeOnInitFinishedEvent(Action<bool, bool> onConsentGiven)
    {
        PrivacyManager.OnConsentGiven -= onConsentGiven;
    }

    public static string GetABTestCohort()
    {
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(DebugCohortKey)))
        {
            return PlayerPrefs.GetString(DebugCohortKey);
        }
#endif
        return PlayerPrefs.GetString(ABCohortKey);
    }

    public enum AnalyticsProvider
    {
        VoodooAnalytics,
        GameAnalytics,
    }
}