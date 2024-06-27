using System;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    [Serializable]
    public class BusinessEventParameters
    {
        public string currency;
        public int amount;
        public string itemType;
        public string itemId;
        public string cartType;
    }
}