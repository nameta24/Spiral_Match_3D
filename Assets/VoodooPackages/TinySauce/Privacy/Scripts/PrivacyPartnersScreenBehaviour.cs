using UnityEngine;
using UnityEngine.UI;

namespace Voodoo.Tiny.Sauce.Privacy {
    public class PrivacyPartnersScreenBehaviour : MonoBehaviour
    {
        private const string TAG = "PrivacyPartnersScreenBehaviour";
        private const string adjustPrivacyLink = "https://www.adjust.com/terms/privacy-policy/";
        private const string facebookPrivacyLink = "https://www.facebook.com/privacy/policy/";
        private const string gameAnalyticsPrivacyLink = "https://gameanalytics.com/privacy/";
        private const string voodooPrivacyLink = "https://www.voodoo.io/privacy";

        [SerializeField] private Button AdjustLearnMoreButton;
        [SerializeField] private Button FacebookLearnMoreButton;
        [SerializeField] private Button GameAnalyticsLearnMoreButton;
        [SerializeField] private Button VoodooLearnMoreButton;
        [SerializeField] private Button CloseButton;

        private void Start()
        {
            AdjustLearnMoreButton.onClick.AddListener(OnPressAdjustLearnMore);
            FacebookLearnMoreButton.onClick.AddListener(OnPressFacebookLearnMore);
            GameAnalyticsLearnMoreButton.onClick.AddListener(OnPressGameAnalyticsLearnMore);
            VoodooLearnMoreButton.onClick.AddListener(OnPressVoodooLearnMore);
            CloseButton.onClick.AddListener(OnPressClose);
        }

        private void OnPressAdjustLearnMore()
        {
            Application.OpenURL(adjustPrivacyLink);
        }

        private void OnPressFacebookLearnMore()
        {
            Application.OpenURL(facebookPrivacyLink);
        }

        private void OnPressGameAnalyticsLearnMore()
        {
            Application.OpenURL(gameAnalyticsPrivacyLink);
        }

        private void OnPressVoodooLearnMore()
        {
            Application.OpenURL(voodooPrivacyLink);
        }

        private void OnPressClose()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            AdjustLearnMoreButton.onClick.RemoveAllListeners();
            FacebookLearnMoreButton.onClick.RemoveAllListeners();
            GameAnalyticsLearnMoreButton.onClick.RemoveAllListeners();
            VoodooLearnMoreButton.onClick.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();
        }
    }
}
