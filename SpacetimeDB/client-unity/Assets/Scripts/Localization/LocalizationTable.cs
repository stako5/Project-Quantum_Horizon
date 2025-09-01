using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Localization
{
    [System.Serializable]
    public class LocalizedValue
    {
        public string language;
        [TextArea]
        public string value;
    }

    [System.Serializable]
    public class LocalizationEntry
    {
        public string key;
        public List<LocalizedValue> values = new();
    }

    [CreateAssetMenu(menuName = "MMORPG/Localization/Table", fileName = "LocalizationTable")]
    public class LocalizationTable : ScriptableObject
    {
        public List<LocalizationEntry> entries = new();

        public bool TryGet(string key, string language, out string value)
        {
            value = null; if (entries == null) return false;
            foreach (var e in entries)
            {
                if (e == null || e.key != key || e.values == null) continue;
                foreach (var v in e.values) if (v != null && v.language == language) { value = v.value; return true; }
            }
            return false;
        }
    }
}

