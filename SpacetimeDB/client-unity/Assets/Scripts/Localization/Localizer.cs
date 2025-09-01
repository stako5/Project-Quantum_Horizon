using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.Localization
{
    [DisallowMultipleComponent]
    public class Localizer : MonoBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private Text text;

        void Awake()
        {
            if (!text) text = GetComponent<Text>();
            Apply();
            LocalizationManager.OnLanguageChanged += Apply;
        }

        void OnDestroy()
        {
            LocalizationManager.OnLanguageChanged -= Apply;
        }

        public void SetKey(string k) { key = k; Apply(); }

        void Apply()
        {
            if (!text || string.IsNullOrEmpty(key)) return;
            text.text = LocalizationManager.Get(key);
        }
    }
}

