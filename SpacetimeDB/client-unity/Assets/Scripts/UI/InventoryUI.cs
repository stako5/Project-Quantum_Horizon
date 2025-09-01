using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Items.Inventory inventory;
        [SerializeField] private Text goldText;
        [SerializeField] private RectTransform content;
        [SerializeField] private Text rowTemplate;
        [SerializeField] private float refreshInterval = 0.5f;

        private float _t;

        void Awake()
        {
            if (!inventory) inventory = FindObjectOfType<Items.Inventory>();
            if (inventory)
            {
                inventory.OnGoldChanged += _ => MarkDirty();
                inventory.OnItemChanged += (_, __) => MarkDirty();
            }
            if (rowTemplate) rowTemplate.gameObject.SetActive(false);
            MarkDirty();
        }

        void OnDestroy()
        {
            if (inventory)
            {
                inventory.OnGoldChanged -= _ => MarkDirty();
                inventory.OnItemChanged -= (_, __) => MarkDirty();
            }
        }

        void MarkDirty() { _t = 0f; }

        void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
            Rebuild();
        }

        void Rebuild()
        {
            if (!inventory) return;
            if (goldText)
            {
                var L = MMORPG.Client.Localization.LocalizationManager.Get;
                goldText.text = $"{L("gold")}: {inventory.gold:N0}";
            }
            if (!content || !rowTemplate) return;
            foreach (Transform c in content) if (c != rowTemplate.transform) Destroy(c.gameObject);
            var items = inventory.AllItems().Where(kv => kv.Value > 0).OrderBy(kv => kv.Key);
            foreach (var kv in items)
            {
                var row = Instantiate(rowTemplate, content);
                row.gameObject.SetActive(true);
                row.text = $"{kv.Key}  x{kv.Value}";
            }
        }
    }
}
