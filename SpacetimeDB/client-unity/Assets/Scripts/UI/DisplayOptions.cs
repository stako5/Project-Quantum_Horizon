using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class DisplayOptions : MonoBehaviour
    {
        [SerializeField] private Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Toggle vsyncToggle;
        [Header("Prefs Keys")]
        [SerializeField] private string prefResW = "options.resW";
        [SerializeField] private string prefResH = "options.resH";
        [SerializeField] private string prefResHz = "options.resHz";
        [SerializeField] private string prefFullscreen = "options.fullscreen";
        [SerializeField] private string prefVsync = "options.vsync";

        private Resolution[] _resolutions;
        private readonly List<string> _options = new();

        void Awake()
        {
            if (resolutionDropdown)
            {
                _resolutions = Screen.resolutions
                    .GroupBy(r => new { r.width, r.height, r.refreshRate })
                    .Select(g => g.First())
                    .OrderBy(r => r.width).ThenBy(r => r.height).ThenBy(r => r.refreshRate)
                    .ToArray();
                resolutionDropdown.ClearOptions();
                _options.Clear();
                int currentIndex = 0;
                for (int i = 0; i < _resolutions.Length; i++)
                {
                    var r = _resolutions[i];
                    string label = $"{r.width}x{r.height} @{r.refreshRate}Hz";
                    _options.Add(label);
                    if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height && r.refreshRate == Screen.currentResolution.refreshRate)
                        currentIndex = i;
                }
                resolutionDropdown.AddOptions(_options);
                // Apply saved resolution
                if (PlayerPrefs.HasKey(prefResW) && PlayerPrefs.HasKey(prefResH))
                {
                    int w = PlayerPrefs.GetInt(prefResW);
                    int h = PlayerPrefs.GetInt(prefResH);
                    int hz = PlayerPrefs.GetInt(prefResHz, Screen.currentResolution.refreshRate);
                    try { Screen.SetResolution(w, h, Screen.fullScreenMode, hz); } catch {}
                    // find index
                    for (int i = 0; i < _resolutions.Length; i++)
                    {
                        var r = _resolutions[i]; if (r.width == w && r.height == h && r.refreshRate == hz) { currentIndex = i; break; }
                    }
                }
                resolutionDropdown.value = currentIndex;
                resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            }
            if (fullscreenToggle)
            {
                bool fs = Screen.fullScreen;
                if (PlayerPrefs.HasKey(prefFullscreen)) fs = PlayerPrefs.GetInt(prefFullscreen) != 0;
                Screen.fullScreen = fs;
                fullscreenToggle.isOn = fs;
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
            }
            if (vsyncToggle)
            {
                int vs = QualitySettings.vSyncCount;
                if (PlayerPrefs.HasKey(prefVsync)) vs = PlayerPrefs.GetInt(prefVsync);
                QualitySettings.vSyncCount = vs > 0 ? 1 : 0;
                vsyncToggle.isOn = vs > 0;
                vsyncToggle.onValueChanged.AddListener(OnVsyncChanged);
            }
        }

        void OnDestroy()
        {
            if (resolutionDropdown) resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
            if (fullscreenToggle) fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
            if (vsyncToggle) vsyncToggle.onValueChanged.RemoveListener(OnVsyncChanged);
        }

        void OnResolutionChanged(int idx)
        {
            if (_resolutions == null || idx < 0 || idx >= _resolutions.Length) return;
            var r = _resolutions[idx];
            Screen.SetResolution(r.width, r.height, Screen.fullScreenMode, r.refreshRate);
            PlayerPrefs.SetInt(prefResW, r.width);
            PlayerPrefs.SetInt(prefResH, r.height);
            PlayerPrefs.SetInt(prefResHz, r.refreshRate);
        }

        void OnFullscreenChanged(bool isFullscreen)
        {
            Screen.fullScreenMode = isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
            PlayerPrefs.SetInt(prefFullscreen, isFullscreen ? 1 : 0);
        }

        void OnVsyncChanged(bool enabled)
        {
            QualitySettings.vSyncCount = enabled ? 1 : 0;
            PlayerPrefs.SetInt(prefVsync, enabled ? 1 : 0);
        }
    }
}
