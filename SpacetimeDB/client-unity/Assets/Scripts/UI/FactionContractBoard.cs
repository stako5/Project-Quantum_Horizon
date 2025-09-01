using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class FactionContractBoard : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private Text itemTemplate;
        [SerializeField] private Button refreshButton;

        private readonly List<string> _factions = new() { "Parallax Concord", "Free Archipelagos", "Brass Arc Cartel", "Shard Tribunal" };

        async void Start()
        {
            if (refreshButton) refreshButton.onClick.AddListener(Rebuild);
            await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
            Rebuild();
        }

        void Rebuild()
        {
            foreach (Transform child in content) Destroy(child.gameObject);
            foreach (var name in _factions)
            {
                var row = new GameObject("Contract"); row.transform.SetParent(content, false);
                var rt = row.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 36);
                var bg = row.AddComponent<Image>(); bg.color = new Color(0,0,0,0.3f);
                var label = Instantiate(itemTemplate, row.transform); label.gameObject.SetActive(true); label.alignment = TextAnchor.MiddleLeft; label.text = name;
                // Add Accept (+rep) button
                var repBtnGo = new GameObject("Rep+10"); repBtnGo.transform.SetParent(row.transform, false);
                var repBtn = repBtnGo.AddComponent<Button>(); var repTxt = repBtnGo.AddComponent<Text>(); repTxt.text = "+Rep"; repTxt.color = Color.white; repTxt.alignment = TextAnchor.MiddleRight;
                repBtn.onClick.AddListener(async () => {
                    await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("adjust_reputation", name, 10);
                });
                // Add Buy Artifact button (if credits available) — example for "adaptive_module"
                var buyBtnGo = new GameObject("Buy"); buyBtnGo.transform.SetParent(row.transform, false);
                var buyBtn = buyBtnGo.AddComponent<Button>(); var buyTxt = buyBtnGo.AddComponent<Text>(); buyTxt.text = "Buy Artifact"; buyTxt.color = Color.white; buyTxt.alignment = TextAnchor.MiddleRight;
                buyBtn.onClick.AddListener(async () => {
                    await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("buy_artifact", "adaptive_module", 100u);
                });
            }
        }
    }
}

