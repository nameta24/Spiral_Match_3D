using UnityEngine;
using UnityEngine.UI;


namespace Voodoo.Tiny.Sauce.Privacy
{
    [RequireComponent(typeof(Toggle))]
    public class SwitchToggle : MonoBehaviour
    {
        private const string TAG = "SwitchToggle";
        [SerializeField] private Image toggleImg;
        [SerializeField] private Sprite spriteOn;
        [SerializeField] private Sprite spriteOff;

        private Toggle toggle;

        private void Awake()
        {
            if(!toggle)
                toggle = GetComponent<Toggle>();

            toggleImg.sprite = toggle.isOn ? spriteOn : spriteOff; //safety, to check if will disabled by default

            toggle.onValueChanged.AddListener(OnSwitch);
        }

        void OnSwitch(bool isOn)
        {
            toggleImg.sprite = isOn ? spriteOn : spriteOff;
        }

        void OnDestroy()
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }
}