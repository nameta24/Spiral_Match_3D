using Facebook.Unity;
using UnityEngine;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public class FacebookTrackerWrapper : IAdTrackerWrapper
    {
        private const string TAG = "FacebookTrackerWrapper";
        private bool _idfaConsent;
        
        public void Init(bool idfaConsent)
        {
            _idfaConsent = idfaConsent;
            
            if (!FB.IsInitialized) 
                FB.Init(InitCallback, OnHideUnity);
            else 
                InitCallback();
        }
        
        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                FB.Mobile.SetAdvertiserTrackingEnabled(_idfaConsent); // iOS only call, do not need to be done on Android
                FB.Mobile.SetAdvertiserIDCollectionEnabled(_idfaConsent);
                FB.Mobile.SetAutoLogAppEventsEnabled(true);
                
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
                Debug.Log("Failed to Initialize the Facebook SDK");
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
    }
}