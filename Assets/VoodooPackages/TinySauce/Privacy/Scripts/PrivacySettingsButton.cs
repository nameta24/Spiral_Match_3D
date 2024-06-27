using UnityEngine;
using UnityEngine.UI;
using Voodoo.Tiny.Sauce.Internal;

namespace Voodoo.Tiny.Sauce.Privacy
{
    public class PrivacySettingsButton : MonoBehaviour
    {
        private const string TAG = "PrivacySettingsButton";
        [SerializeField] private Button gdprButton;

        private void Start()
        { 
            gdprButton.onClick.AddListener(() => TinySauceBehaviour.Instance.privacyManager.OpenPrivacyScreen());
            // (1) privacy screen should show current choices when open
            // (2) only if choices are different we should init / terminate sdks
        }
    }
}
