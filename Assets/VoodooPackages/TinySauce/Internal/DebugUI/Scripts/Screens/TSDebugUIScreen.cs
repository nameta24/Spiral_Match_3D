using UnityEngine;

namespace Voodoo.Tiny.Sauce.Internal
{
    public class TSDebugUIScreen : MonoBehaviour
    {
        private const string TAG = "TSDebugUIScreen";
        protected TinySauceSettings _tsSettings;

        public TinySauceSettings TSSettings
        {
            get => _tsSettings;
            set
            {
                _tsSettings = value;
                UpdateInfo();
            }
        }

        protected virtual void UpdateInfo() { }
    }
}
