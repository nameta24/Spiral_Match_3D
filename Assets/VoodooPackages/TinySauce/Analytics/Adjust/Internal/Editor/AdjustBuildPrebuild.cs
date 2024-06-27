using System.Collections.Generic;
using Facebook.Unity.Settings;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Voodoo.Sauce.Internal.Editor;
using Voodoo.Tiny.Sauce.Internal;

namespace Voodoo.Tiny.Sauce.Internal.Analytics.Editor
{
    public class AdjustBuildPrebuild : IPreprocessBuildWithReport
    {
        private const string TAG = "AdjustBuildPrebuild";
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateAdjustSettings(TinySauceSettings.Load());
        }

        public static bool CheckAndUpdateAdjustSettings(TinySauceSettings sauceSettings)
        {
#if UNITY_IOS
            if (sauceSettings == null || string.IsNullOrEmpty(sauceSettings.adjustIOSToken.Replace(" ", string.Empty)))
            {
                //                BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoAdjustToken);
                return false;
            }
#endif
#if UNITY_ANDROID
            if (sauceSettings == null ||
                string.IsNullOrEmpty(sauceSettings.adjustAndroidToken.Replace(" ", string.Empty)))
            {
                return false;
            }
            //BuildErrorWindow.LogBuildError(BuildErrorConfig.ErrorID.NoAdjustToken);
#endif
            return true;

        }
    }
}