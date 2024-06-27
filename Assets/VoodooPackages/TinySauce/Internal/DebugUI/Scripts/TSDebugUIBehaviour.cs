using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Voodoo.Tiny.Sauce.Internal
{
    public class TSDebugUIBehaviour : MonoBehaviour
    {
        private const string TAG = "TSDebugUIBehaviour";
        public enum TSDebugUIActiveScreen
        {
            Info,
            Events,
            ABTest
        }

        [Header("== Tabs ==")]
        [SerializeField] private string infoScreenName = "INFO";
        [SerializeField] private string eventsScreenName = "EVENTS";
        [SerializeField] private string abtestScreenName = "AB TEST";
        [Space(4)]
        [SerializeField] private Button infoTabBtn;
        [SerializeField] private Button eventsTabBtn;
        [SerializeField] private Button abtestTabBtn;

        [Header("== Screens ==")]
        [SerializeField] private TSDebugUIScreen infoScreen;
        [SerializeField] private TSDebugUIScreen eventsScreen;
        [SerializeField] private TSDebugUIScreen abtestScreen;

        [Header("== App Info Fields ==")]
        [SerializeField] private Text unityVerion;
        [SerializeField] private Text tsVersion;
        [SerializeField] private Text appNameTop;


        private static TSDebugUIBehaviour _instance;
        public static TSDebugUIBehaviour Instance { get => _instance; }

        
        private EventSystem _eventSystemPrefab;
        private EventSystem _eventSystem;
        
        private TinySauceSettings _tsSettings;

        private TSDebugUIActiveScreen activeScreen = TSDebugUIActiveScreen.Info;
        private Dictionary<TSDebugUIActiveScreen, Button> tabDictionary = new Dictionary<TSDebugUIActiveScreen, Button>();
        private Dictionary<TSDebugUIActiveScreen, TSDebugUIScreen> screenDictionary = new Dictionary<TSDebugUIActiveScreen, TSDebugUIScreen>();


        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            //if (FindObjectOfType<EventSystem>() == null) Instantiate(new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)));
            InitEventSystem();

            _tsSettings = TinySauceSettings.Load();
            UpdateInfo();
           
            abtestTabBtn.gameObject.SetActive(TinySauceBehaviour.ABTestManager != null);
            

            infoScreen.TSSettings = _tsSettings;
        }

        private void Start()
        {
            InitDictionaries();
            Set_ActiveScreen(infoScreenName);
        }

        private void OnDestroy()
        {
            _instance = null;
        }


        private  void InitEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            
            if (_eventSystemPrefab == null)
                _eventSystemPrefab = Resources.LoadAll<EventSystem>("Prefabs")[0];
                    
            if (_eventSystemPrefab == null)
                Debug.LogError("There is no TSEventSystem prefab in the 'Assets/VoodooPackages/TinySauce/Resources/Prefabs' folder");

            _eventSystem = Instantiate(_eventSystemPrefab);
        }
        
        public void CloseDebugUI()
        {
            if (_eventSystem != null)
                Destroy(_eventSystem.gameObject);
            
            Destroy(gameObject);
        }

        private void UpdateInfo()
        {
            unityVerion.text = Application.unityVersion;
            tsVersion.text = "TS v. " + TinySauce.Version;
            appNameTop.text = Application.productName;
        }

        #region [TABS]
        private void InitDictionaries()
        {
            tabDictionary[TSDebugUIActiveScreen.Info] = infoTabBtn;
            tabDictionary[TSDebugUIActiveScreen.Events] = eventsTabBtn;
            tabDictionary[TSDebugUIActiveScreen.ABTest] = abtestTabBtn;

            screenDictionary[TSDebugUIActiveScreen.Info] = infoScreen;
            screenDictionary[TSDebugUIActiveScreen.Events] = eventsScreen;
            screenDictionary[TSDebugUIActiveScreen.ABTest] = abtestScreen;

            infoScreen.gameObject.SetActive(false);
            eventsScreen.gameObject.SetActive(false);
            abtestScreen.gameObject.SetActive(false);

            if (TinySauceBehaviour.ABTestManager == null || TinySauceBehaviour.ABTestManager.GetAbTestValues().Length == 0)
            {
                abtestTabBtn.interactable = false;
                abtestTabBtn.image.color = new Color(1, 0.75f, 0.75f);
            }
        }

        private void ToggleTab(bool isActive)
        {
            tabDictionary[activeScreen].interactable = !isActive;
            screenDictionary[activeScreen].gameObject.SetActive(isActive);
        }

        public void Set_ActiveScreen(string screenName)
        {
            TSDebugUIActiveScreen newActiveScreen;

            if (screenName == infoScreenName)
                newActiveScreen = TSDebugUIActiveScreen.Info;
            else if (screenName == eventsScreenName)
                newActiveScreen = TSDebugUIActiveScreen.Events;
            else if (screenName == abtestScreenName)
                newActiveScreen = TSDebugUIActiveScreen.ABTest;
            else
            {
                Debug.LogError("Screen name is not existing");
                return;
            }


            ToggleTab(false);
            activeScreen = newActiveScreen;
            ToggleTab(true);
        }
        #endregion []
    }
}