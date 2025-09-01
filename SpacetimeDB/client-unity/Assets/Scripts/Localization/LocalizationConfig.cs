using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Localization
{
    [CreateAssetMenu(menuName = "MMORPG/Localization/Config", fileName = "LocalizationConfig")]
    public class LocalizationConfig : ScriptableObject
    {
        [Tooltip("List of supported language codes (e.g., en, es, fr)")]
        public List<string> languages = new List<string> { "en" };
        [Tooltip("Default language code")] public string defaultLanguage = "en";
        [Tooltip("PlayerPrefs key for saved language")] public string prefsKey = "loc.lang";
    }
}

