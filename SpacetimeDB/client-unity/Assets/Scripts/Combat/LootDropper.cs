using UnityEngine;

namespace MMORPG.Client.Combat
{
    [RequireComponent(typeof(Health))]
    public class LootDropper : MonoBehaviour
    {
        [Header("Gold Loot")] public int minGold = 1; public int maxGold = 5;
        [Header("XP Loot")] public int minXP = 3; public int maxXP = 10;
        [Header("Drops")]
        public bool useDropTables = true;
        [Tooltip("If assigned, overrides registry lookup")] public MMORPG.Client.Items.DropTable overrideTable;
        [Header("Pickup Visual")] public GameObject goldPickupPrefab;

        private Health _hp;

        void Awake()
        {
            _hp = GetComponent<Health>();
            _hp.OnDied += OnDied;
        }
        void OnDestroy()
        {
            if (_hp != null) _hp.OnDied -= OnDied;
        }

        void OnDied()
        {
            int gold = Random.Range(minGold, maxGold + 1);
            int xp = Random.Range(minXP, maxXP + 1);
            if (goldPickupPrefab)
            {
                if (gold > 0)
                {
                    var go = Instantiate(goldPickupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                    var p = go.GetComponent<MMORPG.Client.Items.Pickup>();
                    if (!p) p = go.AddComponent<MMORPG.Client.Items.Pickup>();
                    p.kind = MMORPG.Client.Items.Pickup.Kind.Gold; p.amount = gold;
                }
                if (xp > 0)
                {
                    var go2 = Instantiate(goldPickupPrefab, transform.position + new Vector3(0.4f,0.5f,0f), Quaternion.identity);
                    var p2 = go2.GetComponent<MMORPG.Client.Items.Pickup>();
                    if (!p2) p2 = go2.AddComponent<MMORPG.Client.Items.Pickup>();
                    p2.kind = MMORPG.Client.Items.Pickup.Kind.XP; p2.amount = xp;
                }
            }
            else
            {
                // Fallback: primitive cubes as pickups
                if (gold > 0)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = transform.position + Vector3.up * 0.5f;
                    go.transform.localScale = Vector3.one * 0.3f;
                    var col = go.GetComponent<Collider>(); col.isTrigger = true;
                    var p = go.AddComponent<MMORPG.Client.Items.Pickup>();
                    p.kind = MMORPG.Client.Items.Pickup.Kind.Gold; p.amount = gold;
                }
                if (xp > 0)
                {
                    var go2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go2.transform.position = transform.position + new Vector3(0.4f,0.5f,0f);
                    go2.transform.localScale = Vector3.one * 0.3f;
                    var col2 = go2.GetComponent<Collider>(); col2.isTrigger = true;
                    var p2 = go2.AddComponent<MMORPG.Client.Items.Pickup>();
                    p2.kind = MMORPG.Client.Items.Pickup.Kind.XP; p2.amount = xp;
                }
            }
            // Item drops via table
            if (useDropTables)
            {
                var table = overrideTable;
                if (!table)
                {
                    var id = GetComponent<MMORPG.Client.Enemies.EnemyIdentity>();
                    if (id) table = MMORPG.Client.Items.DropsRegistry.Find(id.Family, id.Tier);
                }
                if (table && table.entries != null && table.entries.Count > 0)
                {
                    int count = SamplePoisson(Mathf.Max(0f, table.avgDrops));
                    for (int i = 0; i < count; i++)
                    {
                        var ent = WeightedPick(table);
                        if (ent == null) continue;
                        if (Random.value > Mathf.Clamp01(ent.chance)) continue;
                        int amt = Random.Range(Mathf.Min(ent.min, ent.max), Mathf.Max(ent.min, ent.max) + 1);
                        if (amt <= 0) continue;
                        SpawnItem(ent.itemId, amt, i);
                    }
                }
            }
        }

        void SpawnItem(string itemId, int amount, int offsetIndex)
        {
            if (goldPickupPrefab)
            {
                var go = Instantiate(goldPickupPrefab, transform.position + new Vector3(0.3f * offsetIndex, 0.5f, 0f), Quaternion.identity);
                var p = go.GetComponent<MMORPG.Client.Items.Pickup>(); if (!p) p = go.AddComponent<MMORPG.Client.Items.Pickup>();
                p.kind = MMORPG.Client.Items.Pickup.Kind.Item; p.itemId = itemId; p.amount = amount;
            }
            else
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.transform.position = transform.position + new Vector3(0.3f * offsetIndex, 0.5f, 0f);
                go.transform.localScale = Vector3.one * 0.25f;
                var col = go.GetComponent<Collider>(); col.isTrigger = true;
                var p = go.AddComponent<MMORPG.Client.Items.Pickup>();
                p.kind = MMORPG.Client.Items.Pickup.Kind.Item; p.itemId = itemId; p.amount = amount;
            }
        }

        static MMORPG.Client.Items.DropEntry WeightedPick(MMORPG.Client.Items.DropTable table)
        {
            float sum = 0f; foreach (var e in table.entries) if (e != null && e.weight > 0f) sum += e.weight;
            if (sum <= 0f) return null;
            float r = Random.value * sum; float acc = 0f;
            foreach (var e in table.entries)
            {
                if (e == null || e.weight <= 0f) continue;
                acc += e.weight; if (r <= acc) return e;
            }
            return table.entries[table.entries.Count - 1];
        }

        static int SamplePoisson(float lambda)
        {
            if (lambda <= 0f) return 0;
            double L = System.Math.Exp(-lambda);
            int k = 0; double p = 1.0;
            do { k++; p *= Random.value; } while (p > L);
            return k - 1;
        }
    }
}
