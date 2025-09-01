using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MMORPG.Client.Combat;

namespace MMORPG.Client.Items
{
    [Serializable] public class ConsumableEffectJson { public string kind; public float amount; public float duration_s; }
    [Serializable] public class ConsumableJson
    {
        public string id; public string name; public int tier; public string category; public float cooldown_s; public string stack; public List<ConsumableEffectJson> effects;
    }
    [Serializable] public class ConsumablesEnvelope { public List<ConsumableJson> consumables; }

    public static class ConsumableCatalog
    {
        static ConsumablesEnvelope _data;
        public static void LoadFromDesign()
        {
            try
            {
                var root = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "SpacetimeDB", "design", "consumables", "data", "consumables.json"));
                if (File.Exists(root))
                {
                    var json = File.ReadAllText(root);
                    _data = JsonUtility.FromJson<ConsumablesEnvelope>(json);
                }
            } catch { _data = null; }
            if (_data == null || _data.consumables == null) _data = new ConsumablesEnvelope { consumables = new List<ConsumableJson>() };
        }

        public static List<ConsumableJson> AllRaw() => _data?.consumables ?? new List<ConsumableJson>();

        public static List<Consumable> All()
        {
            var outList = new List<Consumable>();
            var raw = AllRaw();
            foreach (var c in raw)
            {
                var cc = new Consumable
                {
                    Id = c.id,
                    Name = c.name,
                    Tier = c.tier,
                    Category = c.category,
                    CooldownS = c.cooldown_s,
                    StackMode = ParseStack(c.stack),
                };
                if (c.effects != null)
                {
                    foreach (var e in c.effects)
                    {
                        if (Enum.TryParse<BuffKind>(e.kind, out var k))
                        {
                            cc.Effects.Add(new BuffEffect { kind = k, amount = e.amount, durationS = e.duration_s });
                        }
                    }
                }
                outList.Add(cc);
            }
            return outList;
        }

        static BuffStackMode ParseStack(string s)
        {
            return s == "StackDiminishing" ? BuffStackMode.StackDiminishing : (s == "Unique" ? BuffStackMode.Unique : BuffStackMode.RefreshDuration);
        }
    }
}
