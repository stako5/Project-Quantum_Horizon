using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.Quests
{
    public class QuestsBootstrapper : MonoBehaviour
    {
        [SerializeField] private bool importOnPlay = true;
        [SerializeField] private TextAsset overrideJson;
        [SerializeField] private uint seedTotal = 1000;
        [SerializeField] private float tickIntervalSeconds = 10f;

        private float _t;

        async void Start()
        {
            await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
            if (importOnPlay)
            {
                string json = null;
                if (overrideJson && !string.IsNullOrEmpty(overrideJson.text)) json = overrideJson.text;
                else
                {
                    try
                    {
                        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "SpacetimeDB", "design", "quests", "data", "quests.json"));
                        if (File.Exists(path)) json = File.ReadAllText(path);
                    } catch (Exception e) { Debug.LogWarning($"[Quests] read error: {e.Message}"); }
                }
                if (!string.IsNullOrEmpty(json))
                {
                    try { await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("import_quests", json); }
                    catch (Exception e) { Debug.LogWarning($"[Quests] import failed: {e.Message}"); }
                    try { await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("seed_npc_quests", seedTotal); }
                    catch (Exception e) { Debug.LogWarning($"[Quests] seed failed: {e.Message}"); }
                }
            }
        }

        async void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = tickIntervalSeconds;
            try { await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("tick_quests"); }
            catch (Exception e) { Debug.LogWarning($"[Quests] tick failed: {e.Message}"); }
        }
    }
}
