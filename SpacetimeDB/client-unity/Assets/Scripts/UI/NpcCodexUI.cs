using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Net;

namespace MMORPG.Client.UI
{
    public class NpcCodexUI : MonoBehaviour
    {
        [SerializeField] private RectTransform listContent;
        [SerializeField] private Text listItemTemplate;
        [SerializeField] private RectTransform detailContent;
        [SerializeField] private Text detailLineTemplate;
        [SerializeField] private RectTransform memoriesContent;
        [SerializeField] private Text memoryItemTemplate;
        [SerializeField] private RectTransform offeredContent;
        [SerializeField] private Text offeredItemTemplate;
        [SerializeField] private Slider opennessSlider;
        [SerializeField] private Slider conscientiousnessSlider;
        [SerializeField] private Slider extraversionSlider;
        [SerializeField] private Text personalitySummaryText;
        [SerializeField] private InputField searchField;

        private ulong _selectedNpcId;

        void Start()
        {
            if (searchField) searchField.onValueChanged.AddListener(_ => RebuildList());
            RebuildList();
        }

        void Clear(Transform t)
        {
            foreach (Transform c in t) Destroy(c.gameObject);
        }

        public void RebuildList()
        {
            Clear(listContent);
            string q = searchField ? (searchField.text ?? string.Empty).ToLowerInvariant() : string.Empty;
            var npcs = BindingsBridge.GetNpcs()
                .Where(n => string.IsNullOrEmpty(q) || (n.Name != null && n.Name.ToLowerInvariant().Contains(q)) || (n.Faction != null && n.Faction.ToLowerInvariant().Contains(q)))
                .OrderBy(n => n.Name)
                .ToList();
            foreach (var n in npcs)
            {
                var row = new GameObject("NpcRow"); row.transform.SetParent(listContent, false);
                var bg = row.AddComponent<Image>(); bg.color = new Color(0,0,0,0.25f);
                var txt = Instantiate(listItemTemplate, row.transform); txt.gameObject.SetActive(true);
                txt.text = $"{n.Name} — {n.Faction} / {n.Role}"; txt.alignment = TextAnchor.MiddleLeft;
                var btn = row.AddComponent<Button>();
                ulong id = n.Id;
                btn.onClick.AddListener(() => { _selectedNpcId = id; RebuildDetail(); });
            }
        }

        void AddDetailLine(string text)
        {
            var t = Instantiate(detailLineTemplate, detailContent); t.gameObject.SetActive(true); t.text = text; t.alignment = TextAnchor.MiddleLeft;
        }

        public void RebuildDetail()
        {
            Clear(detailContent); Clear(memoriesContent);
            if (_selectedNpcId == 0) return;
            var n = BindingsBridge.GetNpcs().FirstOrDefault(x => x.Id == _selectedNpcId);
            if (n == null) return;
            AddDetailLine($"Name: {n.Name}");
            AddDetailLine($"Faction/Role: {n.Faction} / {n.Role}");
            AddDetailLine($"Personality: {n.Personality}");
            AddDetailLine($"Home Region: {n.HomeRegion}");
            AddDetailLine($"Bio: {n.Bio}");

            // Parse and show trait sliders if available
            if (opennessSlider || conscientiousnessSlider || extraversionSlider || personalitySummaryText)
            {
                ParseTraits(n.Personality ?? string.Empty, out var o, out var c, out var e);
                if (opennessSlider) opennessSlider.value = o;
                if (conscientiousnessSlider) conscientiousnessSlider.value = c;
                if (extraversionSlider) extraversionSlider.value = e;
                if (personalitySummaryText) personalitySummaryText.text = $"O:{o:F2} C:{c:F2} E:{e:F2}";
            }

            var st = BindingsBridge.GetNpcStates().FirstOrDefault(s => s.NpcId == _selectedNpcId);
            if (st != null)
            {
                AddDetailLine($"State: {st.Activity} @ Region {st.RegionId} ({st.X:F0},{st.Z:F0})");
            }

            var mems = BindingsBridge.GetNpcMemories().Where(m => m.NpcId == _selectedNpcId).OrderByDescending(m => m.At).Take(20).ToList();
            foreach (var m in mems)
            {
                var row = new GameObject("Mem"); row.transform.SetParent(memoriesContent, false);
                var bg = row.AddComponent<Image>(); bg.color = new Color(0,0,0,0.18f);
                var txt = Instantiate(memoryItemTemplate, row.transform); txt.gameObject.SetActive(true);
                System.DateTimeOffset dto = System.DateTimeOffset.FromUnixTimeMilliseconds(m.At/1000);
                txt.text = $"[{dto:HH:mm}] {m.Kind}: {m.Description}";
                txt.alignment = TextAnchor.MiddleLeft;
            }

            if (offeredContent && offeredItemTemplate)
            {
                Clear(offeredContent);
                var arcs = BindingsBridge.GetQuestArchetypes().ToDictionary(a => a.Id, a => a);
                var offered = BindingsBridge.GetQuestInstances().Where(q => q.NpcId == _selectedNpcId && q.State == "available").ToList();
                foreach (var q in offered)
                {
                    if (!arcs.TryGetValue(q.ArchetypeId, out var a)) continue;
                    var row = new GameObject("OfferedQuest"); row.transform.SetParent(offeredContent, false);
                    row.AddComponent<Image>().color = new Color(0,0,0,0.22f);
                    var txt = Instantiate(offeredItemTemplate, row.transform); txt.gameObject.SetActive(true);
                    string reward = $" [+{a.RewardCredits}c" + (string.IsNullOrEmpty(a.RewardItem)?"":$", {a.RewardItem}x{(a.RewardItemQty??1)}") + "]";
                    txt.text = $"{a.Name} — {a.Description} {reward}"; txt.alignment = TextAnchor.MiddleLeft;
                    var btn = row.AddComponent<Button>();
                    ulong id = q.Id;
                    btn.onClick.AddListener(async () => { await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("assign_quest_to_self", (ulong)id); RebuildDetail(); });
                }
            }
        }

        static void ParseTraits(string s, out float o, out float c, out float e)
        {
            o = c = e = 0.5f;
            try
            {
                var parts = s.Split(new[] { ' ', ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                {
                    if (p.StartsWith("O:", System.StringComparison.OrdinalIgnoreCase)) { float.TryParse(p.Substring(2), out o); }
                    else if (p.StartsWith("C:", System.StringComparison.OrdinalIgnoreCase)) { float.TryParse(p.Substring(2), out c); }
                    else if (p.StartsWith("E:", System.StringComparison.OrdinalIgnoreCase)) { float.TryParse(p.Substring(2), out e); }
                }
                o = Mathf.Clamp01(o); c = Mathf.Clamp01(c); e = Mathf.Clamp01(e);
            }
            catch { o = c = e = 0.5f; }
        }
    }
}
