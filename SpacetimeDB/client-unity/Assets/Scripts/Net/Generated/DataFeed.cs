using System.Collections.Generic;

namespace MMORPG.Client.Generated
{
    // Simple in-memory feed your generated SDK can write to.
    // SpacetimeDbSubscriptions reads these lists and maps into BindingsCaches.
    public static class DataFeed
    {
        public static readonly List<PlayerStateRow> PlayerStates = new();
        public static readonly List<ActiveBiomeEventRow> ActiveBiomeEvents = new();
        public static readonly List<WorldBossTypeRow> WorldBossTypes = new();
        public static readonly List<WorldBossSpawnRow> WorldBossSpawns = new();
        public static readonly List<ArmorSetRow> ArmorSets = new();
        public static readonly List<ArmorSetInfoRow> ArmorSetInfos = new();
        public static readonly List<ArmorPerkTypeRow> ArmorPerkTypes = new();
        public static readonly List<NpcRow> Npcs = new();
        public static readonly List<NpcStateRow> NpcStates = new();
        public static readonly List<NpcMemoryRow> NpcMemories = new();
        public static readonly List<QuestArchetypeRow> QuestArchetypes = new();
        public static readonly List<QuestInstanceRow> QuestInstances = new();

        public static void SetPlayerStates(IEnumerable<PlayerStateRow> rows)
        { PlayerStates.Clear(); if (rows != null) PlayerStates.AddRange(rows); }
        public static void SetActiveBiomeEvents(IEnumerable<ActiveBiomeEventRow> rows)
        { ActiveBiomeEvents.Clear(); if (rows != null) ActiveBiomeEvents.AddRange(rows); }
        public static void SetWorldBossTypes(IEnumerable<WorldBossTypeRow> rows)
        { WorldBossTypes.Clear(); if (rows != null) WorldBossTypes.AddRange(rows); }
        public static void SetWorldBossSpawns(IEnumerable<WorldBossSpawnRow> rows)
        { WorldBossSpawns.Clear(); if (rows != null) WorldBossSpawns.AddRange(rows); }
        public static void SetArmorSets(IEnumerable<ArmorSetRow> rows)
        { ArmorSets.Clear(); if (rows != null) ArmorSets.AddRange(rows); }
        public static void SetArmorSetInfos(IEnumerable<ArmorSetInfoRow> rows)
        { ArmorSetInfos.Clear(); if (rows != null) ArmorSetInfos.AddRange(rows); }
        public static void SetArmorPerkTypes(IEnumerable<ArmorPerkTypeRow> rows)
        { ArmorPerkTypes.Clear(); if (rows != null) ArmorPerkTypes.AddRange(rows); }
        public static void SetNpcs(IEnumerable<NpcRow> rows)
        { Npcs.Clear(); if (rows != null) Npcs.AddRange(rows); }
        public static void SetNpcStates(IEnumerable<NpcStateRow> rows)
        { NpcStates.Clear(); if (rows != null) NpcStates.AddRange(rows); }
        public static void SetNpcMemories(IEnumerable<NpcMemoryRow> rows)
        { NpcMemories.Clear(); if (rows != null) NpcMemories.AddRange(rows); }
        public static void SetQuestArchetypes(IEnumerable<QuestArchetypeRow> rows)
        { QuestArchetypes.Clear(); if (rows != null) QuestArchetypes.AddRange(rows); }
        public static void SetQuestInstances(IEnumerable<QuestInstanceRow> rows)
        { QuestInstances.Clear(); if (rows != null) QuestInstances.AddRange(rows); }
    }
}
