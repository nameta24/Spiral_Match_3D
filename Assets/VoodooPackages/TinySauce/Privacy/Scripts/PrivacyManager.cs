using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_IOS
using Voodoo.Sauce.Internal.IdfaAuthorization;
#endif

namespace Voodoo.Tiny.Sauce.Privacy
{
    public class PrivacyManager : MonoBehaviour
    {
        private const string TAG = "PrivacyManager";
        [SerializeField] private PrivacyScreenBehaviour privacyScreenPrefab;
        [SerializeField] private PrivacyPartnersScreenBehaviour privacyPartnersScreenPrefab;
        
        private const string ConsentUrl = "https://api-gdpr.voodoo-tech.io/need_consent?bundle_id=&popup_version=&os_type=&locale=&app_version=&uuid=0";
        
        public static bool AdConsent { private set; get; }
        public static bool AnalyticsConsent { private set; get; }
        public bool IdfaConsent { private set; get; }

        private const string FirstLaunchKey = "FirstStart";
        private const string AdConsentKey = "AdConsent";
        private const string AnalyticsConsentKey = "AnalyticsConsent";
        
        public static event Action<bool, bool> OnConsentGiven;
        public static bool ConsentReady;
        
        public async Task CheckConsents()
        {
            if (!PlayerPrefs.HasKey(FirstLaunchKey))
            {
                var needGdprConsent = await CheckGdprApiForRequirement();
                
                if (needGdprConsent)
                {
                    await OpenPrivacyScreen();
                }
                else
                {
                    SaveConsents(true, true);
                }
                PlayerPrefs.SetInt(FirstLaunchKey, 0);
                PlayerPrefs.Save();
            }
            else
            {
                AdConsent = PlayerPrefs.GetInt(AdConsentKey) != 0;
                AnalyticsConsent = PlayerPrefs.GetInt(AnalyticsConsentKey) != 0;
            }

            #if UNITY_IOS && !UNITY_EDITOR

            if (AdConsent || AnalyticsConsent)
            {
                var iOsIdfaWaitTask = new TaskCompletionSource<bool>();
                NativeWrapper.RequestAuthorization((status) =>
                {
                    iOsIdfaWaitTask?.TrySetResult(status == IdfaAuthorizationStatus.Authorized);
                });
                IdfaConsent = await iOsIdfaWaitTask.Task;
            }
            
            #else
            
            IdfaConsent = true;
            
            #endif
            
            ConsentReady = true;
            OnConsentGiven?.Invoke(AdConsent, AnalyticsConsent);
        }

        private async Task<bool> CheckGdprApiForRequirement()
        {
            var request = UnityWebRequest.Get(ConsentUrl);
            request.SendWebRequest();

            while (!request.isDone)
            {
                await Task.Yield();
            }

#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.Log(request.error);
                return false;
            }
            
            var consentInfo = JsonUtility.FromJson<ConsentInfo>(request.downloadHandler.text);
            return consentInfo.is_gdpr;
        }

        public async Task OpenPrivacyScreen()
        {
            var privacyScreen = Instantiate(privacyScreenPrefab).GetComponent<PrivacyScreenBehaviour>();
            DontDestroyOnLoad(privacyScreen.gameObject);
            privacyScreen.ConfirmWaitTask = new TaskCompletionSource<bool[]>();
            await privacyScreen.ConfirmWaitTask.Task;
            var result = privacyScreen.ConfirmWaitTask.Task.Result;
            SaveConsents(result[0], result[1]);
        }

        private void SaveConsents(bool newAdConsent, bool newAnalyticsConsent)
        {
            AdConsent = newAdConsent;
            AnalyticsConsent = newAnalyticsConsent;
            PlayerPrefs.SetInt(AdConsentKey, AdConsent ? 1 : 0);
            PlayerPrefs.SetInt(AnalyticsConsentKey, AnalyticsConsent ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        public void OpenPrivacyPartnersScreen()
        {
            Instantiate(privacyPartnersScreenPrefab);
        }
    }
}
