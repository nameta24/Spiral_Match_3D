using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine;
using System.Collections.Generic;
using Facebook.Unity.Settings;
using UnityEditor;
#endif

namespace Voodoo.Tiny.Sauce.Internal.Analytics.Editor
{
    public class FacebookPreBuild : IPreprocessBuildWithReport
    {
        private const string TAG = "FacebookPreBuild";
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateFacebookSettings(TinySauceSettings.Load());
        }

        public static bool CheckAndUpdateFacebookSettings(TinySauceSettings sauceSettings)
        {
#if UNITY_ANDROID || UNITY_IOS
            if (sauceSettings == null || string.IsNullOrEmpty(sauceSettings.facebookAppId))
            {
                // BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.SettingsNoFacebookAppID);
                return false;
            }
            
            if (string.IsNullOrEmpty(sauceSettings.facebookClientToken))
            {
                // BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoFacebookClientToken);
                return false;
            }
            
            FacebookSettings.AppIds = new List<string> {sauceSettings.facebookAppId};
            FacebookSettings.AppLabels = new List<string> {Application.productName};
            FacebookSettings.ClientTokens = new List<string> {sauceSettings.facebookClientToken};
            EditorUtility.SetDirty(FacebookSettings.Instance);
#endif
            return true;

        }
    }
}