using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Combat
{
    [Serializable]
    public class BuffEffect
    {
        public BuffKind kind;
        public float amount; // percent (0.15 = +15%) or flat depending on kind
        public float durationS;
    }

    [Serializable]
    public class BuffDefinition
    {
        public string id;
        public string name;
        public int tier; // 1..5
        public BuffStackMode stackMode;
        public List<BuffEffect> effects = new List<BuffEffect>();
    }

    public class BuffInstance
    {
        public BuffDefinition def;
        public float remainingS;
        public int stacks;
        public BuffInstance(BuffDefinition def)
        {
            this.def = def;
            this.remainingS = MaxEffectDuration(def);
            this.stacks = 1;
        }
        static float MaxEffectDuration(BuffDefinition def)
        {
            float max = 0f; foreach (var e in def.effects) if (e.durationS > max) max = e.durationS; return max;
        }
    }

    [Serializable]
    public struct StatsModifiers
    {
        public float damageMult;      // multiplicative
        public float defenseMult;     // multiplicative
        public float moveSpeedMult;   // multiplicative
        public float critChanceAdd;   // additive (absolute)
        public float critDamageMult;  // multiplicative
        public float cooldownReductionMult; // multiplicative
        public float staminaRegenMult; // multiplicative
        public float healthRegenFlat; // flat per second
        public float lifeStealPct;    // additive
        public float shieldFlat;      // flat shield amount (applied once on consume)
        public float resistFire;      // [0..0.75] combined via 1 - prod(1-r)
        public float resistIce;
        public float resistShock;
        public float resistVoid;
        public float dropRateMult;
        public float xpGainMult;
        public float goldFindMult;

        public static StatsModifiers Zero => new StatsModifiers
        {
            damageMult = 1f,
            defenseMult = 1f,
            moveSpeedMult = 1f,
            critChanceAdd = 0f,
            critDamageMult = 1f,
            cooldownReductionMult = 1f,
            staminaRegenMult = 1f,
            healthRegenFlat = 0f,
            lifeStealPct = 0f,
            shieldFlat = 0f,
            resistFire = 0f,
            resistIce = 0f,
            resistShock = 0f,
            resistVoid = 0f,
            dropRateMult = 1f,
            xpGainMult = 1f,
            goldFindMult = 1f,
        };
    }
}

