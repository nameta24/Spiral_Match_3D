using System.Collections.Generic;
using System.IO;
using System.Xml;
using Facebook.Unity.Editor;
using GooglePlayServices;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Voodoo.Sauce.Internal.Editor
{
    public class AndroidPrebuild : IPreprocessBuildWithReport
    {
        private const string TAG = "AndroidPrebuild";
        private const string SourceFolderPath = "VoodooPackages/TinySauce/Internal/Android/Editor";
        private static readonly string SourceManifestPath = $"{SourceFolderPath}/AndroidManifest.xml";
        private static readonly string SourceGradlePath = $"{SourceFolderPath}/mainTemplate.gradle";
        private static readonly string SourceLauncherManifestPath = $"{SourceFolderPath}/LauncherManifest.xml";
        private static readonly string SourceLauncherGradlePath = $"{SourceFolderPath}/launcherTemplate.gradle";

        private const string PluginFolderPath = "Plugins";
        private const string AndroidFolderPath = "Plugins/Android";

        private static readonly string DestManifestPath = $"{AndroidFolderPath}/AndroidManifest.xml";
        private static readonly string DestGradlePath = $"{AndroidFolderPath}/mainTemplate.gradle";
        private static readonly string DestLauncherManifestPath = $"{AndroidFolderPath}/LauncherManifest.xml";
        private static readonly string DestLauncherGradlePath = $"{AndroidFolderPath}/launcherTemplate.gradle";

        private static readonly string AndroidGradleVersion = "3.4.3";
        private static readonly string AndroidBuildToolsGradleClasspath = $"classpath 'com.android.tools.build:gradle:{AndroidGradleVersion}'";

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android) {
                return;
            }

            CreateAndroidFolder();
            UpdateManifest();
            UpdateLauncherManifest();
            UpdateGradle();
            UpdateLauncherGradle();
            PreparePlayerSettings();
            PrepareResolver();
        }

        private static void PreparePlayerSettings()
        {
            // Set Android ARM64/ARMv7 Architecture   
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            // Set Android min version
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel22) {
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
            }
        }

        private static void PrepareResolver()
        {
            // Force playServices Resolver
            PlayServicesResolver.Resolve(null, true);
        }

        private static void CreateAndroidFolder()
        {
            string pluginPath = Path.Combine(Application.dataPath, PluginFolderPath);
            string androidPath = Path.Combine(Application.dataPath, AndroidFolderPath);
            if (!Directory.Exists(pluginPath))
                Directory.CreateDirectory(pluginPath);
            if (!Directory.Exists(androidPath))
                Directory.CreateDirectory(androidPath);
        }

        private static void UpdateManifest()
        {
            string sourcePath = Path.Combine(Application.dataPath, SourceManifestPath);
            string content = File.ReadAllText(sourcePath)
                                 .Replace("attribute='**APPLICATION_ATTRIBUTES**'", string.Empty)
                                 .Replace("**APPLICATION_ATTRIBUTES_REPLACE**", string.Empty);
            string destPath = Path.Combine(Application.dataPath, DestManifestPath);
            
            var sourcePermissionsList = ExtractPermissions(content);
            if (File.Exists(destPath))
            {
                string destContent = File.ReadAllText(destPath);
                var destPermissionsList = ExtractPermissions(destContent);

                var permissionsToAdd = new List<XmlNode>();
                foreach (XmlNode destPermission in destPermissionsList)
                    if (destPermission.Attributes != null && destPermission.Attributes.Count > 0 &&
                        destPermission.Attributes[0].Name == "android:name")
                    {
                        var permissionIsNew = true;
                        foreach (XmlNode sourcePermission in sourcePermissionsList)
                            if (sourcePermission.Attributes != null && sourcePermission.Attributes.Count > 0 &&
                                sourcePermission.Attributes[0].Name == "android:name")
                                if (destPermission.Attributes[0].Value == sourcePermission.Attributes[0].Value)
                                    permissionIsNew = false;
                        if (permissionIsNew)
                            permissionsToAdd.Add(destPermission);
                    }

                if (permissionsToAdd.Count > 0)
                {
                    foreach (XmlNode node in permissionsToAdd)
                    {
                        var insertPlace = content.IndexOf("<application");
                        content = content.Insert(insertPlace, "<uses-permission android:name=\"" + node.Attributes[0].Value + "\"/>\n");
                    }
                }
            }
            
            File.Delete(destPath);
            File.WriteAllText(destPath, content);
            //Add Facebook Manifest to  application manifest
            ManifestMod.GenerateManifest();
        }
        
        public static XmlNodeList ExtractPermissions(string content)
        {
            XmlDocument xmlDoc = new XmlDocument ();
            xmlDoc.Load(new StringReader(content));
            string xmlPathPattern = "//manifest/uses-permission";
            XmlNodeList permissions = xmlDoc.SelectNodes(xmlPathPattern);
            return permissions;
        }

        private static void UpdateLauncherManifest()
        {
            string sourcePath = Path.Combine(Application.dataPath, SourceLauncherManifestPath);
            string destPath = Path.Combine(Application.dataPath, DestLauncherManifestPath);
            File.Copy(sourcePath, destPath, true);
        }

        private static void UpdateLauncherGradle()
        {
            string sourcePath = Path.Combine(Application.dataPath, SourceLauncherGradlePath);
            string contentLauncher = File.ReadAllText(sourcePath)
#if !UNITY_2022_2_OR_NEWER
                .Replace("ndkPath \"**NDKPATH**\"","")
#endif
                .Replace("**BUILD_SCRIPT_DEPS**", AndroidBuildToolsGradleClasspath);
#if UNITY_2020_1_OR_NEWER
        contentLauncher = contentLauncher.Replace("**STREAMING_ASSETS**]", "]+ unityStreamingAssets.tokenize(', ')");
#endif
            string destPath = Path.Combine(Application.dataPath, DestLauncherGradlePath);
            File.Delete(destPath);
            File.WriteAllText(destPath, contentLauncher);
        }


        private static void UpdateGradle()
        {
            string sourcePath = Path.Combine(Application.dataPath, SourceGradlePath);
            string content = File.ReadAllText(sourcePath)
                                 .Replace("**BUILD_SCRIPT_DEPS**", AndroidBuildToolsGradleClasspath)
                                 .Replace("**APPLY_PLUGINS**", "apply plugin: 'com.android.library'")
#if !UNITY_2022_2_OR_NEWER
                                 .Replace("ndkPath \"**NDKPATH**\"","")
#endif
                                 .Replace("**APPLICATIONID**", string.Empty);

#if UNITY_2020_1_OR_NEWER
        content = content.Replace("**STREAMING_ASSETS**", " + unityStreamingAssets.tokenize(', ')");
#endif

            string destPath = Path.Combine(Application.dataPath, DestGradlePath);
            File.Delete(destPath);
            File.WriteAllText(destPath, content);
        }
    }
}