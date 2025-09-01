using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Net;

namespace MMORPG.Client.UI
{
    public static class WorldBossTracking { public static string TrackedBossId; }

    public class WorldBossMapUI : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private Text itemTemplate;
        [SerializeField] private float refreshInterval = 1.0f;

        private float _timer;

        void Update()
        {
            _timer -= Time.deltaTime; if (_timer > 0f) return; _timer = refreshInterval; Rebuild();
        }

        void Rebuild()
        {
            foreach (Transform child in content) Destroy(child.gameObject);
#if SPACETIMEDB_SDK
            var types = BindingsBridge.GetWorldBossTypes().ToDictionary(t => t.Id, t => t);
            var spawns = BindingsBridge.GetWorldBossSpawns().Where(s => s.Alive).ToList();
            foreach (var s in spawns)
            {
                if (!types.TryGetValue(s.BossId, out var t)) continue;
                var go = new GameObject("WBItem"); go.transform.SetParent(content, false);
                var rt = go.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 30f);
                var bg = go.AddComponent<Image>(); bg.color = new Color(0,0,0,0.3f);
                var label = Instantiate(itemTemplate, go.transform); label.gameObject.SetActive(true);
                label.alignment = TextAnchor.MiddleLeft; label.text = $"{t.Name} — {t.Biome}  ({s.X:F0},{s.Z:F0})";
                var btnGo = new GameObject("TrackBtn"); btnGo.transform.SetParent(go.transform, false);
                var btn = btnGo.AddComponent<Button>(); var txt = btnGo.AddComponent<Text>(); txt.text = WorldBossTracking.TrackedBossId == s.BossId ? "Untrack" : "Track"; txt.color = Color.white; txt.alignment = TextAnchor.MiddleRight;
                btn.onClick.AddListener(() => { WorldBossTracking.TrackedBossId = WorldBossTracking.TrackedBossId == s.BossId ? null : s.BossId; Rebuild(); });
            }
#endif
        }
    }
}

