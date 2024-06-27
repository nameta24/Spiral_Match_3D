using System;
using UnityEditor;
using UnityEngine;
using Voodoo.Tiny.Sauce.Internal.Analytics.Editor;
using Voodoo.Sauce.Internal.Editor;
using Voodoo.Tiny.Sauce.Privacy;

namespace Voodoo.Tiny.Sauce.Internal.Editor
{
    [CustomEditor(typeof(TinySauceSettings))]
    public class TinySauceSettingsEditor : UnityEditor.Editor
    {
        private const string TAG = "TinySauceSettingsEditor";
        private const string EditorPrefEditorIDFA = "EditorIDFA";
        private TinySauceSettings SauceSettings => target as TinySauceSettings;

        private static bool isGASettingsFilled = false;
        private static bool isFBSettingsFilled = false;
        private static bool isAdjustFilled = false;
        private static bool isPrivacySettingsFilled = false;
        private static bool isPrivacyURLValid = false;
        private static bool isPrivacyEmailValid = false;
        private bool isAllFilled = false;
        
        
        

        [MenuItem("TinySauce/TinySauce Settings/Edit Settings", false, 100)]
        
        private static void EditSettings()
        {
            Selection.activeObject = CreateTinySauceSettings();
        }

        private static TinySauceSettings CreateTinySauceSettings()
        {
            TinySauceSettings settings = TinySauceSettings.Load();
            if (settings == null) {
                settings = CreateInstance<TinySauceSettings>();
                //create tinySauce folders if it not exists
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");

                if (!AssetDatabase.IsValidFolder("Assets/Resources/TinySauce"))
                    AssetDatabase.CreateFolder("Assets/Resources", "TinySauce");
                //create TinySauceSettings file
                AssetDatabase.CreateAsset(settings, "Assets/Resources/TinySauce/Settings.asset");
                settings = TinySauceSettings.Load();
            }

            return settings;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(15);

            GUIStyle style = new GUIStyle(GUI.skin.button);
            GUI.backgroundColor = isAllFilled ? Color.green : Color.red;
            string buttonText = isAllFilled ? "All Good!" : "FILL THE SETTINGS AND PRESS";
            buttonText = Environment.NewLine + buttonText + Environment.NewLine;
            if (!isGASettingsFilled) buttonText += Environment.NewLine + "You need to fill GA Settings" + Environment.NewLine;
            if (!isFBSettingsFilled) buttonText += Environment.NewLine + "You need to fill Facebook Settings" + Environment.NewLine;
            if (!isAdjustFilled) buttonText += Environment.NewLine + "You need to fill Adjust Settings" + Environment.NewLine;
            if (!isPrivacySettingsFilled) buttonText += Environment.NewLine + "You need to fill Privacy Settings" + Environment.NewLine;
            if (!isPrivacyURLValid) buttonText += Environment.NewLine + "Make sure you entered a valid URL (with http:// or https://)" + Environment.NewLine;
            if (!isPrivacyEmailValid) buttonText += Environment.NewLine + "Make sure you entered a valid Developer contact email" + Environment.NewLine;

#if UNITY_IOS || UNITY_ANDROID      
            if (GUILayout.Button(buttonText, style)) {
                TrimAllFields(SauceSettings);
                isAllFilled = CheckAndUpdateSdkSettings(SauceSettings);
            }
#else
            EditorGUILayout.HelpBox(BuildErrorConfig.ErrorMessageDict[BuildErrorConfig.ErrorID.INVALID_PLATFORM], MessageType.Error);   
#endif

            string editorIdfa = EditorPrefs.GetString(EditorPrefEditorIDFA);
            if (string.IsNullOrEmpty(editorIdfa))
            {
                editorIdfa = Guid.NewGuid().ToString();
                EditorPrefs.SetString(EditorPrefEditorIDFA, editorIdfa);
            }

            SauceSettings.EditorIdfa = editorIdfa;
            
            GUI.backgroundColor = Color.yellow;
            EditorGUILayout.HelpBox("Please note that events won’t be sent when you run your game in Unity Editor or simulator.", MessageType.Warning);
        }

        private static void TrimAllFields(TinySauceSettings sauceSettings)
        {
            if (sauceSettings == null) return;
            sauceSettings.gameAnalyticsAndroidGameKey = sauceSettings.gameAnalyticsAndroidGameKey.Trim();
            sauceSettings.gameAnalyticsAndroidSecretKey = sauceSettings.gameAnalyticsAndroidSecretKey.Trim();
            sauceSettings.gameAnalyticsIosGameKey = sauceSettings.gameAnalyticsIosGameKey.Trim();
            sauceSettings.gameAnalyticsIosSecretKey = sauceSettings.gameAnalyticsIosSecretKey.Trim();
            
            sauceSettings.facebookAppId = sauceSettings.facebookAppId.Trim();
            sauceSettings.facebookClientToken = sauceSettings.facebookClientToken.Trim();

            sauceSettings.adjustAndroidToken = sauceSettings.adjustAndroidToken.Trim();
            sauceSettings.adjustIOSToken = sauceSettings.adjustIOSToken.Trim();

            sauceSettings.companyName = sauceSettings.companyName.Trim();
            sauceSettings.privacyPolicyURL = sauceSettings.privacyPolicyURL.Trim();
            sauceSettings.developerContactEmail = sauceSettings.developerContactEmail.Trim();
        }

        private static bool CheckAndUpdateSdkSettings(TinySauceSettings sauceSettings)
        {
            Console.Clear();
            isGASettingsFilled = GameAnalyticsPreBuild.CheckAndUpdateGameAnalyticsSettings(sauceSettings);
            isFBSettingsFilled = FacebookPreBuild.CheckAndUpdateFacebookSettings(sauceSettings);
            isAdjustFilled = AdjustBuildPrebuild.CheckAndUpdateAdjustSettings(sauceSettings);
            isPrivacySettingsFilled = PrivacyPrebuild.CheckAndUpdatePrivacySettings(sauceSettings);
            isPrivacyURLValid = PrivacyPrebuild.CheckPrivacyURLValidity(sauceSettings);
            isPrivacyEmailValid = PrivacyPrebuild.CheckPrivacyEmailValidity(sauceSettings);

            return isGASettingsFilled && isFBSettingsFilled && isAdjustFilled && isPrivacySettingsFilled && isPrivacyURLValid && isPrivacyEmailValid;
        }
    }
}