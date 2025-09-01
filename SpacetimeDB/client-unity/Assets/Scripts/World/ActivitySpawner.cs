using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MMORPG.Client.World
{
    public static class ActivityHistory
    {
        static readonly Queue<string> recent = new Queue<string>();
        static readonly HashSet<string> set = new HashSet<string>();
        const int Max = 32;
        public static bool Seen(string key) => set.Contains(key);
        public static void Add(string key)
        {
            if (set.Contains(key)) return;
            recent.Enqueue(key); set.Add(key);
            while (recent.Count > Max) { var old = recent.Dequeue(); set.Remove(old); }
        }
    }

    public class ActivitySpawner : MonoBehaviour
    {
        public static void SpawnPOIs(ProcGenConfig config, Rect bounds, string activeBiome, Transform parent, System.Random rand)
        {
            if (!config.activitiesJson || string.IsNullOrEmpty(config.activitiesJson.text)) return;
            ActivityCatalog.Load(config.activitiesJson);
            var list = ActivityCatalog.PoisFor(activeBiome);
            if (list.Count == 0) return;
            float areaM2 = bounds.width * bounds.height; float areaHa = areaM2 / 10000f;
            foreach (var poi in list)
            {
                var lambda = Mathf.Max(0f, poi.density_per_hectare * areaHa * config.globalDensityScale);
                int count = SamplePoisson(lambda, rand);
                ScatterPOI(config, poi, count, bounds, parent, rand);
            }
        }

        static void ScatterPOI(ProcGenConfig config, PoiDef def, int count, Rect bounds, Transform parent, System.Random rand)
        {
            var rule = config.poiPrefabs.FirstOrDefault(r => r.name == def.name);
            const int maxAttempts = 12;
            var placed = new List<Vector2>();
            for (int i = 0; i < count; i++)
            {
                bool ok = false; for (int a=0;a<maxAttempts && !ok;a++)
                {
                    float px = bounds.xMin + (float)rand.NextDouble() * bounds.width;
                    float pz = bounds.yMin + (float)rand.NextDouble() * bounds.height;
                    var p2 = new Vector2(px, pz);
                    ok = true;
                    float minSpacing = rule != null ? rule.minSpacing : 8f;
                    foreach (var prev in placed) { if (Vector2.Distance(prev, p2) < minSpacing) { ok=false; break; } }
                    if (ok)
                    {
                        placed.Add(p2);
                        var key = $"poi:{def.name}:{Mathf.RoundToInt(px)}:{Mathf.RoundToInt(pz)}";
                        if (ActivityHistory.Seen(key)) continue; // avoid repetition
                        ActivityHistory.Add(key);
                        GameObject go = rule != null && rule.prefab ? GameObject.Instantiate(rule.prefab, parent) : GameObject.CreatePrimitive(PrimitiveType.Cube);
                        go.name = def.name; go.transform.position = new Vector3(px, 0f, pz);
                        var interact = go.AddComponent<ActivityInteractable>();
                        interact.activityKey = $"poi:{def.name}";
                        interact.biome = def.biome;
                    }
                }
            }
        }

        static int SamplePoisson(float lambda, System.Random rand)
        {
            if (lambda <= 0f) return 0; double L = System.Math.Exp(-lambda); int k = 0; double p = 1.0; do { k++; p *= rand.NextDouble(); } while (p > L); return k - 1;
        }
    }
}
