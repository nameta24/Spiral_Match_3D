using UnityEngine;
using Voodoo.Sauce.Common.Utils;

namespace Voodoo.Sauce.Internal.Utils
{
    public static class DeviceUtils
    {
        private const string TAG = "DeviceUtils";

        private static string _operatingSystemVersion;


        private const string BuildClassName = "android.os.Build";
        private static string _versionClassName = $"{BuildClassName}$VERSION";
        public static string OperatingSystemVersion
        {
            get
            {
                if (_operatingSystemVersion == null)
                    _operatingSystemVersion = GetOperatingSystemVersion();
                return _operatingSystemVersion;
            }
        }
        
        public static string GetOperatingSystemVersion()
        {
            if (PlatformUtils.UNITY_IOS && !PlatformUtils.UNITY_EDITOR) {
                return UnityIosDevice.SystemVersion;
            }

            if (PlatformUtils.UNITY_ANDROID && !PlatformUtils.UNITY_EDITOR) {
                return $"Android API {CallDeviceInformationMethod<int>(_versionClassName, "SDK_INT")}";
            }

            return SystemInfo.operatingSystem;
        }
        
        private static T CallDeviceInformationMethod<T>(string className, string method)
        {
            using (AndroidJavaClass jo = new AndroidJavaClass(className)) {
                return jo.GetStatic<T>(method);
            }
        }

    }
}