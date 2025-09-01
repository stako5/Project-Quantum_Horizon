using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Net
{
    public class SpacetimeDbSubscriptions : MonoBehaviour
    {
        [SerializeField] private float refreshInterval = 0.5f;
        [SerializeField] private bool subscribePlayerState = true;
        [SerializeField] private bool subscribeActiveEvents = true;
        [SerializeField] private bool subscribeWorldBoss = true;
        [SerializeField] private bool subscribeArmor = false;
        [SerializeField] private bool subscribeNpcs = true;

        private float _t;

        async void Start()
        {
            await SpacetimeDbClient.Instance.ConnectAsync();
        }

        void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
#if SPACETIMEDB_SDK
            try
            {
                if (subscribePlayerState) UpdatePlayerStates();
                if (subscribeActiveEvents) UpdateActiveEvents();
                if (subscribeWorldBoss) UpdateWorldBoss();
                if (subscribeArmor) UpdateArmor();
                if (subscribeNpcs) UpdateNpcs();
                UpdateQuests();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[STDB] Subscriptions polling error: {e.Message}");
            }
        }

        void UpdateQuests()
        {
            var arcs = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.QuestArchetypes, r => r.ToBridge());
            var inst = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.QuestInstances, r => r.ToBridge());
            BindingsCaches.SetQuestArchetypes(arcs);
            BindingsCaches.SetQuestInstances(inst);
        }
#endif
        }

#if SPACETIMEDB_SDK
        void UpdatePlayerStates()
        {
            // Replace the following with your generated SDK accessors
            // Example: var rows = Module.PlayerState.All().ToList();
            var src = MMORPG.Client.Generated.DataFeed.PlayerStates;
            var list = MMORPG.Client.Generated.Adapters.MapList(src, r => r.ToBridge());
            BindingsCaches.SetPlayerStates(list);
        }

        void UpdateActiveEvents()
        {
            var src2 = MMORPG.Client.Generated.DataFeed.ActiveBiomeEvents;
            var list2 = MMORPG.Client.Generated.Adapters.MapList(src2, r => r.ToBridge());
            BindingsCaches.SetActiveEvents(list2);
        }

        void UpdateWorldBoss()
        {
            var types = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.WorldBossTypes, r => r.ToBridge());
            var spawns = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.WorldBossSpawns, r => r.ToBridge());
            BindingsCaches.SetWorldBossTypes(types);
            BindingsCaches.SetWorldBossSpawns(spawns);
        }

        void UpdateArmor()
        {
            var sets = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.ArmorSets, r => r.ToBridge());
            var infos = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.ArmorSetInfos, r => r.ToBridge());
            var perks = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.ArmorPerkTypes, r => r.ToBridge());
            BindingsCaches.SetArmorSets(sets);
            BindingsCaches.SetArmorInfos(infos);
            BindingsCaches.SetArmorPerkTypes(perks);
            // For pieces, if your SDK provides per-set rows:
            // foreach (var set in MMORPG.Client.Generated.DataFeed.ArmorSets) { var pieces = ...; BindingsCaches.SetArmorPieces(set.Id, pieces.MapList(p => p.ToBridge())); }
        }

        void UpdateNpcs()
        {
            var npcs = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.Npcs, r => r.ToBridge());
            var states = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.NpcStates, r => r.ToBridge());
            var mems = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.NpcMemories, r => r.ToBridge());
            BindingsCaches.SetNpcs(npcs);
            BindingsCaches.SetNpcStates(states);
            BindingsCaches.SetNpcMemories(mems);
        }
        }

        void UpdateQuests()
        {
            var arcs = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.QuestArchetypes, r => r.ToBridge());
            var inst = MMORPG.Client.Generated.Adapters.MapList(MMORPG.Client.Generated.DataFeed.QuestInstances, r => r.ToBridge());
            BindingsCaches.SetQuestArchetypes(arcs);
            BindingsCaches.SetQuestInstances(inst);
        }
#endif
    }
}
