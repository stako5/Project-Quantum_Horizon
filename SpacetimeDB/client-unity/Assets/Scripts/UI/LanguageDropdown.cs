using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    [RequireComponent(typeof(Dropdown))]
    public class LanguageDropdown : MonoBehaviour
    {
        [SerializeField] private Dropdown dropdown;
        void Awake()
        {
            if (!dropdown) dropdown = GetComponent<Dropdown>();
            var langs = MMORPG.Client.Localization.LocalizationManager.Languages;
            var options = new List<string>(langs);
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            // Set current language index
            string cur = MMORPG.Client.Localization.LocalizationManager.Language;
            int idx = options.IndexOf(cur); if (idx < 0) idx = 0;
            dropdown.value = idx;
            dropdown.onValueChanged.AddListener(i =>
            {
                if (i >= 0 && i < options.Count)
                    MMORPG.Client.Localization.LocalizationManager.Language = options[i];
            });
        }
        void OnDestroy(){ if (dropdown) dropdown.onValueChanged.RemoveAllListeners(); }
    }
}

