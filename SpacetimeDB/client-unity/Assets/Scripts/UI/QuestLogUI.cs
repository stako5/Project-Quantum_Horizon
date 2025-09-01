using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Net;

namespace MMORPG.Client.UI
{
    public class QuestLogUI : MonoBehaviour
    {
        [SerializeField] private RectTransform availableContent;
        [SerializeField] private RectTransform activeContent;
        [SerializeField] private Text itemTemplate;

        async void Start()
        {
            await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
            Rebuild();
        }

        void Clear(Transform t) { foreach (Transform c in t) Destroy(c.gameObject); }

        void AddRow(Transform parent, string text, System.Action onClick = null)
        {
            var row = new GameObject("Row"); row.transform.SetParent(parent, false);
            row.AddComponent<Image>().color = new Color(0,0,0,0.25f);
            var txt = Instantiate(itemTemplate, row.transform); txt.gameObject.SetActive(true); txt.text = text; txt.alignment = TextAnchor.MiddleLeft;
            if (onClick != null) { row.AddComponent<Button>().onClick.AddListener(() => onClick()); }
        }

        public void Rebuild()
        {
            Clear(availableContent); Clear(activeContent);
            var arcs = BindingsBridge.GetQuestArchetypes().ToDictionary(a => a.Id, a => a);
            var inst = BindingsBridge.GetQuestInstances().ToList();
            var avail = inst.Where(i => i.State == "available").ToList();
            var act = inst.Where(i => i.State == "active" || i.State == "completed").ToList();
            foreach (var q in avail)
            {
                if (!arcs.TryGetValue(q.ArchetypeId, out var a)) continue;
                string reward = $" [+{a.RewardCredits}c" + (string.IsNullOrEmpty(a.RewardItem)?"":$", {a.RewardItem}x{(a.RewardItemQty??1)}") + "]";
                AddRow(availableContent, $"[{q.Id}] {a.Name} — {a.Description} {reward}", async () => {
                    await SpacetimeDbClient.Instance.CallReducerAsync("assign_quest_to_self", (ulong)q.Id);
                    Rebuild();
                });
            }
            foreach (var q in act)
            {
                if (!arcs.TryGetValue(q.ArchetypeId, out var a)) continue;
                string state = q.State;
                AddRow(activeContent, $"[{state}] {a.Name} ({q.CurrentCount}/{a.RequiredCount})", async () => {
                    if (q.CurrentCount >= a.RequiredCount && state == "active")
                    {
                        await SpacetimeDbClient.Instance.CallReducerAsync("complete_quest", (ulong)q.Id);
                        Rebuild();
                    }
                });
            }
        }
    }
}
