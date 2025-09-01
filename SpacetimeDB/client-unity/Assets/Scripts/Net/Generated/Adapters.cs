using System.Collections.Generic;
using System.Linq;

namespace MMORPG.Client.Generated
{
    public static class Adapters
    {
        public static MMORPG.Client.Net.BindingsBridge.PlayerStateRow ToBridge(this PlayerStateRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.PlayerStateRow
            {
                Identity = r.Identity,
                RegionId = r.RegionId,
                X = r.X, Y = r.Y, Z = r.Z,
                YawDeg = r.YawDeg,
                UpdatedAt = r.UpdatedAt,
            };
        }

        public static MMORPG.Client.Net.BindingsBridge.ActiveBiomeEventRow ToBridge(this ActiveBiomeEventRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.ActiveBiomeEventRow
            {
                Id = r.Id,
                Biome = r.Biome,
                Name = r.Name,
                RegionId = r.RegionId,
                X = r.X,
                Z = r.Z,
                StartedAt = r.StartedAt,
                ExpiresAt = r.ExpiresAt,
            };
        }

        public static MMORPG.Client.Net.BindingsBridge.WorldBossTypeRow ToBridge(this WorldBossTypeRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.WorldBossTypeRow
            {
                Id = r.Id, Name = r.Name, Biome = r.Biome, Tier = r.Tier,
                EnvJson = r.EnvJson, MinNgPlus = r.MinNgPlus, Description = r.Description,
            };
        }

        public static MMORPG.Client.Net.BindingsBridge.WorldBossSpawnRow ToBridge(this WorldBossSpawnRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.WorldBossSpawnRow
            {
                Id = r.Id, BossId = r.BossId, RegionId = r.RegionId, X = r.X, Z = r.Z, Alive = r.Alive,
            };
        }

        public static MMORPG.Client.Net.BindingsBridge.ArmorSetRow ToBridge(this ArmorSetRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.ArmorSetRow { Id = r.Id, Name = r.Name };
        }
        public static MMORPG.Client.Net.BindingsBridge.ArmorSetInfoRow ToBridge(this ArmorSetInfoRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.ArmorSetInfoRow { SetId = r.SetId, Category = r.Category, FactionName = r.FactionName, EnemyFamily = r.EnemyFamily, RepRequired = r.RepRequired };
        }
        public static MMORPG.Client.Net.BindingsBridge.ArmorPieceRow ToBridge(this ArmorPieceRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.ArmorPieceRow { Id = r.Id, SetId = r.SetId, Slot = r.Slot, Name = r.Name, Bonus = r.Bonus };
        }
        public static MMORPG.Client.Net.BindingsBridge.ArmorPerkTypeRow ToBridge(this ArmorPerkTypeRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.ArmorPerkTypeRow { Id = r.Id, Name = r.Name, Stat = r.Stat, Min = r.Min, Max = r.Max, Rarity = r.Rarity };
        }

        public static List<TOut> MapList<TIn, TOut>(this IEnumerable<TIn> src, System.Func<TIn, TOut> f)
            => src == null ? new List<TOut>() : src.Select(f).ToList();

        public static MMORPG.Client.Net.BindingsBridge.NpcRow ToBridge(this NpcRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.NpcRow
            {
                Id = r.Id,
                Name = r.Name,
                Faction = r.Faction,
                Role = r.Role,
                Personality = r.Personality,
                HomeRegion = r.HomeRegion,
                Bio = r.Bio,
            };
        }

        public static MMORPG.Client.Net.BindingsBridge.NpcStateRow ToBridge(this NpcStateRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.NpcStateRow
            {
                NpcId = r.NpcId,
                RegionId = r.RegionId,
                X = r.X, Y = r.Y, Z = r.Z,
                Mood = r.Mood,
                Activity = r.Activity,
                UpdatedAt = r.UpdatedAt,
            };
        }

        public static MMORPG.Client.Net.BindingsBridge.NpcMemoryRow ToBridge(this NpcMemoryRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.NpcMemoryRow
            {
                Id = r.Id,
                NpcId = r.NpcId,
                At = r.At,
                Kind = r.Kind,
                Description = r.Description,
                Importance = r.Importance,
            };
        }

        public static MMORPG.Client.Net.BindingsBridge.QuestArchetypeRow ToBridge(this QuestArchetypeRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.QuestArchetypeRow
            {
                Id = r.Id, Name = r.Name, Description = r.Description, Category = r.Category,
                EventType = r.EventType, EventParam = r.EventParam, RequiredCount = r.RequiredCount, TimeLimitS = r.TimeLimitS,
                ChainNextId = r.ChainNextId, RewardCredits = r.RewardCredits, RewardItem = r.RewardItem, RewardItemQty = r.RewardItemQty,
            };
        }

        public static MMORPG.Client.Net.BindingsBridge.QuestInstanceRow ToBridge(this QuestInstanceRow r)
        {
            return new MMORPG.Client.Net.BindingsBridge.QuestInstanceRow
            {
                Id = r.Id, ArchetypeId = r.ArchetypeId, NpcId = r.NpcId, PlayerIdentity = r.PlayerIdentity, State = r.State,
                CreatedAt = r.CreatedAt, ExpiresAt = r.ExpiresAt, CurrentCount = r.CurrentCount,
            };
        }

    }
}
