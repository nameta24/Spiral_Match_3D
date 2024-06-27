using System;
using GameAnalyticsSDK;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    [Serializable]
    public class ResourceEventParameters
    {
        public string currency;
        public int amount;
        public string itemType;
        public string itemName;
    }
}