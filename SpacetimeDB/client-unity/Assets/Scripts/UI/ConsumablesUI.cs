using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Items;
using MMORPG.Client.Combat;

namespace MMORPG.Client.UI
{
    public class ConsumablesUI : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private Text itemTemplate;
        [SerializeField] private ConsumableUse consumer;

        void Start()
        {
            ConsumableCatalog.LoadFromDesign();
            Rebuild();
        }

        void Rebuild()
        {
            foreach (Transform child in content) Destroy(child.gameObject);
            var all = ConsumableCatalog.All();
            foreach (var c in all)
            {
                var row = new GameObject("Consumable"); row.transform.SetParent(content, false);
                var rt = row.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 28);
                var img = row.AddComponent<Image>(); img.color = new Color(0,0,0,0.25f);
                var txt = Instantiate(itemTemplate, row.transform); txt.gameObject.SetActive(true);
                txt.text = $"[{c.Tier}] {c.Name} — {c.Category}"; txt.alignment = TextAnchor.MiddleLeft;

                var btn = row.AddComponent<Button>();
                btn.onClick.AddListener(() => {
                    consumer.Use(c);
                });
            }
        }
    }
}
