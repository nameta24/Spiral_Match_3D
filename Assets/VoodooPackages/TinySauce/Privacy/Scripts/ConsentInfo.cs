using System;

namespace Voodoo.Tiny.Sauce.Privacy
{
    [Serializable]
    public class ConsentInfo
    {
        private const string TAG = "ConsentInfo";
        public bool need_consent; //always true, before gdpr_applicable
        public bool already_consent;
        public bool embargoed_country;
        public string country_code;
        public bool ads_consent;
        public bool analytics_consent;
        public string texts;
        public bool is_gdpr; //new field
        public string privacy_version; //new field
        public bool is_ccpa; //new field
    }
}
