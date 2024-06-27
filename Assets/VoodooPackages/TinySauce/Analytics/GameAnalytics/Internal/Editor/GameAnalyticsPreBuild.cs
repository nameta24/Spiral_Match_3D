using GameAnalyticsSDK;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Voodoo.Sauce.Internal.Editor;
using Voodoo.Tiny.Sauce.Internal;

namespace Voodoo.Tiny.Sauce.Internal.Analytics.Editor
{
    public class GameAnalyticsPreBuild : IPreprocessBuildWithReport
    {
        private const string TAG = "GameAnalyticsPreBuild";
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateGameAnalyticsSettings(TinySauceSettings.Load());
        }

        public static bool CheckAndUpdateGameAnalyticsSettings(TinySauceSettings settings)
        {
#if UNITY_ANDROID
            if (settings == null || !CheckGameAnalyticsSettings(settings.gameAnalyticsAndroidGameKey, settings.gameAnalyticsAndroidSecretKey,RuntimePlatform.Android)) {
                // BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.GANoAndroidAndKey);
                return false;
            }
#elif UNITY_IOS
            if (settings == null || !CheckGameAnalyticsSettings(settings.gameAnalyticsIosGameKey, settings.gameAnalyticsIosSecretKey, RuntimePlatform.IPhonePlayer))
            {
                // BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.GANoIOSKey);
                return false;
            }
#endif
            return true;
        }

        private static bool CheckGameAnalyticsSettings(string gameKey, string secretKey, RuntimePlatform platform)
        {
            if (string.IsNullOrEmpty(gameKey) || string.IsNullOrEmpty(secretKey))
                return false;

            if (gameKey.ToLower() == "ignore" && secretKey.ToLower() == "ignore")
                return true;

            if (!GameAnalytics.SettingsGA.Platforms.Contains(platform))
                GameAnalytics.SettingsGA.AddPlatform(platform);

            int platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);
            GameAnalytics.SettingsGA.UpdateGameKey(platformIndex, gameKey);
            GameAnalytics.SettingsGA.UpdateSecretKey(platformIndex, secretKey);
            GameAnalytics.SettingsGA.Build[platformIndex] = Application.version;
            GameAnalytics.SettingsGA.InfoLogBuild = false;
            GameAnalytics.SettingsGA.InfoLogEditor = false;
            return true;
        }
    }
}