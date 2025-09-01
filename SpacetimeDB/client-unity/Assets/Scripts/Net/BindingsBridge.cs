using System;
using System.Collections.Generic;

namespace MMORPG.Client.Net
{
    // Bridge to plug in generated SpacetimeDB bindings without hardcoding namespaces.
    // After you run `spacetime generate`, call BindingsBridge.Provide(...) once at startup.
    public static class BindingsBridge
    {
        public class ClassCatalogRow { public string ClassName; public long UpdatedAtMicros; }
        public class AbilityRow
        {
            public string Id; public string ClassName; public string Name; public string Branch; public byte Tier;
            public string Kind; public string ResourceType; public float? ResourceCost; public float? CooldownS; public string Description;
        }
        public class PerkRow
        {
            public string Id; public string ClassName; public string Name; public string Tier; public string TagsCsv; public string Description;
            public string RollStat; public float? RollMin; public float? RollMax;
        }
        public class EnemyTypeRow
        {
            public string Id; public string Name; public string Family; public byte Tier; public string Role; public string Element; public string Size; public string Movement; public string Armor;
            public uint Hp; public uint Damage; public float Speed; public string AbilitiesCsv; public string TagsCsv; public string Description;
        }
        public class WorldBossTypeRow
        {
            public string Id; public string Name; public string Biome; public byte Tier; public string EnvJson; public byte MinNgPlus; public string Description;
        }
        public class WorldBossSpawnRow
        {
            public ulong Id; public string BossId; public byte RegionId; public float X; public float Z; public bool Alive;
        }
        public class ActiveBiomeEventRow
        {
            public ulong Id; public string Biome; public string Name; public byte RegionId; public float X; public float Z; public long StartedAt; public long ExpiresAt;
        }
        public class ArmorSetRow { public string Id; public string Name; }
        public class ArmorPieceRow { public ulong Id; public string SetId; public string Slot; public string Name; public string Bonus; }
        public class ArmorSetInfoRow { public string SetId; public string Category; public string FactionName; public string EnemyFamily; public uint? RepRequired; }
        public class ArmorPerkTypeRow { public string Id; public string Name; public string Stat; public float Min; public float Max; public string Rarity; }
        public class PlayerActiveBuffRow { public string BuffId; public byte Tier; public uint Stacks; public string StackMode; public long StartedAt; public long ExpiresAt; }
        public class PlayerStateRow { public string Identity; public byte RegionId; public float X; public float Y; public float Z; public float YawDeg; public long UpdatedAt; }
        public class NpcRow { public ulong Id; public string Name; public string Faction; public string Role; public string Personality; public byte HomeRegion; public string Bio; }
        public class NpcStateRow { public ulong NpcId; public byte RegionId; public float X; public float Y; public float Z; public string Mood; public string Activity; public long UpdatedAt; }
        public class NpcMemoryRow { public ulong Id; public ulong NpcId; public long At; public string Kind; public string Description; public byte Importance; }
        public class QuestArchetypeRow { public string Id; public string Name; public string Description; public string Category; public string EventType; public string EventParam; public uint RequiredCount; public uint? TimeLimitS; public string ChainNextId; public uint RewardCredits; public string RewardItem; public uint? RewardItemQty; }
        public class QuestInstanceRow { public ulong Id; public string ArchetypeId; public ulong NpcId; public string PlayerIdentity; public string State; public long CreatedAt; public long? ExpiresAt; public uint CurrentCount; }

        public delegate IEnumerable<ClassCatalogRow> ClassesProvider();
        public delegate IEnumerable<AbilityRow> AbilitiesProvider(string className);
        public delegate IEnumerable<PerkRow> PerksProvider(string className);
        public delegate IEnumerable<EnemyTypeRow> EnemiesProvider();
        public delegate IEnumerable<WorldBossTypeRow> WorldBossTypesProvider();
        public delegate IEnumerable<WorldBossSpawnRow> WorldBossSpawnsProvider();
        public delegate IEnumerable<ActiveBiomeEventRow> ActiveBiomeEventsProvider();
        public delegate IEnumerable<ArmorSetRow> ArmorSetsProvider();
        public delegate IEnumerable<ArmorPieceRow> ArmorPiecesProvider(string setId);
        public delegate IEnumerable<ArmorSetInfoRow> ArmorSetInfosProvider();
        public delegate IEnumerable<ArmorPerkTypeRow> ArmorPerkTypesProvider();
        public delegate IEnumerable<PlayerActiveBuffRow> PlayerActiveBuffsProvider();
        public delegate IEnumerable<PlayerStateRow> PlayerStatesProvider();
        public delegate IEnumerable<NpcRow> NpcsProvider();
        public delegate IEnumerable<NpcStateRow> NpcStatesProvider();
        public delegate IEnumerable<NpcMemoryRow> NpcMemoriesProvider();
        public delegate IEnumerable<QuestArchetypeRow> QuestArchetypesProvider();
        public delegate IEnumerable<QuestInstanceRow> QuestInstancesProvider();

        private static ClassesProvider _classes;
        private static AbilitiesProvider _abilities;
        private static PerksProvider _perks;
        private static EnemiesProvider _enemies;
        private static WorldBossTypesProvider _wbTypes;
        private static WorldBossSpawnsProvider _wbSpawns;
        private static ActiveBiomeEventsProvider _activeEvents;
        private static ArmorSetsProvider _armorSets;
        private static ArmorPiecesProvider _armorPieces;
        private static ArmorSetInfosProvider _armorSetInfos;
        private static ArmorPerkTypesProvider _armorPerkTypes;
        private static PlayerActiveBuffsProvider _playerBuffs;
        private static PlayerStatesProvider _playerStates;
        private static NpcsProvider _npcs;
        private static NpcStatesProvider _npcStates;
        private static NpcMemoriesProvider _npcMemories;
        private static QuestArchetypesProvider _questArchetypes;
        private static QuestInstancesProvider _questInstances;

        public static void Provide(ClassesProvider classes, AbilitiesProvider abilities, PerksProvider perks, EnemiesProvider enemies = null, WorldBossTypesProvider wbTypes = null, WorldBossSpawnsProvider wbSpawns = null, ActiveBiomeEventsProvider activeEvents = null, ArmorSetsProvider armorSets = null, ArmorPiecesProvider armorPieces = null, ArmorSetInfosProvider armorSetInfos = null, ArmorPerkTypesProvider armorPerkTypes = null, PlayerActiveBuffsProvider playerBuffs = null, PlayerStatesProvider playerStates = null, NpcsProvider npcs = null, NpcStatesProvider npcStates = null, NpcMemoriesProvider npcMemories = null, QuestArchetypesProvider questArchetypes = null, QuestInstancesProvider questInstances = null)
        {
            _classes = classes; _abilities = abilities; _perks = perks; _enemies = enemies; _wbTypes = wbTypes; _wbSpawns = wbSpawns; _activeEvents = activeEvents; _armorSets = armorSets; _armorPieces = armorPieces; _armorSetInfos = armorSetInfos; _armorPerkTypes = armorPerkTypes; _playerBuffs = playerBuffs; _playerStates = playerStates; _npcs = npcs; _npcStates = npcStates; _npcMemories = npcMemories; _questArchetypes = questArchetypes; _questInstances = questInstances;
        }

        public static IEnumerable<ClassCatalogRow> GetClasses() => _classes != null ? _classes() : Array.Empty<ClassCatalogRow>();
        public static IEnumerable<AbilityRow> GetAbilities(string className) => _abilities != null ? _abilities(className) : Array.Empty<AbilityRow>();
        public static IEnumerable<PerkRow> GetPerks(string className) => _perks != null ? _perks(className) : Array.Empty<PerkRow>();
        public static IEnumerable<EnemyTypeRow> GetEnemies() => _enemies != null ? _enemies() : Array.Empty<EnemyTypeRow>();
        public static IEnumerable<WorldBossTypeRow> GetWorldBossTypes() => _wbTypes != null ? _wbTypes() : Array.Empty<WorldBossTypeRow>();
        public static IEnumerable<WorldBossSpawnRow> GetWorldBossSpawns() => _wbSpawns != null ? _wbSpawns() : Array.Empty<WorldBossSpawnRow>();
        public static IEnumerable<ActiveBiomeEventRow> GetActiveBiomeEvents() => _activeEvents != null ? _activeEvents() : Array.Empty<ActiveBiomeEventRow>();
        public static IEnumerable<ArmorSetRow> GetArmorSets() => _armorSets != null ? _armorSets() : Array.Empty<ArmorSetRow>();
        public static IEnumerable<ArmorPieceRow> GetArmorPieces(string setId) => _armorPieces != null ? _armorPieces(setId) : Array.Empty<ArmorPieceRow>();
        public static IEnumerable<ArmorSetInfoRow> GetArmorSetInfos() => _armorSetInfos != null ? _armorSetInfos() : Array.Empty<ArmorSetInfoRow>();
        public static IEnumerable<ArmorPerkTypeRow> GetArmorPerkTypes() => _armorPerkTypes != null ? _armorPerkTypes() : Array.Empty<ArmorPerkTypeRow>();
        public static IEnumerable<PlayerActiveBuffRow> GetPlayerActiveBuffs() => _playerBuffs != null ? _playerBuffs() : Array.Empty<PlayerActiveBuffRow>();
        public static IEnumerable<PlayerStateRow> GetPlayerStates() => _playerStates != null ? _playerStates() : Array.Empty<PlayerStateRow>();
        public static IEnumerable<NpcRow> GetNpcs() => _npcs != null ? _npcs() : Array.Empty<NpcRow>();
        public static IEnumerable<NpcStateRow> GetNpcStates() => _npcStates != null ? _npcStates() : Array.Empty<NpcStateRow>();
        public static IEnumerable<NpcMemoryRow> GetNpcMemories() => _npcMemories != null ? _npcMemories() : Array.Empty<NpcMemoryRow>();
        public static IEnumerable<QuestArchetypeRow> GetQuestArchetypes() => _questArchetypes != null ? _questArchetypes() : Array.Empty<QuestArchetypeRow>();
        public static IEnumerable<QuestInstanceRow> GetQuestInstances() => _questInstances != null ? _questInstances() : Array.Empty<QuestInstanceRow>();
    }
}
