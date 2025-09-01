using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.World
{
    [CreateAssetMenu(menuName = "MMORPG/ProcGen Config", fileName = "ProcGenConfig")]
    public class ProcGenConfig : ScriptableObject
    {
        [Header("World")]
        public int seed = 1337;
        [Tooltip("Total world size in meters (Unity units). 5000 = 5km square.")]
        public float worldSizeMeters = 5000f;
        [Tooltip("Chunk size in meters. 100m yields 50x50 chunks for 5km.")]
        public float chunkSizeMeters = 100f;
        [Tooltip("How many chunks in each direction to load around the player.")]
        public int viewDistanceChunks = 5;
        [Range(0.1f, 5f)] public float globalDensityScale = 1f;

        [Header("Ground Tiling")]
        public bool tileGround = true;
        public GameObject groundPrefab; // simple plane/tile prefab with proper scale

        [Header("Spawn Rules")] public List<SpawnRule> rules = new();

        [Header("Enemies")]
        [Tooltip("Optional local JSON (from design/enemies/data/enemies.json) to drive enemy spawns in Editor/standalone without SDK.")]
        public TextAsset enemiesJson;
        public List<EnemySpawnRule> enemyRules = new();

        [Header("Dungeon Portals")]
        [Tooltip("Optional local JSON (from design/bosses/data/bosses.json) for random dungeon portal destinations.")]
        public TextAsset bossesJson;
        public List<PortalRule> portalRules = new();
        [Tooltip("Per-biome portal themes")] public List<ThemedPortalRule> themedPortals = new();

        [Header("Biomes (Noise)")]
        public bool useBiomeNoise = false;
        [Tooltip("Perlin noise scale (lower = larger biomes)")] public float biomeNoiseScale = 0.001f;
        public List<BiomeDef> biomes = new();

        [Header("Activities & POIs")] public TextAsset activitiesJson; // design/activities/data/activities.json
        public List<PoiPrefabRule> poiPrefabs = new();
    }

    [System.Serializable]
    public class SpawnRule
    {
        public string name;
        public GameObject prefab;
        [Tooltip("Instances per hectare (10,000 m^2)")] public float densityPerHectare = 50f;
        [Tooltip("Minimum spacing between instances in meters")] public float minSpacing = 1.5f;
        [Header("Transform Variance")] public float minScale = 1f;
        public float maxScale = 1.5f;
        public bool alignToNormal = false;
        public bool randomYaw = true;
        [Header("Placement Filters")] public float minY = -1000f;
        public float maxY = 1000f;
        public float maxSlope = 60f; // not enforced without terrain normals
    }

    [System.Serializable]
    public class EnemySpawnRule
    {
        public string family = "Neon Raider";
        public int minTier = 1;
        public int maxTier = 5;
        public GameObject prefab;
        [Tooltip("Enemies per hectare (10,000 m^2)")] public float densityPerHectare = 5f;
        public float minSpacing = 3f;
    }

    [System.Serializable]
    public class PortalRule
    {
        public GameObject prefab;
        public float densityPerHectare = 0.2f;
        public float minSpacing = 20f;
    }

    [System.Serializable]
    public class BiomeDef
    {
        public string name = "Industrial";
        [Range(0f,1f)] public float min = 0f;
        [Range(0f,1f)] public float max = 0.5f;
        public List<BiomeFamilyWeight> familyWeights = new();
    }

    [System.Serializable]
    public class BiomeFamilyWeight
    {
        public string family;
        public float densityMultiplier = 1f;
    }

    [System.Serializable]
    public class ThemedPortalRule
    {
        public string biomeName;
        public GameObject portalPrefab;
        public GameObject vfxPrefab;
    }

    [System.Serializable]
    public class PoiPrefabRule
    {
        public string name;
        public GameObject prefab;
        public float minSpacing = 8f;
    }
}
