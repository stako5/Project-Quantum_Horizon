using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MMORPG.Client.Net;
using MMORPG.Client.Combat;
using MMORPG.Client.Items;

namespace MMORPG.Client.UI
{
    public class BuffSyncFromServer : MonoBehaviour
    {
        [SerializeField] private BuffManager buffManager;
        [SerializeField] private float refreshInterval = 1.0f;

        private float _timer;
        private Dictionary<string, Consumable> _byId;

        void Awake()
        {
            if (!buffManager) buffManager = GetComponent<BuffManager>();
            ConsumableCatalog.LoadFromDesign();
            _byId = ConsumableCatalog.All().ToDictionary(c => c.Id, c => c);
        }

        void Update()
        {
            _timer -= Time.deltaTime; if (_timer > 0f) return; _timer = refreshInterval;
            if (!buffManager) return;
            var rows = BindingsBridge.GetPlayerActiveBuffs().ToList();
            if (rows.Count == 0) return; // nothing to sync
            long nowMicros = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000L;
            var snap = new List<(BuffDefinition def, float remainingS, int stacks)>();
            foreach (var r in rows)
            {
                if (!_byId.TryGetValue(r.BuffId, out var c)) continue;
                var def = new BuffDefinition { id = c.Id, name = c.Name, tier = c.Tier, stackMode = c.StackMode, effects = c.Effects };
                float remainingS = Mathf.Max(0f, (r.ExpiresAt - nowMicros) / 1_000_000f);
                snap.Add((def, remainingS, (int)r.Stacks));
            }
            if (snap.Count > 0) buffManager.SetServerSnapshot(snap);
        }
    }
}

