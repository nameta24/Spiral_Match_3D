using UnityEngine;
using com.adjust.sdk;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public class AdjustTrackerWrapper : IAdTrackerWrapper
    {
        private const string TAG = "AdjustTrackerWrapper";
        public void Init(bool idfaConsent)
        {
#if UNITY_IOS
            AdjustConfig adjustConfig = new AdjustConfig(TinySauceBehaviour.Instance.sauceSettings.adjustIOSToken, AdjustEnvironment.Production);
#elif UNITY_ANDROID
            AdjustConfig adjustConfig = new AdjustConfig(TinySauceBehaviour.Instance.sauceSettings.adjustAndroidToken, AdjustEnvironment.Production);
#else
            Debug.LogWarning("Please make sur to select iOS or Android platform on your build settings");
            AdjustConfig adjustConfig = new AdjustConfig("", AdjustEnvironment.Sandbox);
#endif
            Adjust.start(adjustConfig);
        }
    }
}