using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voodoo.Analytics
{
    public class GlobalContext
    {
        private readonly Dictionary<string, string> _parameters;
        
        private const string K_PREFS_CACHED_PARAMETERS = "GlobalContext_CachedParameters";
        private const string K_PREFS_CACHED_PARAMETERS_KEY = "GlobalContext_CachedParameters_Key_{0}";

        public GlobalContext()
        {
            _parameters = new Dictionary<string, string>();
            
            Dictionary<string, string> cachedParameters = GetCachedParameters();
            foreach (KeyValuePair<string, string> param in cachedParameters) {
                _parameters.Add(param.Key, param.Value);
            }
        }

        public Dictionary<string, string> GetParameters() => _parameters;

        /// <summary>
        /// Add a key/value pair to the global context for all VAN events.
        /// </summary>
        /// <param name="key">Parameter key</param>
        /// <param name="value">Value of this key (can only be a string, int or bool)</param>
        /// <param name="cached">If true, the value will be cached and used without the need to call this method</param>
        public void Add(string key, string value, bool cached)
        {
            if (!_parameters.ContainsKey(key)) {
                _parameters.Add(key, value);
            } else {
                _parameters[key] = value;
            }

            if (cached) {
                AddParameterToCache(key, value);
            }
        }

        private void AddParameterToCache(string key, string value)
        {
            CachedKeys cachedKeys = GetCachedKeys();
            cachedKeys.keys.Add(key);
            
            PlayerPrefs.SetString(K_PREFS_CACHED_PARAMETERS, JsonUtility.ToJson(cachedKeys));
            SetCachedValue(key, value);
            PlayerPrefs.Save();
        }

        private CachedKeys GetCachedKeys()
        {
            string cachedParametersStr = PlayerPrefs.GetString(K_PREFS_CACHED_PARAMETERS, "");
            var cachedParameters = new CachedKeys();
            if (!string.IsNullOrEmpty(cachedParametersStr)) {
                cachedParameters = JsonUtility.FromJson<CachedKeys>(cachedParametersStr);
            } else {
                cachedParameters.keys = new List<string>();
            }
            return cachedParameters;
        }
        
        private Dictionary<string, string> GetCachedParameters()
        {
            CachedKeys cachedKeys = GetCachedKeys();
            var cachedParameters = new Dictionary<string, string>();
            foreach (string key in cachedKeys.keys) {
                string value = GetCachedValue(key);
                if (!string.IsNullOrEmpty(value)) {
                    if (!cachedParameters.ContainsKey(key)) {
                        cachedParameters.Add(key, value);
                    } else {
                        cachedParameters[key] = value;
                    }
                }
            }
            return cachedParameters;
        }

        private string GetPrefsKey(string key)
        {
            return string.Format(K_PREFS_CACHED_PARAMETERS_KEY, key);
        }

        public string GetCachedValue(string key)
        {
            string kPrefsKeys = GetPrefsKey(key);
            return PlayerPrefs.GetString(kPrefsKeys, "");
        }
        
        private void SetCachedValue(string key, string value)
        {
            string kPrefsKeys = GetPrefsKey(key);
            PlayerPrefs.SetString(kPrefsKeys, value);
            PlayerPrefs.Save();
        }

        public void DeleteAllCachedValues()
        {
            CachedKeys cachedKeys = GetCachedKeys();
            foreach (string key in cachedKeys.keys) {
                string prefsKeys = GetPrefsKey(key);
                PlayerPrefs.DeleteKey(prefsKeys);
            }
            PlayerPrefs.DeleteKey(K_PREFS_CACHED_PARAMETERS);
        }

        [Serializable]
        internal struct CachedKeys
        {
            public List<string> keys;
        }
    }
}