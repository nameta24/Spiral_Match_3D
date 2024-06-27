using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voodoo.Sauce.Internal;
using Voodoo.Tiny.Sauce.Internal.ABTest;
using Voodoo.Tiny.Sauce.Internal.Analytics;
using Voodoo.Tiny.Sauce.Privacy;

namespace Voodoo.Tiny.Sauce.Internal
{
    internal class TinySauceBehaviour : MonoBehaviour
    {
        private const string TAG = "TinySauceBehaviour";
        public static TinySauceBehaviour Instance { get; private set; }
        public TinySauceSettings sauceSettings { get; private set; }
        private static IABTestManager _abTestManager;
        public static IABTestManager ABTestManager => _abTestManager;
        [HideInInspector] public PrivacyManager privacyManager;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
            
            VoodooLog.Initialize(VoodooLogLevel.WARNING);

            InitABTest();
            LoadSettings();
            InitTracking();

            if (transform != transform.root)
                throw new Exception("TinySauce prefab HAS to be at the ROOT level!");
        }

        private void InitABTest() //All initializations should be done like this. Would be useful for module/sdk addition/removal
        {
            if(GetAbTestingManager().Count == 0) return;
            _abTestManager = (IABTestManager) Activator.CreateInstance(GetAbTestingManager()[0]);
            _abTestManager.Init();
        }

        private static List<Type> GetAbTestingManager()
        {
            Type interfaceType = typeof(IABTestManager);
            List<Type> AbTest = GetTypes(interfaceType);
            return AbTest;
        }

        private void LoadSettings()
        {
            sauceSettings = TinySauceSettings.Load();
            if (sauceSettings == null)
                throw new Exception("Can't find TinySauce sauceSettings file.");
        }

        public async void InitTracking()
        {
            privacyManager = GetComponent<PrivacyManager>();;
            
            await privacyManager.CheckConsents();
            
            if (PrivacyManager.AdConsent)
            {
                // Old__ADTrackingInit();
                AdTrackingInit(PrivacyManager.AdConsent);
            }
            
            if (PrivacyManager.AnalyticsConsent)
            {
                VoodooLog.Log(TAG, "Initializing Analytics");
                AnalyticsManager.TrackApplicationLaunch(); 
                AnalyticsManager.Initialize(sauceSettings, true);
            }
        }

        private void AdTrackingInit(bool consent)
        {
            List<IAdTrackerWrapper> adTrackerWrappers = new List<IAdTrackerWrapper>()
            {
                new AdjustTrackerWrapper(),
                new FacebookTrackerWrapper()
            };
            
            // Initialize providers
            adTrackerWrappers.ForEach(wrapper => wrapper.Init(consent));

        }
        
        private void Old__AdTrackingInit() // Issues with some project, GetTypes does not return the correct Wrappers
        {
            var wrapperTypes = GetTypes(typeof(IAdTrackerWrapper));
            foreach (var wrapperType in wrapperTypes)
            {
                var wrapper = (IAdTrackerWrapper)Activator.CreateInstance(wrapperType);
                wrapper.Init(PrivacyManager.AdConsent);
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            // Brought forward after soft closing 
            if (!pauseStatus) {
                AnalyticsManager.OnApplicationResume();
            }
        }
        
        internal static void InvokeCoroutine(IEnumerator coroutine)
        {
            if (Instance == null) return;
            Instance.StartCoroutine(coroutine);
        }

        private static List<Type> GetTypes(Type toGetType)
        {
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => toGetType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();

            types.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

            return types;
        }
    }
}