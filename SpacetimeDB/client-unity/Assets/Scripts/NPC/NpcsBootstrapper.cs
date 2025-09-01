using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.NPC
{
    public class NpcsBootstrapper : MonoBehaviour
    {
        [SerializeField] private bool importOnPlay = true;
        [SerializeField] private TextAsset overrideJson;
        [SerializeField] private byte tickRegion = 0;
        [SerializeField] private float tickIntervalSeconds = 5f;
        [SerializeField] private bool enableGossip = true;
        [SerializeField] private uint gossipSpreadsPerTick = 8;

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
                        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "SpacetimeDB", "design", "npcs", "data", "npcs.json"));
                        if (File.Exists(path)) json = File.ReadAllText(path);
                    } catch (Exception e) { Debug.LogWarning($"[NPCs] read error: {e.Message}"); }
                }
                if (!string.IsNullOrEmpty(json))
                {
                    try { await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("import_npcs", json); }
                    catch (Exception e) { Debug.LogWarning($"[NPCs] import failed: {e.Message}"); }
                }
            }
        }

        async void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = tickIntervalSeconds;
            try
            {
                var hour = (byte)System.DateTime.UtcNow.Hour;
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("tick_npcs", (uint)tickRegion, (uint)hour);
                if (enableGossip)
                {
                    await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("tick_npc_gossip", (uint)tickRegion, gossipSpreadsPerTick);
                }
            }
            catch (Exception e) { Debug.LogWarning($"[NPCs] tick failed: {e.Message}"); }
        }
    }
}
