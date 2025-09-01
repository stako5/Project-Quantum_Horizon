using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Net;

namespace MMORPG.Client.UI
{
    public class ArmorCodexUI : MonoBehaviour
    {
        [SerializeField] private Dropdown categoryDropdown; // All/World/Faction/Enemy/Boss
        [SerializeField] private RectTransform setsContent;
        [SerializeField] private Text itemTemplate;

        void Start()
        {
            if (categoryDropdown)
            {
                categoryDropdown.ClearOptions(); categoryDropdown.AddOptions(new List<string>{"All","World","Faction","Enemy","Boss"});
                categoryDropdown.onValueChanged.AddListener(_ => Rebuild());
            }
            Rebuild();
        }

        void Rebuild()
        {
            foreach (Transform child in setsContent) Destroy(child.gameObject);
            var infos = BindingsBridge.GetArmorSetInfos().ToDictionary(i => i.SetId, i => i);
            var sets = BindingsBridge.GetArmorSets().ToList();
            string cat = categoryDropdown ? categoryDropdown.options[categoryDropdown.value].text : "All";
            foreach (var s in sets)
            {
                if (infos.TryGetValue(s.Id, out var info))
                {
                    if (cat != "All" && info.Category != cat) continue;
                }
                var row = new GameObject("Set"); row.transform.SetParent(setsContent, false);
                var rt = row.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 28);
                var bg = row.AddComponent<Image>(); bg.color = new Color(0,0,0,0.25f);
                var label = Instantiate(itemTemplate, row.transform); label.gameObject.SetActive(true);
                var meta = infos.ContainsKey(s.Id) ? infos[s.Id] : null;
                string metaTxt = meta != null ? $"[{meta.Category}{(string.IsNullOrEmpty(meta.FactionName)?"":" "+meta.FactionName)}{(string.IsNullOrEmpty(meta.EnemyFamily)?"":" "+meta.EnemyFamily)}]" : "";
                label.text = $"{s.Name} {metaTxt}"; label.alignment = TextAnchor.MiddleLeft;
                // Pieces
                var pieces = BindingsBridge.GetArmorPieces(s.Id).ToList();
                foreach (var p in pieces)
                {
                    var pRow = new GameObject("Piece"); pRow.transform.SetParent(setsContent, false);
                    var prt = pRow.AddComponent<RectTransform>(); prt.sizeDelta = new Vector2(0, 22);
                    var pbg = pRow.AddComponent<Image>(); pbg.color = new Color(0,0,0,0.15f);
                    var ptxt = Instantiate(itemTemplate, pRow.transform); ptxt.gameObject.SetActive(true); ptxt.fontSize = 12; ptxt.text = $" - {p.Slot}: {p.Name}"; ptxt.alignment = TextAnchor.MiddleLeft;
                }
            }
        }
    }
}

