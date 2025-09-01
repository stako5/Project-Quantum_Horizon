using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.InputSystem;

namespace MMORPG.Client.UI
{
    public class GameSettingsUI : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private Text itemTemplate;

        private InputBindings _bindings;
        private string _awaitingAction;

        class Row { public string action; public Text label; public Button rebind; }
        private readonly List<Row> _rows = new();

        void Start()
        {
            _bindings = InputManager.Instance ? InputManager.Instance.Bindings : InputBindings.Load();
            Build();
        }

        void Build()
        {
            foreach (Transform t in content) Destroy(t.gameObject);
            _rows.Clear();

            // Language row at top
            var langRow = new GameObject("LanguageRow"); langRow.transform.SetParent(content, false);
            var lrt = langRow.AddComponent<RectTransform>(); lrt.sizeDelta = new Vector2(0, 28);
            var lbg = langRow.AddComponent<Image>(); lbg.color = new Color(0,0,0,0.2f);
            // Label
            var lLabelGo = new GameObject("Label"); lLabelGo.transform.SetParent(langRow.transform, false);
            var lLabelRt = lLabelGo.AddComponent<RectTransform>(); lLabelRt.anchorMin = new Vector2(0,0); lLabelRt.anchorMax = new Vector2(0.5f,1); lLabelRt.offsetMin = new Vector2(6,2); lLabelRt.offsetMax = new Vector2(-6,-2);
            var lLabel = lLabelGo.AddComponent<Text>(); lLabel.text = MMORPG.Client.Localization.LocalizationManager.Get("language_label"); lLabel.color = Color.white; lLabel.alignment = TextAnchor.MiddleLeft; lLabel.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            // Control
            var lCtl = new GameObject("Control"); lCtl.transform.SetParent(langRow.transform, false);
            var lCtlRt = lCtl.AddComponent<RectTransform>(); lCtlRt.anchorMin = new Vector2(0.5f,0); lCtlRt.anchorMax = new Vector2(1,1); lCtlRt.offsetMin = new Vector2(6,2); lCtlRt.offsetMax = new Vector2(-6,-2);
            var ldd = lCtl.AddComponent<Dropdown>();
            ldd.options.Clear(); foreach (var code in MMORPG.Client.Localization.LocalizationManager.Languages) ldd.options.Add(new Dropdown.OptionData(code));
            int li = (new System.Collections.Generic.List<string>(MMORPG.Client.Localization.LocalizationManager.Languages)).IndexOf(MMORPG.Client.Localization.LocalizationManager.Language);
            ldd.value = li >= 0 ? li : 0;
            ldd.onValueChanged.AddListener(idx => { var langs = MMORPG.Client.Localization.LocalizationManager.Languages; if (idx >= 0 && idx < langs.Count) MMORPG.Client.Localization.LocalizationManager.Language = langs[idx]; });

            foreach (var action in InputBindings.Actions)
            {
                var rowGo = new GameObject(action); rowGo.transform.SetParent(content, false);
                var rt = rowGo.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 28);
                var bg = rowGo.AddComponent<Image>(); bg.color = new Color(0,0,0,0.2f);
                var label = Instantiate(itemTemplate, rowGo.transform); label.gameObject.SetActive(true); label.alignment = TextAnchor.MiddleLeft;
                var btnGo = new GameObject("Rebind"); btnGo.transform.SetParent(rowGo.transform, false);
                var btn = btnGo.AddComponent<Button>(); var btnTxt = btnGo.AddComponent<Text>(); btnTxt.color = Color.white; btnTxt.alignment = TextAnchor.MiddleRight; btnTxt.text = "Rebind";
                btn.onClick.AddListener(() => StartRebind(action));
                _rows.Add(new Row { action = action, label = label, rebind = btn });
            }
            RefreshLabels();
        }

        void RefreshLabels()
        {
            foreach (var r in _rows)
            {
                _bindings.Keys.TryGetValue(r.action, out var key);
                r.label.text = $"{r.action}: {key}";
            }
        }

        void StartRebind(string action)
        {
            _awaitingAction = action;
        }

        void Update()
        {
            if (string.IsNullOrEmpty(_awaitingAction)) return;
            if (!UnityEngine.Input.anyKeyDown) return;
            // Detect key pressed
            foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
            {
                if (UnityEngine.Input.GetKeyDown(kc))
                {
                    _bindings.Keys[_awaitingAction] = kc;
                    if (InputManager.Instance)
                    {
                        InputManager.Instance.Bindings.Keys[_awaitingAction] = kc;
                        InputManager.Instance.Save();
                    }
                    _awaitingAction = null;
                    RefreshLabels();
                    break;
                }
            }
        }
    }
}
