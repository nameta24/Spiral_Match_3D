using UnityEngine;

namespace Voodoo.Sauce.Common.Utils
{
    public static class PlatformUtils
    {
        private const string TAG = "PlatformUtils";
        public static bool UNITY_IOS {
            get {
#if UNITY_IOS
                return true;
#else
                return false;
#endif
            }
        }

        public static bool UNITY_ANDROID {
            get {
#if UNITY_ANDROID
                return true;
#else
                return false;
#endif
            }
        }
        
        public static bool UNITY_EDITOR {
            get {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }
        
        public static bool IS_UNITY_2019_3_OR_NEWER {
            get {
#if UNITY_2019_3_OR_NEWER
                return true;
#else
                return false;
#endif
            }
        }

        public static readonly bool IS_LINUX = Application.platform == RuntimePlatform.LinuxEditor; 
    }
}