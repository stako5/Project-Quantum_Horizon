using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class SoulsHUD : MonoBehaviour
    {
        [Header("Bars")]
        [SerializeField] private Image hpFill;   // red
        [SerializeField] private Image fpFill;   // blue (mana)
        [SerializeField] private Image staFill;  // green (stamina)

        [Header("Quick Slots")]
        [SerializeField] private Image[] quickSlots; // bottom-left

        [Header("Souls/Score")]
        [SerializeField] private Text soulsText; // bottom-right

        [Header("Smoothing")]
        [SerializeField] private float smooth = 8f;

        [Header("Quests Button")]
        [SerializeField] private bool autoAddQuestsButton = true;
        [SerializeField] private string questsButtonLabel = "Quests";

        private float _hpTarget = 1f, _fpTarget = 1f, _staTarget = 1f;

        public void SetHP01(float v) { _hpTarget = Mathf.Clamp01(v); }
        public void SetFP01(float v) { _fpTarget = Mathf.Clamp01(v); }
        public void SetStamina01(float v) { _staTarget = Mathf.Clamp01(v); }
        public void SetSouls(long souls) { if (soulsText) soulsText.text = souls.ToString("N0"); }
        public void SetQuickSlotIcon(int index, Sprite sprite) { if (quickSlots != null && index>=0 && index < quickSlots.Length && quickSlots[index]) quickSlots[index].sprite = sprite; }

        void Awake()
        {
            if (autoAddQuestsButton)
            {
                EnsureQuestsButton();
            }
        }

        void Update()
        {
            if (hpFill) hpFill.fillAmount = Mathf.Lerp(hpFill.fillAmount, _hpTarget, Time.deltaTime * smooth);
            if (fpFill) fpFill.fillAmount = Mathf.Lerp(fpFill.fillAmount, _fpTarget, Time.deltaTime * smooth);
            if (staFill) staFill.fillAmount = Mathf.Lerp(staFill.fillAmount, _staTarget, Time.deltaTime * smooth);
        }

        void EnsureQuestsButton()
        {
            // If a QuestsButtonLink exists under this HUD, do nothing
            var existing = GetComponentInChildren<QuestsButtonLink>(true);
            if (existing) return;

            // Create a simple button bottom-right
            var go = new GameObject("QuestsButton");
            go.transform.SetParent(transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 0f); // bottom-right
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-20f, 20f);
            rt.sizeDelta = new Vector2(120f, 36f);

            var img = go.AddComponent<Image>(); img.color = new Color(0f, 0f, 0f, 0.5f);
            var btn = go.AddComponent<Button>();
            var link = go.AddComponent<QuestsButtonLink>();

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var trt = textGo.AddComponent<RectTransform>();
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one; trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            var t = textGo.AddComponent<Text>();
            t.text = string.IsNullOrEmpty(questsButtonLabel) ? "quests" : questsButtonLabel;
            t.alignment = TextAnchor.MiddleCenter; t.color = Color.white;
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            // Localize when not overridden
            if (string.IsNullOrEmpty(questsButtonLabel))
            {
                var loc = textGo.AddComponent<MMORPG.Client.Localization.Localizer>();
                loc.SetKey("quests");
            }
        }
    }
}
