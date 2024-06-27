using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Voodoo.Tiny.Sauce.Internal;

namespace Voodoo.Tiny.Sauce.Privacy
{
    public class PrivacyScreenBehaviour : MonoBehaviour
    {
        private const string TAG = "PrivacyScreenBehaviour";
        [SerializeField] private Toggle advertisingToggle;
        [SerializeField] private Toggle analyticsToggle;
        [SerializeField] private Toggle ageToggle;
        
        [SerializeField] private Button playButton;
        [SerializeField] private Button privacyPolicyButton;
        [SerializeField] private Button learnMoreButton;
        
        private TinySauceSettings _sauceSettings;
        
        [SerializeField] private GameObject mainUIObject;
        
        private EventSystem _eventSystemPrefab;
        private EventSystem _eventSystem;
        
        public TaskCompletionSource<bool[]> ConfirmWaitTask;
        
        private void Start()
        {
            ageToggle.onValueChanged.AddListener(OnToggleAge);

            learnMoreButton.onClick.AddListener(OnPressLearnMore);

            playButton.onClick.AddListener(OnPressPlay);
            privacyPolicyButton.onClick.AddListener(OnPressPrivacyPolicy);
            
            _sauceSettings = TinySauceSettings.Load();
            if (_sauceSettings == null)
            {
                throw new Exception("Can't find the Settings ScriptableObject in the Resources/TinySauce folder.");
            }
            
            InitEventSystem();
        }

        private void OnToggleAge(bool consent)
        {
            playButton.interactable = consent;
        }

        private void OnPressLearnMore()
        {
            TinySauceBehaviour.Instance.privacyManager.OpenPrivacyPartnersScreen();
        }

        private void OnPressPlay()
        {
            mainUIObject.SetActive(false);
            
            ConfirmWaitTask?.TrySetResult(new []{advertisingToggle.isOn, analyticsToggle.isOn});
            
            RemoveListeners();
            
            if (_eventSystem != null)
                Destroy(_eventSystem.gameObject);
            
            Destroy(gameObject);
        }

        private void OnPressPrivacyPolicy()
        {
            if (_sauceSettings.privacyPolicyURL != "")
            {
                Application.OpenURL(_sauceSettings.privacyPolicyURL);
            }
        }

        private void RemoveListeners()
        {
            ageToggle.onValueChanged.RemoveAllListeners();
            playButton.onClick.RemoveAllListeners();
            privacyPolicyButton.onClick.RemoveAllListeners();
        }
        
        private void InitEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            
            if (_eventSystemPrefab == null)
                _eventSystemPrefab = Resources.LoadAll<EventSystem>("Prefabs")[0];
                    
            if (_eventSystemPrefab == null)
                Debug.LogError("There is no TSEventSystem prefab in the 'Assets/VoodooPackages/TinySauce/Resources/Prefabs' folder");

            _eventSystem = Instantiate(_eventSystemPrefab);
        }
    }
}
