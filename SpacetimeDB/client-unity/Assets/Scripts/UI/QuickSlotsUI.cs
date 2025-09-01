
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Items;

namespace MMORPG.Client.UI
{
    public class QuickSlotsUI : MonoBehaviour
    {
        [SerializeField] private int slotCount = 4;
        [SerializeField] private ConsumableUse consumer;
        [SerializeField] private RectTransform root;

        private readonly List<Slot> _slots = new();

        class Slot
        {
            public Consumable c;
            public Button button;
            public Text label;
            public Image cooldownOverlay;
            public KeyCode key;
        }

        void Awake()
        {
            if (!consumer) consumer = FindObjectOfType<ConsumableUse>();
            if (!root) root = GetComponent<RectTransform>();
            BuildUI();
            PopulateDefaults();
        }

        void BuildUI()
        {
            var hlg = root.gameObject.GetComponent<HorizontalLayoutGroup>();
            if (!hlg) { hlg = root.gameObject.AddComponent<HorizontalLayoutGroup>(); hlg.spacing = 8; }
            for (int i = 0; i < slotCount; i++)
            {
                var go = new GameObject($"QS{i+1}"); go.transform.SetParent(root, false);
                var img = go.AddComponent<Image>(); img.color = new Color(1,1,1,0.12f);
                var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(96, 48);
                var btn = go.AddComponent<Button>();

                var textGo = new GameObject("Label"); textGo.transform.SetParent(go.transform, false);
                var txt = textGo.AddComponent<Text>(); txt.color = Color.white; txt.alignment = TextAnchor.MiddleCenter; txt.raycastTarget = false;
                var trt = txt.GetComponent<RectTransform>(); trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one; trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;

                var cdGo = new GameObject("Cooldown"); cdGo.transform.SetParent(go.transform, false);
                var cd = cdGo.AddComponent<Image>(); cd.color = new Color(0,0,0,0.5f); cd.type = Image.Type.Filled; cd.fillMethod = Image.FillMethod.Vertical; cd.fillAmount = 0f; cd.raycastTarget = false;
                var crt = cd.GetComponent<RectTransform>(); crt.anchorMin = Vector2.zero; crt.anchorMax = Vector2.one; crt.offsetMin = Vector2.zero; crt.offsetMax = Vector2.zero;

                var slot = new Slot { c = null, button = btn, label = txt, cooldownOverlay = cd, key = (KeyCode)((int)KeyCode.Alpha1 + i) };
                int captured = i;
                btn.onClick.AddListener(() => TryUse(captured));
                _slots.Add(slot);
            }
        }

        void PopulateDefaults()
        {
            ConsumableCatalog.LoadFromDesign();
            var seed = ConsumableCatalog.All()
                .OrderBy(c => c.Tier)
                .ThenBy(c => c.Category)
                .Take(slotCount)
                .ToList();
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].c = i < seed.Count ? seed[i] : null;
                RefreshSlot(i);
            }
        }

        void Update()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                var s = _slots[i];
                if (s.c != null && consumer)
                {
                    float rem = consumer.GetRemainingCooldown(s.c);
                    s.cooldownOverlay.fillAmount = s.c.CooldownS > 0f ? Mathf.Clamp01(rem / s.c.CooldownS) : 0f;
                }
                if (Input.GetKeyDown(s.key)) TryUse(i);
            }
        }

        void TryUse(int index)
        {
            if (index < 0 || index >= _slots.Count) return;
            var s = _slots[index]; if (s.c == null || consumer == null) return;
            consumer.Use(s.c);
            RefreshSlot(index);
        }

        void RefreshSlot(int index)
        {
            var s = _slots[index];
            if (s.c == null) { s.label.text = $"[{index+1}] —"; return; }
            s.label.text = $"[{index+1}] T{s.c.Tier} {s.c.Category}";
        }
    }
}
