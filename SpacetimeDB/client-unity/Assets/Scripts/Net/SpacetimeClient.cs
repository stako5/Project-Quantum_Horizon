using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.Net
{
    public class SpacetimeClient : MonoBehaviour
    {
        [SerializeField] private string url = "ws://127.0.0.1:3000";
        [SerializeField] private string module = "mmorpg_server";
        [SerializeField] private string playerName = "Hero";

        // Replace with actual SDK types once imported
        private object _db;

        private async void Start()
        {
            await ConnectAndRegister();
        }

        public async Task ConnectAndRegister()
        {
            // TODO: swap to SpacetimeDB C# SDK connect call
            await Task.Yield();
            _db = new object();
            Debug.Log($"[SpacetimeDB] Connected to {url} module={module}");

            await CallReducer("register_player");
            await CallReducer("set_player_name", playerName);
        }

        public Task CallReducer(string name, params object[] args)
        {
            // SDK placeholder
            Debug.Log($"[SpacetimeDB] Call reducer {name} args={args?.Length ?? 0}");
            return Task.CompletedTask;
        }
    }
}

