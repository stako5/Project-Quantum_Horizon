using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.Items
{
    public interface IConsumablesBackend
    {
        Task<bool> ImportAsync(string json);
        Task<bool> GrantAsync(string id, uint qty);
        Task<bool> ConsumeAsync(string id);
    }

    public class StdbConsumablesBackend : IConsumablesBackend
    {
        public async Task<bool> ImportAsync(string json)
        {
            try
            {
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("import_consumables", json);
                return true;
            }
            catch (System.Exception e) { Debug.LogWarning($"[ConsumablesBackend] import failed: {e.Message}"); return false; }
        }

        public async Task<bool> GrantAsync(string id, uint qty)
        {
            try
            {
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("grant_consumable_to_self", id, qty);
                return true;
            }
            catch (System.Exception e) { Debug.LogWarning($"[ConsumablesBackend] grant failed: {e.Message}"); return false; }
        }

        public async Task<bool> ConsumeAsync(string id)
        {
            try
            {
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("consume_consumable", id);
                return true;
            }
            catch (System.Exception e) { Debug.LogWarning($"[ConsumablesBackend] consume failed: {e.Message}"); return false; }
        }
    }
}

