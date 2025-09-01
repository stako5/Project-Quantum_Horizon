using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.World
{
    [System.Serializable]
    public class BiomeEnvelope { public int count; public List<BiomeDefModel> biomes; }
    [System.Serializable]
    public class BiomeDefModel
    {
        public int id; public string name; public long seed; public int size_meters; public int chunk_size_m;
        public SerializableDict palette; public string description;
        public List<SpawnWeight> spawn_weights;
    }
    [System.Serializable] public class SerializableDict { }
    [System.Serializable] public class SpawnWeight { public string family; public float multiplier; }

    public class WorldRegionManager : MonoBehaviour
    {
        [SerializeField] private ProcGenConfig config;
        [SerializeField] private TextAsset biomesJson;
        [SerializeField] private Transform player;
        [SerializeField] private WorldStreamer streamer;

        private BiomeEnvelope _env;

        void Awake()
        {
            if (biomesJson && !string.IsNullOrEmpty(biomesJson.text))
            {
                _env = JsonUtility.FromJson<BiomeEnvelope>(biomesJson.text);
            }
        }

        public void ApplyRegionByIndex(int index)
        {
            if (_env == null || _env.biomes == null || index < 0 || index >= _env.biomes.Count) return;
            var b = _env.biomes[index];
            ApplyRegion(b);
        }

        public void ApplyRegion(BiomeDefModel b)
        {
            if (!config) return;
            config.seed = (int)(b.seed & 0x7fffffff);
            config.worldSizeMeters = b.size_meters;
            config.chunkSizeMeters = b.chunk_size_m;
            // Map biome spawn weights to ProcGen biomes for multiplier application
            config.useBiomeNoise = false; // fixed region theme
            config.biomes.Clear();
            var bd = new BiomeDef { name = b.name, min = 0, max = 1 };
            foreach (var sw in b.spawn_weights) bd.familyWeights.Add(new BiomeFamilyWeight { family = sw.family, densityMultiplier = sw.multiplier });
            config.biomes.Add(bd);
            // Rebuild world
            if (streamer) {
                // Force reload by disabling and enabling
                streamer.enabled = false; streamer.enabled = true;
            }
            if (player) player.position = Vector3.zero;
            // Quest trigger: entering biome
            _ = MMORPG.Client.Quests.QuestEventReporter.ReportAsync("enter_biome", b.name, 1);
        }
    }
}
