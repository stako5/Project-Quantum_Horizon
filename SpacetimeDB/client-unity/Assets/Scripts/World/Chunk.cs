using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.World
{
    public class Chunk : MonoBehaviour
    {
        public Vector2Int Coord { get; private set; }
        public Rect BoundsXZ { get; private set; }

        private List<GameObject> _spawned = new();

        public void Build(ProcGenConfig config, Vector2Int coord)
        {
            Clear();
            Coord = coord;
            float cs = config.chunkSizeMeters;
            float x = coord.x * cs;
            float z = coord.y * cs;
            BoundsXZ = new Rect(x, z, cs, cs);
            transform.position = new Vector3(x + cs * 0.5f, 0f, z + cs * 0.5f);
            name = $"Chunk_{coord.x}_{coord.y}";

            // Ground tiling
            if (config.tileGround && config.groundPrefab)
            {
                var ground = Instantiate(config.groundPrefab, transform);
                ground.transform.position = new Vector3(BoundsXZ.center.x, 0f, BoundsXZ.center.y);
                ground.transform.localScale = new Vector3(cs, 1f, cs);
                _spawned.Add(ground);
            }

            var rand = new System.Random(Hash(config.seed, coord.x, coord.y));
            float areaM2 = cs * cs;
            float areaHa = areaM2 / 10000f;

            foreach (var rule in config.rules)
            {
                if (!rule?.prefab) continue;
                float lambda = Mathf.Max(0f, rule.densityPerHectare * areaHa * config.globalDensityScale);
                int count = SamplePoisson(lambda, rand);
                Scatter(rule, count, rand);
            }

            // Enemy spawns (local JSON source only; swap to SDK-backed provider in bootstrap)
            if (config.enemyRules != null && config.enemyRules.Count > 0)
            {
                if (config.enemiesJson)
                {
                    MMORPG.Client.Enemies.EnemyCatalog.Use(new MMORPG.Client.Enemies.LocalJsonEnemySource(config.enemiesJson));
                }
                // Determine biome multiplier map for this chunk (if enabled)
                System.Collections.Generic.Dictionary<string, float> biomeMult = null;
                string activeBiome = null;
                if (config.useBiomeNoise && config.biomes != null && config.biomes.Count > 0)
                {
                    float nx = (BoundsXZ.center.x + config.seed * 0.123f) * config.biomeNoiseScale;
                    float ny = (BoundsXZ.center.y + config.seed * 0.321f) * config.biomeNoiseScale;
                    float n = Mathf.PerlinNoise(nx, ny);
                    foreach (var b in config.biomes)
                    {
                        if (n >= Mathf.Min(b.min, b.max) && n <= Mathf.Max(b.min, b.max))
                        {
                            biomeMult = new System.Collections.Generic.Dictionary<string, float>();
                            foreach (var fw in b.familyWeights)
                                if (fw != null && !string.IsNullOrEmpty(fw.family)) biomeMult[fw.family] = fw.densityMultiplier;
                            activeBiome = b.name;
                            break;
                        }
                    }
                }
                if (!config.useBiomeNoise && config.biomes != null && config.biomes.Count > 0)
                    activeBiome = config.biomes[0].name;
                foreach (var er in config.enemyRules)
                {
                    if (!er.prefab) continue;
                    var pool = MMORPG.Client.Enemies.EnemyCatalog.Filter(er.family, er.minTier, er.maxTier);
                    if (pool.Count == 0) continue;
                    float mult = 1f;
                    if (biomeMult != null && biomeMult.TryGetValue(er.family, out var m)) mult = m;
                    float lambda = Mathf.Max(0f, er.densityPerHectare * areaHa * config.globalDensityScale * mult);
                    int count = SamplePoisson(lambda, rand);
                    ScatterEnemies(pool, er, count, rand);
                }
                // Portals
                if (config.portalRules != null && config.portalRules.Count > 0 && config.bossesJson)
                {
                    // Average lambda across all portal rules (each spawns one portal instance)
                    float portalLambda = 0f;
                    foreach (var pr in config.portalRules) portalLambda += Mathf.Max(0f, pr.densityPerHectare * areaHa * config.globalDensityScale);
                    int pcount = SamplePoisson(portalLambda, rand);
                    ScatterPortals(config, pcount, rand, activeBiome);
                }

                // POIs/Activities (non-repetitive; varied per biome)
                if (config.activitiesJson)
                {
                    MMORPG.Client.World.ActivitySpawner.SpawnPOIs(config, BoundsXZ, activeBiome ?? "", transform, rand);
                }
            }
        }

        void Scatter(SpawnRule rule, int count, System.Random rand)
        {
            const int maxAttemptsPerInstance = 12;
            var placed = new List<Vector2>();
            for (int i = 0; i < count; i++)
            {
                bool success = false;
                for (int attempt = 0; attempt < maxAttemptsPerInstance && !success; attempt++)
                {
                    float rx = (float)rand.NextDouble();
                    float rz = (float)rand.NextDouble();
                    float px = BoundsXZ.xMin + rx * BoundsXZ.width;
                    float pz = BoundsXZ.yMin + rz * BoundsXZ.height;
                    var p2 = new Vector2(px, pz);
                    success = true;
                    float minSpacing = Mathf.Max(0f, rule.minSpacing);
                    if (minSpacing > 0f)
                    {
                        foreach (var prev in placed)
                        {
                            if (Vector2.Distance(prev, p2) < minSpacing) { success = false; break; }
                        }
                    }
                    if (success)
                    {
                        placed.Add(p2);
                        var go = Instantiate(rule.prefab, transform);
                        float y = 0f; // replace with terrain sampling if present
                        go.transform.position = new Vector3(px, y, pz);
                        if (rule.randomYaw)
                        {
                            float yaw = (float)rand.NextDouble() * 360f;
                            go.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
                        }
                        float s = Mathf.Lerp(rule.minScale, rule.maxScale, (float)rand.NextDouble());
                        go.transform.localScale = new Vector3(s, s, s);
                        _spawned.Add(go);
                    }
                }
            }
        }

        void ScatterEnemies(System.Collections.Generic.List<MMORPG.Client.Enemies.EnemyTypeModel> pool, EnemySpawnRule rule, int count, System.Random rand)
        {
            const int maxAttemptsPerInstance = 12;
            var placed = new System.Collections.Generic.List<Vector2>();
            for (int i = 0; i < count; i++)
            {
                bool success = false;
                for (int attempt = 0; attempt < maxAttemptsPerInstance && !success; attempt++)
                {
                    float rx = (float)rand.NextDouble();
                    float rz = (float)rand.NextDouble();
                    float px = BoundsXZ.xMin + rx * BoundsXZ.width;
                    float pz = BoundsXZ.yMin + rz * BoundsXZ.height;
                    var p2 = new Vector2(px, pz);
                    success = true;
                    float minSpacing = Mathf.Max(0f, rule.minSpacing);
                    if (minSpacing > 0f)
                    {
                        foreach (var prev in placed)
                        {
                            if (Vector2.Distance(prev, p2) < minSpacing) { success = false; break; }
                        }
                    }
                    if (success)
                    {
                        placed.Add(p2);
                        var type = pool[(int)(rand.NextDouble() * pool.Count) % pool.Count];
                        var go = Instantiate(rule.prefab, transform);
                        go.transform.position = new Vector3(px, 0f, pz);
                        var id = go.AddComponent<MMORPG.Client.Enemies.EnemyIdentity>();
                        id.TypeId = type.id; id.DisplayName = type.name; id.Family = type.family; id.Tier = type.tier;
                        id.Role = type.role; id.Size = type.size; id.BaseHP = type.hp; id.BaseDamage = type.damage; id.BaseSpeed = type.speed;
                        // Parse stage from tags (e.g., "stage:Adult")
                        if (type.tags != null)
                        {
                            foreach (var tag in type.tags)
                            {
                                if (!string.IsNullOrEmpty(tag) && tag.StartsWith("stage:")) { id.Stage = tag.Substring(6); break; }
                            }
                        }
                        // Ensure Health + death watcher for quest triggers
                        if (!go.GetComponent<MMORPG.Client.Combat.Health>()) go.AddComponent<MMORPG.Client.Combat.Health>();
                        if (!go.GetComponent<MMORPG.Client.Combat.DamageNumberSource>()) go.AddComponent<MMORPG.Client.Combat.DamageNumberSource>();
                        if (!go.GetComponent<MMORPG.Client.Enemies.EnemyDeathWatcher>()) go.AddComponent<MMORPG.Client.Enemies.EnemyDeathWatcher>();
                        if (!go.GetComponent<MMORPG.Client.Combat.LootDropper>()) go.AddComponent<MMORPG.Client.Combat.LootDropper>();
                        // Personality + brain bootstrap
                        var pers = go.GetComponent<MMORPG.Client.Enemies.EnemyPersonality>();
                        if (!pers) pers = go.AddComponent<MMORPG.Client.Enemies.EnemyPersonality>();
                        if (type.tags != null)
                        {
                            foreach (var tag in type.tags)
                            {
                                // Parse O/C/E tags like "O:0.8"
                                pers.ApplyTag(tag);
                            }
                        }
                        // Ensure targeting + utility brain for personality-driven actions
                        if (!go.GetComponent<MMORPG.Client.AI.UtilityAgent>()) go.AddComponent<MMORPG.Client.AI.UtilityAgent>();
                        if (!go.GetComponent<MMORPG.Client.Enemies.EnemyTargeting>()) go.AddComponent<MMORPG.Client.Enemies.EnemyTargeting>();
                        if (!go.GetComponent<MMORPG.Client.Enemies.EnemyBrain>()) go.AddComponent<MMORPG.Client.Enemies.EnemyBrain>();
                        _spawned.Add(go);
                    }
                }
            }
        }

        // Dungeon portals
        void ScatterPortals(ProcGenConfig config, int count, System.Random rand, string activeBiome)
        {
            var bosses = MMORPG.Client.Dungeon.BossCatalog.FromJson(config.bossesJson);
            if (bosses.Count == 0) return;
            const int maxAttemptsPerInstance = 12;
            var placed = new System.Collections.Generic.List<Vector2>();
            for (int i = 0; i < count; i++)
            {
                bool success = false;
                for (int attempt = 0; attempt < maxAttemptsPerInstance && !success; attempt++)
                {
                    float rx = (float)rand.NextDouble();
                    float rz = (float)rand.NextDouble();
                    float px = BoundsXZ.xMin + rx * BoundsXZ.width;
                    float pz = BoundsXZ.yMin + rz * BoundsXZ.height;
                    var p2 = new Vector2(px, pz);
                    success = true;
                    float minSpacing = 15f;
                    foreach (var prev in placed)
                    {
                        if (Vector2.Distance(prev, p2) < minSpacing) { success = false; break; }
                    }
                    if (success)
                    {
                        placed.Add(p2);
                        var boss = bosses[(int)(rand.NextDouble() * bosses.Count) % bosses.Count];
                        GameObject chosenPortal = null; GameObject chosenVfx = null;
                        // Themed first
                        if (config.themedPortals != null)
                        {
                            foreach (var t in config.themedPortals)
                            {
                                if (!string.IsNullOrEmpty(activeBiome) && t.biomeName == activeBiome && t.portalPrefab) { chosenPortal = t.portalPrefab; chosenVfx = t.vfxPrefab; break; }
                            }
                        }
                        // Fallback generic portal rules
                        if (!chosenPortal && config.portalRules != null)
                        {
                            foreach (var pr in config.portalRules)
                            {
                                if (pr.prefab) { chosenPortal = pr.prefab; break; }
                            }
                        }
                        if (!chosenPortal) continue;
                        var portal = Instantiate(chosenPortal, transform);
                        portal.transform.position = new Vector3(px, 0f, pz);
                        var comp = portal.AddComponent<MMORPG.Client.Dungeon.DungeonPortal>();
                        comp.TargetBossId = boss.id; comp.TargetBossName = boss.name;
                        if (chosenVfx) { var v = Instantiate(chosenVfx, portal.transform); v.transform.localPosition = Vector3.zero; }
                    }
                }
            }
        }

        public void Clear()
        {
            foreach (var go in _spawned)
            {
                if (go) Destroy(go);
            }
            _spawned.Clear();
        }

        static int Hash(int seed, int x, int y)
        {
            unchecked
            {
                int h = 17;
                h = h * 31 + seed;
                h = h * 31 + x;
                h = h * 31 + y;
                return h;
            }
        }

        static int SamplePoisson(float lambda, System.Random rand)
        {
            if (lambda <= 0f) return 0;
            double L = Math.Exp(-lambda);
            int k = 0;
            double p = 1.0;
            do
            {
                k++;
                p *= rand.NextDouble();
            } while (p > L);
            return k - 1;
        }
    }
}
