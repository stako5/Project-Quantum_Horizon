using System;
using UnityEngine;

namespace MMORPG.Client.Combat
{
    public enum BuffStackMode { Unique, RefreshDuration, StackDiminishing }

    public enum BuffKind
    {
        DamageMult,
        DefenseMult,
        MoveSpeedMult,
        CritChanceAdd,
        CritDamageMult,
        CooldownReductionMult,
        StaminaRegenMult,
        HealthRegenFlat,
        LifeStealPct,
        ShieldFlat,
        ResistFire,
        ResistIce,
        ResistShock,
        ResistVoid,
        DropRateMult,
        XPGainMult,
        GoldFindMult,
    }
}

