using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MMORPG.Client.Launcher
{
    public class LauncherController : MonoBehaviour
    {
        [SerializeField] private LauncherConfig config;
        [Header("UI")]
        [SerializeField] private Image logoImage;
        [SerializeField] private Text titleText;
        [SerializeField] private Dropdown scenesDropdown;
        [SerializeField] private Button playButton;
        [SerializeField] private Button continueButton;
        [Header("Profiles")]
        [SerializeField] private Dropdown profileDropdown;
        [SerializeField] private InputField newProfileInput;
        [SerializeField] private Button createProfileButton;
        [Header("Loading Overlay")]
        [SerializeField] private CanvasGroup loadingOverlay;
        [SerializeField] private Slider progressBar;
        [SerializeField] private Text progressText;
        [SerializeField] private Button quitButton;

        private readonly List<string> _scenes = new();
        [Header("Shortcuts")]
        [SerializeField] private bool enableShortcuts = true;
        [SerializeField] private KeyCode playKey = KeyCode.Return;
        [SerializeField] private KeyCode continueKey = KeyCode.C;
        [SerializeField] private KeyCode quitKey = KeyCode.Escape;
        [SerializeField] private KeyCode fullscreenKey = KeyCode.Return; // with Alt
        [Header("Splash")]
        [SerializeField] private CanvasGroup splashOverlay;
        [SerializeField] private float splashFadeIn = 0.8f;

        void Awake()
        {
            if (!config) config = Resources.Load<LauncherConfig>("LauncherConfig");
            BuildUI();
            if (playButton) playButton.onClick.AddListener(OnPlay);
            if (continueButton) continueButton.onClick.AddListener(OnContinue);
            if (createProfileButton) createProfileButton.onClick.AddListener(OnCreateProfile);
            if (quitButton) quitButton.onClick.AddListener(OnQuit);

            if (splashOverlay)
            {
                splashOverlay.alpha = 1f; splashOverlay.blocksRaycasts = true; splashOverlay.interactable = true;
                StartCoroutine(FadeOutSplash());
            }
        }

        void OnDestroy()
        {
            if (playButton) playButton.onClick.RemoveListener(OnPlay);
            if (continueButton) continueButton.onClick.RemoveListener(OnContinue);
            if (createProfileButton) createProfileButton.onClick.RemoveListener(OnCreateProfile);
            if (quitButton) quitButton.onClick.RemoveListener(OnQuit);
        }

        void BuildUI()
        {
            if (config)
            {
                if (logoImage && config.logo) logoImage.sprite = config.logo;
                if (titleText && !string.IsNullOrEmpty(config.gameTitle)) titleText.text = config.gameTitle;
            }
            BuildProfiles();
            _scenes.Clear();
            if (config && config.sceneNames != null && config.sceneNames.Count > 0)
                _scenes.AddRange(config.sceneNames);
            else
            {
                // As a fallback, include current active scene and any known samples
                _scenes.Add(SceneManager.GetActiveScene().name);
                _scenes.Add("FPSDemo");
                _scenes.Add("Bootstrap");
                _scenes.Add("Overworld");
            }
            if (scenesDropdown)
            {
                scenesDropdown.ClearOptions();
                scenesDropdown.AddOptions(_scenes);
                int index = 0;
                if (config)
                {
                    index = Mathf.Clamp(config.defaultSceneIndex, 0, _scenes.Count - 1);
                    if (config.rememberLastSelection)
                        index = PlayerPrefs.GetInt(config.prefsKey, index);
                }
                scenesDropdown.value = index;
            }

            UpdateContinueButtonState();
        }

        void Update()
        {
            if (!enableShortcuts) return;
            // Alt+Enter toggles fullscreen
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(fullscreenKey))
            {
                ToggleFullscreen();
            }
            else if (Input.GetKeyDown(quitKey))
            {
                OnQuit();
            }
            else if (Input.GetKeyDown(playKey))
            {
                OnPlay();
            }
            else if (Input.GetKeyDown(continueKey) && continueButton && continueButton.interactable)
            {
                OnContinue();
            }
        }

        void BuildProfiles()
        {
            var profiles = SaveSystem.GetProfiles();
            if (!profileDropdown) return;
            profileDropdown.ClearOptions();
            var names = new System.Collections.Generic.List<string>();
            int activeIndex = 0; int i = 0;
            var active = SaveSystem.GetActive();
            foreach (var p in profiles)
            {
                names.Add(p.name);
                if (active != null && p.id == active.id) activeIndex = i;
                i++;
            }
            profileDropdown.AddOptions(names);
            profileDropdown.value = activeIndex;
            profileDropdown.onValueChanged.AddListener(idx => { var list = SaveSystem.GetProfiles(); if (idx >= 0 && idx < list.Count) SaveSystem.SetActive(list[idx].id); UpdateContinueButtonState(); });
            if (newProfileInput) newProfileInput.text = "";
        }

        void UpdateContinueButtonState()
        {
            if (!continueButton) return;
            var last = SaveSystem.GetLastSceneForActive();
            continueButton.interactable = !string.IsNullOrEmpty(last);
        }

        void OnPlay()
        {
            int idx = scenesDropdown ? scenesDropdown.value : 0;
            if (config && config.rememberLastSelection) PlayerPrefs.SetInt(config.prefsKey, idx);
            string scene = (idx >= 0 && idx < _scenes.Count) ? _scenes[idx] : null;
            if (!string.IsNullOrEmpty(scene))
            {
                SaveSystem.SetLastSceneForActive(scene);
                StartCoroutine(LoadSceneAsync(scene));
            }
        }

        void OnContinue()
        {
            var scene = SaveSystem.GetLastSceneForActive();
            if (!string.IsNullOrEmpty(scene)) StartCoroutine(LoadSceneAsync(scene));
        }

        void OnCreateProfile()
        {
            string name = newProfileInput ? newProfileInput.text : null;
            SaveSystem.CreateProfile(name);
            BuildProfiles();
        }

        System.Collections.IEnumerator LoadSceneAsync(string scene)
        {
            if (loadingOverlay) { loadingOverlay.alpha = 1f; loadingOverlay.blocksRaycasts = true; loadingOverlay.interactable = true; }
            var op = SceneManager.LoadSceneAsync(scene);
            op.allowSceneActivation = false;
            while (!op.isDone)
            {
                float p = Mathf.Clamp01(op.progress / 0.9f);
                if (progressBar) progressBar.value = p;
                if (progressText) progressText.text = $"Loading... {p*100f:0}%";
                if (op.progress >= 0.9f)
                {
                    // brief hold to show 100%, then activate
                    yield return new WaitForSeconds(0.1f);
                    op.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        System.Collections.IEnumerator FadeOutSplash()
        {
            float t = 0f; float d = Mathf.Max(0.01f, splashFadeIn);
            while (t < d)
            {
                t += Time.deltaTime; float k = 1f - Mathf.Clamp01(t / d);
                splashOverlay.alpha = k; yield return null;
            }
            splashOverlay.alpha = 0f; splashOverlay.blocksRaycasts = false; splashOverlay.interactable = false;
        }

        void ToggleFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
        void OnQuit()
        {
            Application.Quit();
        }
    }
}
