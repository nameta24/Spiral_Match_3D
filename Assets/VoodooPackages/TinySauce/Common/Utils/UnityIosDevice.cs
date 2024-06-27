namespace Voodoo.Sauce.Common.Utils
{
    public class UnityIosDevice
    {
        private const string TAG = "UnityIosDevice";
        public static string Generation {
            get {
#if UNITY_IOS
                return UnityEngine.iOS.Device.generation.ToString();
#else
                return null;
#endif
            }
        }

        public static string SystemVersion {
            get {
#if UNITY_IOS
                return UnityEngine.iOS.Device.systemVersion;
#else
                return null;
#endif
            }
        }

        public static string VendorIdentifier {
            get {
#if UNITY_IOS
                return UnityEngine.iOS.Device.vendorIdentifier;
#else
                return null;
#endif
            }
        }
        public static string AdvertisingIdentifier {
            get {
#if UNITY_IOS
                return UnityEngine.iOS.Device.advertisingIdentifier;
#else
                return null;
#endif
            }
        }
    }
}