// Auto-generated row type stubs matching SpacetimeDB server tables (names/fields)
// Note: Identity is represented as string and Timestamp as microseconds (long).
// Replace or extend with your actual SDK-generated types as needed.

using System;

namespace MMORPG.Client.Generated
{
    // Players
    public class PlayerRow { public string Identity; public string Name; public long CreatedAt; }

    // World state/config
    public class WorldStateRow { public uint Id; public ulong Tick; }
    public class WorldConfigRow { public uint Id; public ulong Seed; public uint SizeMeters; public uint ChunkSizeM; public long CreatedAt; }

    // Realtime player state
    public class PlayerStateRow { public string Identity; public byte RegionId; public float X; public float Y; public float Z; public float YawDeg; public long UpdatedAt; }

    // NPCs
    public class NpcRow { public ulong Id; public string Name; public string Faction; public string Role; public string Personality; public byte HomeRegion; public string Bio; }
    public class NpcStateRow { public ulong NpcId; public byte RegionId; public float X; public float Y; public float Z; public string Mood; public string Activity; public long UpdatedAt; }
    public class NpcMemoryRow { public ulong Id; public ulong NpcId; public long At; public string Kind; public string Description; public byte Importance; }

    // Quests
    public class QuestArchetypeRow { public string Id; public string Name; public string Description; public string Category; public string EventType; public string EventParam; public uint RequiredCount; public uint? TimeLimitS; public string ChainNextId; public uint RewardCredits; public string RewardItem; public uint? RewardItemQty; }
    public class QuestInstanceRow { public ulong Id; public string ArchetypeId; public ulong NpcId; public string PlayerIdentity; public string State; public long CreatedAt; public long? ExpiresAt; public uint CurrentCount; }

    // Content: abilities/perks/catalogs
    public class AbilityRow
    {
        public string Id; public string ClassName; public string Name; public string Branch; public byte Tier;
        public string Kind; public string ResourceType; public float? ResourceCost; public float? CooldownS; public string Description;
    }
    public class PerkRow
    {
        public string Id; public string ClassName; public string Name; public string Tier; public string TagsCsv;
        public string EffectDesc; public string RollStat; public float? RollMin; public float? RollMax; public string Description;
    }
    public class PerkCatalogRow { public string ClassName; public string IdsCsv; public long UpdatedAt; }
    public class ClassCatalogRow { public string ClassName; public long UpdatedAt; }

    // Weapons
    public class WeaponInstanceRow { public ulong Id; public string Owner; public string ClassName; public string WeaponType; public string Rarity; public uint Power; public uint Level; public long CreatedAt; }
    public class WeaponPerkRow { public ulong Id; public ulong WeaponId; public string PerkId; public string PerkName; public string RolledStat; public float? RolledValue; }
    public class RarityConfigRow { public string Name; public uint Weight; public byte Order; }

    // World regions
    public class WorldRegionRow { public byte Id; public string Name; public ulong Seed; public uint SizeMeters; public uint ChunkSizeM; public string BiomeJson; public long CreatedAt; public long UpdatedAt; }

    // Boss catalog and armor sets
    public class BossTypeRow { public string Id; public string Name; public string Biome; public byte Tier; public string MechanicsCsv; public string PuzzleModulesCsv; public string PhasesCsv; public string ArmorSetId; public string Description; }
    public class ArmorSetRow { public string Id; public string Name; public string SetBonusesCsv; }
    public class ArmorPieceRow { public ulong Id; public string SetId; public string Slot; public string Name; public string Bonus; }

    // World bosses
    public class WorldBossTypeRow { public string Id; public string Name; public string Biome; public byte Tier; public string EnvJson; public byte MinNgPlus; public string Description; }
    public class WorldBossSpawnRow { public ulong Id; public string BossId; public byte RegionId; public float X; public float Z; public bool Alive; public long SpawnedAt; }

    // Enemies
    public class EnemyTypeRow
    {
        public string Id; public string Name; public string Family; public byte Tier; public string Role; public string Element; public string Size; public string Movement; public string Armor;
        public uint Hp; public uint Damage; public float Speed; public string AbilitiesCsv; public string TagsCsv; public string Description;
    }

    // Avatar
    public class AvatarRow { public string Identity; public ushort HeightCm; public ushort WeightKg; public string BodyJson; public string FaceJson; public long CreatedAt; public long UpdatedAt; }

    // Items
    public class ItemRow { public ulong Id; public string Owner; public string Name; public uint Qty; }

    // Consumables
    public class ConsumableCatalogRow { public string Id; public string Name; public byte Tier; public string Category; public uint CooldownS; public string Stack; }
    public class ConsumableEffectRow { public ulong Id; public string ConsumableId; public string Kind; public float Amount; public uint DurationS; }

    // Active buffs and cooldowns
    public class PlayerActiveBuffRow { public ulong Id; public string Identity; public string BuffId; public byte Tier; public byte Stacks; public string StackMode; public long StartedAt; public long ExpiresAt; }
    public class PlayerConsumableCooldownRow { public string IdentityConsumable; public string Identity; public string ConsumableId; public long AvailableAt; }

    // Owned armor
    public class OwnedArmorPieceRow { public ulong Id; public string Owner; public string SetId; public string Slot; public string PieceName; public string Bonus; public long AcquiredAt; }
    public class ArmorSetInfoRow { public string SetId; public string Category; public string FactionName; public string EnemyFamily; public uint? RepRequired; }
    public class ArmorPerkTypeRow { public string Id; public string Name; public string Stat; public float Min; public float Max; public string Rarity; }
    public class OwnedArmorPiecePerkRow { public ulong Id; public ulong OwnedPieceId; public string PerkId; public float RolledValue; }

    // Factions & reputation
    public class FactionRow { public string Name; public string Description; }
    public class PlayerReputationRow { public ulong Id; public string Identity; public string FactionName; public int Value; public long UpdatedAt; }

    // Artifacts & currency
    public class ArtifactTypeRow { public string Id; public string Name; public string Rarity; public string Description; }
    public class OwnedArtifactRow { public ulong Id; public string Identity; public string ArtifactId; public string Source; public long AcquiredAt; }
    public class PlayerCurrencyRow { public string Identity; public uint Credits; public uint MythicShards; public long UpdatedAt; }

    // Difficulty and NG+ state
    public class PlayerNgPlusRow { public string Identity; public byte Level; public long UpdatedAt; }
    public class DifficultyStateRow { public uint Id; public byte Level; public long UpdatedAt; }

    // Activities (cooldowns/progress)
    public class ActivityCooldownRow { public string Biome; public long AvailableAt; }
    public class PlayerActivityProgressRow { public ulong Id; public string Identity; public string ActivityKey; public uint Completed; public long LastCompletedAt; }

    // Biome events
    public class BiomeEventCatalogRow { public ulong Id; public string Biome; public string Name; public uint Weight; public uint CooldownSeconds; }
    public class BiomeEventCooldownRow { public string Biome; public long AvailableAt; }
    public class ActiveBiomeEventRow { public ulong Id; public string Biome; public string Name; public byte RegionId; public float X; public float Z; public long StartedAt; public long ExpiresAt; }
}
