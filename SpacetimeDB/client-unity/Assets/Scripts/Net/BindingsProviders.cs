using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MMORPG.Client.Net
{
    public static class BindingsCaches
    {
        public static readonly List<BindingsBridge.WorldBossTypeRow> WbTypes = new();
        public static readonly List<BindingsBridge.WorldBossSpawnRow> WbSpawns = new();
        public static readonly List<BindingsBridge.ActiveBiomeEventRow> ActiveEvents = new();

        public static readonly List<BindingsBridge.ArmorSetRow> ArmorSets = new();
        public static readonly Dictionary<string, List<BindingsBridge.ArmorPieceRow>> ArmorPieces = new();
        public static readonly List<BindingsBridge.ArmorSetInfoRow> ArmorInfos = new();
        public static readonly List<BindingsBridge.ArmorPerkTypeRow> ArmorPerkTypes = new();

        public static readonly List<BindingsBridge.PlayerActiveBuffRow> PlayerBuffs = new();
        public static readonly List<BindingsBridge.PlayerStateRow> PlayerStates = new();
        public static readonly List<BindingsBridge.NpcRow> Npcs = new();
        public static readonly List<BindingsBridge.NpcStateRow> NpcStates = new();
        public static readonly List<BindingsBridge.NpcMemoryRow> NpcMemories = new();
        public static readonly List<BindingsBridge.QuestArchetypeRow> Quests = new();
        public static readonly List<BindingsBridge.QuestInstanceRow> QuestInstances = new();

        // SDK adapters can call these to update snapshots atomically
        public static void SetWorldBossTypes(IEnumerable<BindingsBridge.WorldBossTypeRow> rows)
        { WbTypes.Clear(); if (rows != null) WbTypes.AddRange(rows); }
        public static void SetWorldBossSpawns(IEnumerable<BindingsBridge.WorldBossSpawnRow> rows)
        { WbSpawns.Clear(); if (rows != null) WbSpawns.AddRange(rows); }
        public static void SetActiveEvents(IEnumerable<BindingsBridge.ActiveBiomeEventRow> rows)
        { ActiveEvents.Clear(); if (rows != null) ActiveEvents.AddRange(rows); }

        public static void SetArmorSets(IEnumerable<BindingsBridge.ArmorSetRow> rows)
        { ArmorSets.Clear(); if (rows != null) ArmorSets.AddRange(rows); }
        public static void SetArmorPieces(string setId, IEnumerable<BindingsBridge.ArmorPieceRow> rows)
        { ArmorPieces[setId ?? ""] = rows != null ? rows.ToList() : new List<BindingsBridge.ArmorPieceRow>(); }
        public static void SetArmorInfos(IEnumerable<BindingsBridge.ArmorSetInfoRow> rows)
        { ArmorInfos.Clear(); if (rows != null) ArmorInfos.AddRange(rows); }
        public static void SetArmorPerkTypes(IEnumerable<BindingsBridge.ArmorPerkTypeRow> rows)
        { ArmorPerkTypes.Clear(); if (rows != null) ArmorPerkTypes.AddRange(rows); }

        public static void SetPlayerBuffs(IEnumerable<BindingsBridge.PlayerActiveBuffRow> rows)
        { PlayerBuffs.Clear(); if (rows != null) PlayerBuffs.AddRange(rows); }
        public static void SetPlayerStates(IEnumerable<BindingsBridge.PlayerStateRow> rows)
        { PlayerStates.Clear(); if (rows != null) PlayerStates.AddRange(rows); }
        public static void SetNpcs(IEnumerable<BindingsBridge.NpcRow> rows)
        { Npcs.Clear(); if (rows != null) Npcs.AddRange(rows); }
        public static void SetNpcStates(IEnumerable<BindingsBridge.NpcStateRow> rows)
        { NpcStates.Clear(); if (rows != null) NpcStates.AddRange(rows); }
        public static void SetNpcMemories(IEnumerable<BindingsBridge.NpcMemoryRow> rows)
        { NpcMemories.Clear(); if (rows != null) NpcMemories.AddRange(rows); }
        public static void SetQuestArchetypes(IEnumerable<BindingsBridge.QuestArchetypeRow> rows)
        { Quests.Clear(); if (rows != null) Quests.AddRange(rows); }
        public static void SetQuestInstances(IEnumerable<BindingsBridge.QuestInstanceRow> rows)
        { QuestInstances.Clear(); if (rows != null) QuestInstances.AddRange(rows); }
    }

    // Attach once in bootstrap to connect BindingsBridge to caches
    public class BindingsProviders : MonoBehaviour
    {
        void Awake()
        {
            BindingsBridge.Provide(
                classes: null,
                abilities: null,
                perks: null,
                enemies: null,
                wbTypes: () => BindingsCaches.WbTypes,
                wbSpawns: () => BindingsCaches.WbSpawns,
                activeEvents: () => BindingsCaches.ActiveEvents,
                armorSets: () => BindingsCaches.ArmorSets,
                armorPieces: (setId) => BindingsCaches.ArmorPieces.TryGetValue(setId ?? "", out var list) ? list : System.Array.Empty<BindingsBridge.ArmorPieceRow>(),
                armorSetInfos: () => BindingsCaches.ArmorInfos,
                armorPerkTypes: () => BindingsCaches.ArmorPerkTypes,
                playerBuffs: () => BindingsCaches.PlayerBuffs,
                playerStates: () => BindingsCaches.PlayerStates,
                npcs: () => BindingsCaches.Npcs,
                npcStates: () => BindingsCaches.NpcStates,
                npcMemories: () => BindingsCaches.NpcMemories,
                questArchetypes: () => BindingsCaches.Quests,
                questInstances: () => BindingsCaches.QuestInstances
            );
        }
    }
}
