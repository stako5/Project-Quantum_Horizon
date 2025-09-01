using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    [RequireComponent(typeof(Dropdown))]
    public class QualityDropdown : MonoBehaviour
    {
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private bool applyOnStart = true;
        [SerializeField] private string prefsKey = "options.quality";

        void Awake()
        {
            if (!dropdown) dropdown = GetComponent<Dropdown>();
            dropdown.ClearOptions();
            dropdown.AddOptions(QualitySettings.names.ToList());
            int cur = QualitySettings.GetQualityLevel();
            if (PlayerPrefs.HasKey(prefsKey)) cur = Mathf.Clamp(PlayerPrefs.GetInt(prefsKey), 0, QualitySettings.names.Length - 1);
            dropdown.value = cur;
            dropdown.onValueChanged.AddListener(OnChanged);
            if (applyOnStart) OnChanged(dropdown.value);
        }

        void OnDestroy()
        {
            if (dropdown) dropdown.onValueChanged.RemoveListener(OnChanged);
        }

        void OnChanged(int idx)
        {
            QualitySettings.SetQualityLevel(idx, true);
            PlayerPrefs.SetInt(prefsKey, idx);
        }
    }
}
