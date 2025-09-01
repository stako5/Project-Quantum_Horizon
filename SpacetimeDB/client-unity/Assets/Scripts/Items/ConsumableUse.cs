using System.Collections.Generic;
using UnityEngine;
using MMORPG.Client.Combat;

namespace MMORPG.Client.Items
{
    public class ConsumableUse : MonoBehaviour
    {
        [SerializeField] private BuffManager buffManager;
        [SerializeField] private bool requireServerAck = false;
        private readonly System.Collections.Generic.Dictionary<string, float> _cooldownUntilById = new();

        // Backward-compat; not used for gating specific consumables
        public bool CanUse => true;

        public bool CanUse(Consumable c)
        {
            if (c == null) return false;
            return GetRemainingCooldown(c) <= 0f;
        }

        public float GetRemainingCooldown(Consumable c)
        {
            if (c == null) return 0f;
            if (_cooldownUntilById.TryGetValue(c.Id, out var until))
            {
                return Mathf.Max(0f, until - Time.time);
            }
            return 0f;
        }

        public async void Use(Consumable c)
        {
            if (!CanUse(c) || c == null) return;
            if (!buffManager) buffManager = GetComponent<BuffManager>();
            if (!buffManager) return;
            // Try server-authoritative consume when SDK is available
#if SPACETIMEDB_SDK
            try
            {
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("consume_consumable", c.Id);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[ConsumableUse] Server consume failed: {ex.Message}. Falling back to local.");
                if (requireServerAck) return;
            }
#endif
            var def = new BuffDefinition{ id = c.Id, name = c.Name, tier = c.Tier, stackMode = c.StackMode, effects = c.Effects };
            buffManager.Apply(def);
            _cooldownUntilById[c.Id] = Time.time + Mathf.Max(0f, c.CooldownS);
        }
    }
}
