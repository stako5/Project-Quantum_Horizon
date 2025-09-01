using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Localization
{
    public static class LocalizationManager
    {
        private static string _language;
        private static LocalizationConfig _config;
        private static List<LocalizationTable> _tables;
        public static System.Action OnLanguageChanged;

        static void Ensure()
        {
            if (_config == null) _config = Resources.Load<LocalizationConfig>("LocalizationConfig");
            if (string.IsNullOrEmpty(_language))
            {
                string def = _config ? _config.defaultLanguage : "en";
                string key = _config ? _config.prefsKey : "loc.lang";
                _language = PlayerPrefs.GetString(key, def);
            }
            if (_tables == null)
            {
                _tables = new List<LocalizationTable>(Resources.LoadAll<LocalizationTable>("Localization"));
            }
        }

        public static string Language
        {
            get { Ensure(); return _language; }
            set
            {
                Ensure();
                if (string.IsNullOrEmpty(value) || value == _language) return;
                _language = value;
                string key = _config ? _config.prefsKey : "loc.lang";
                PlayerPrefs.SetString(key, _language);
                OnLanguageChanged?.Invoke();
            }
        }

        public static IReadOnlyList<string> Languages
        {
            get
            {
                Ensure();
                if (_config && _config.languages != null && _config.languages.Count > 0) return _config.languages;
                return new List<string> { "en" };
            }
        }

        public static string Get(string key)
        {
            Ensure();
            if (_tables != null)
            {
                foreach (var t in _tables)
                {
                    if (t != null && t.TryGet(key, _language, out var val)) return val;
                }
            }
            return key; // fallback to key
        }
    }
}

