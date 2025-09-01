using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.Items
{
    public class ConsumablesBootstrapper : MonoBehaviour
    {
        [Header("Import")]
        [SerializeField] private bool importOnPlay = true;
        [SerializeField] private TextAsset overrideJson;

        [Header("Starter Pack")]
        [SerializeField] private bool grantStarterOnPlay = true;
        [SerializeField] private uint starterQtyEach = 3;
        [SerializeField] private List<string> starterConsumableIds = new();

        private static bool _doneThisSession;

        async void Start()
        {
            if (_doneThisSession) return; // avoid duplicate import on scene changes
            _doneThisSession = true;

            await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();

            if (importOnPlay)
            {
                string json = null;
                if (overrideJson != null && !string.IsNullOrEmpty(overrideJson.text)) json = overrideJson.text;
                else
                {
                    try
                    {
                        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "SpacetimeDB", "design", "consumables", "data", "consumables.json"));
                        if (File.Exists(path)) json = File.ReadAllText(path);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"[ConsumablesBootstrapper] Failed to read consumables.json: {e.Message}");
                    }
                }
                if (!string.IsNullOrEmpty(json))
                {
                    try { await new StdbConsumablesBackend().ImportAsync(json); }
                    catch (Exception e) { Debug.LogWarning($"[ConsumablesBootstrapper] import_consumables failed: {e.Message}"); }
                }
            }

            if (grantStarterOnPlay)
            {
                var ids = starterConsumableIds;
                if (ids == null || ids.Count == 0)
                {
                    // If not provided, pick a sample across categories and lower tiers from design JSON
                    try
                    {
                        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "SpacetimeDB", "design", "consumables", "data", "consumables.json"));
                        if (File.Exists(path))
                        {
                            var json = File.ReadAllText(path);
                            var env = JsonUtility.FromJson<ConsumablesEnvelope>(json);
                            var pool = env?.consumables ?? new List<ConsumableJson>();
                            ids = pool
                                .OrderBy(c => c.tier)
                                .ThenBy(c => c.category)
                                .Take(12)
                                .Select(c => c.id)
                                .ToList();
                        }
                    }
                    catch (Exception e) { Debug.LogWarning($"[ConsumablesBootstrapper] Starter selection failed: {e.Message}"); }
                }
                if (ids != null)
                {
                    foreach (var id in ids)
                    {
                        try { await new StdbConsumablesBackend().GrantAsync(id, (uint)starterQtyEach); }
                        catch (Exception e) { Debug.LogWarning($"[ConsumablesBootstrapper] grant {id} failed: {e.Message}"); }
                    }
                }
            }
        }

        [Serializable] class ConsumablesEnvelope { public List<ConsumableJson> consumables; }
        [Serializable] class ConsumableJson { public string id; public string name; public int tier; public string category; public float cooldown_s; public string stack; }
    }
}
