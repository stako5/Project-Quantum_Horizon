using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Combat
{
    public class BuffManager : MonoBehaviour
    {
        private readonly Dictionary<string, BuffInstance> _active = new(); // key = def.id

        public System.Action OnBuffsChanged;

        void Update()
        {
            List<string> toRemove = null;
            foreach (var kv in _active)
            {
                kv.Value.remainingS -= Time.deltaTime;
                if (kv.Value.remainingS <= 0f)
                {
                    (toRemove ??= new List<string>()).Add(kv.Key);
                }
            }
            if (toRemove != null)
            {
                foreach (var k in toRemove) _active.Remove(k);
                OnBuffsChanged?.Invoke();
            }
        }

        public void Apply(BuffDefinition def)
        {
            if (_active.TryGetValue(def.id, out var inst))
            {
                switch (def.stackMode)
                {
                    case BuffStackMode.Unique:
                    case BuffStackMode.RefreshDuration:
                        inst.remainingS = Mathf.Max(inst.remainingS, GetMaxDuration(def));
                        break;
                    case BuffStackMode.StackDiminishing:
                        inst.stacks = Mathf.Min(inst.stacks + 1, 5);
                        inst.remainingS = Mathf.Max(inst.remainingS, GetMaxDuration(def));
                        break;
                }
            }
            else
            {
                _active[def.id] = new BuffInstance(def);
            }
            OnBuffsChanged?.Invoke();
            // Immediate one-off shield application
            float shield = 0f;
            foreach (var e in def.effects) if (e.kind == BuffKind.ShieldFlat) shield += e.amount;
            if (shield > 0f)
            {
                var shieldable = GetComponent<IShieldConsumer>();
                if (shieldable != null) shieldable.ApplyShield(shield);
            }
        }

        public IEnumerable<BuffInstance> ActiveBuffs() => _active.Values;

        public StatsModifiers GetModifiers()
        {
            var m = StatsModifiers.Zero;
            foreach (var kv in _active)
            {
                var inst = kv.Value;
                foreach (var e in inst.def.effects)
                {
                    if (inst.remainingS <= 0f || e.durationS <= 0f) continue;
                    float amt = e.amount;
                    if (inst.def.stackMode == BuffStackMode.StackDiminishing && inst.stacks > 1)
                    {
                        // Diminishing returns: sum_{i=0..n-1} amt * 0.6^i
                        float sum = 0f; float cur = amt; for (int i=0;i<inst.stacks;i++){ sum += cur; cur *= 0.6f; } amt = sum;
                    }
                    switch (e.kind)
                    {
                        case BuffKind.DamageMult: m.damageMult *= 1f + amt; break;
                        case BuffKind.DefenseMult: m.defenseMult *= 1f + amt; break;
                        case BuffKind.MoveSpeedMult: m.moveSpeedMult *= 1f + amt; break;
                        case BuffKind.CritChanceAdd: m.critChanceAdd += amt; break;
                        case BuffKind.CritDamageMult: m.critDamageMult *= 1f + amt; break;
                        case BuffKind.CooldownReductionMult: m.cooldownReductionMult *= 1f - Mathf.Clamp01(amt); break; // amt=0.1 => 10% CDR
                        case BuffKind.StaminaRegenMult: m.staminaRegenMult *= 1f + amt; break;
                        case BuffKind.HealthRegenFlat: m.healthRegenFlat += amt; break;
                        case BuffKind.LifeStealPct: m.lifeStealPct += amt; break;
                        case BuffKind.ResistFire: m.resistFire = CombineResist(m.resistFire, amt); break;
                        case BuffKind.ResistIce: m.resistIce = CombineResist(m.resistIce, amt); break;
                        case BuffKind.ResistShock: m.resistShock = CombineResist(m.resistShock, amt); break;
                        case BuffKind.ResistVoid: m.resistVoid = CombineResist(m.resistVoid, amt); break;
                        case BuffKind.DropRateMult: m.dropRateMult *= 1f + amt; break;
                        case BuffKind.XPGainMult: m.xpGainMult *= 1f + amt; break;
                        case BuffKind.GoldFindMult: m.goldFindMult *= 1f + amt; break;
                        case BuffKind.ShieldFlat: default: break; // applied on consume
                    }
                }
            }
            // Clamp reasonable caps
            m.resistFire = Mathf.Clamp(m.resistFire, 0f, 0.75f);
            m.resistIce = Mathf.Clamp(m.resistIce, 0f, 0.75f);
            m.resistShock = Mathf.Clamp(m.resistShock, 0f, 0.75f);
            m.resistVoid = Mathf.Clamp(m.resistVoid, 0f, 0.75f);
            m.critChanceAdd = Mathf.Clamp(m.critChanceAdd, 0f, 0.75f);
            m.lifeStealPct = Mathf.Clamp(m.lifeStealPct, 0f, 0.20f);
            return m;
        }

        static float GetMaxDuration(BuffDefinition def)
        {
            float max = 0f; foreach (var e in def.effects) if (e.durationS > max) max = e.durationS; return max;
        }

        static float CombineResist(float current, float add)
        {
            // 1 - (1-a)*(1-b)
            float combined = 1f - (1f - current) * (1f - add);
            return combined;
        }

        // Replace local state with a server snapshot (idempotent).
        // Each tuple: (definition, remainingSeconds, stacks)
        public void SetServerSnapshot(System.Collections.Generic.IEnumerable<(BuffDefinition def, float remainingS, int stacks)> snapshot)
        {
            _active.Clear();
            if (snapshot != null)
            {
                foreach (var t in snapshot)
                {
                    var inst = new BuffInstance(t.def) { remainingS = Mathf.Max(0f, t.remainingS), stacks = Mathf.Max(1, t.stacks) };
                    _active[t.def.id] = inst;
                }
            }
            OnBuffsChanged?.Invoke();
        }
    }

    public interface IShieldConsumer { void ApplyShield(float amount); }
}
