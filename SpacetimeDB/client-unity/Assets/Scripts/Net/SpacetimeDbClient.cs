using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.Net
{
    // Lightweight wrapper around the SpacetimeDB C# SDK.
    public class SpacetimeDbClient : MonoBehaviour
    {
        public static SpacetimeDbClient Instance { get; private set; }

        [Header("Connection")]
        [SerializeField] private string host = "http://localhost:3000";
        [SerializeField] private string moduleName = "mmorpg_server";

        public bool IsConnected { get; private set; }

#if SPACETIMEDB_SDK
        private DbConnection _conn;
#endif

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
        }

        public async Task ConnectAsync()
        {
#if SPACETIMEDB_SDK
            if (IsConnected) return;
            _conn = DbConnection.Builder()
                .WithUri(host)
                .WithModuleName(moduleName)
                .WithToken(AuthToken.Token)
                .OnConnect((c, id, token) => { AuthToken.Token = token; IsConnected = true; })
                .OnConnectError(e => { Debug.LogError($"[STDB] Connect error: {e}"); })
                .OnDisconnect((c, e) => { IsConnected = false; })
                .Build();
            // Optional: subscribe to everything; narrow later
            _conn.SubscriptionBuilder().SubscribeToAllTables();
            await Task.Yield();
#else
            await Task.Yield();
            Debug.LogWarning("[STDB] SPACETIMEDB_SDK not defined; running in stub mode.");
#endif
        }

        public Task CallReducerAsync(string name, params object[] args)
        {
#if SPACETIMEDB_SDK
            // If using generated reducers, prefer _conn.Reducers.<Name>(args)
            // Generic invoke fallback:
            return _conn.Reducers.Invoke(name, args);
#else
            Debug.Log($"[STDB] Stub CallReducer {name} ({args?.Length ?? 0} args)");
            return Task.CompletedTask;
#endif
        }

        // Experimental: call a reducer expected to return a string payload.
        public Task<string> CallReducerForStringAsync(string name, params object[] args)
        {
#if SPACETIMEDB_SDK
            // TODO: replace with actual SDK call that returns string
            // Placeholder returns empty string to avoid compile errors when SDK is present.
            return Task.FromResult(string.Empty);
#else
            Debug.Log($"[STDB] Stub CallReducerForString {name} ({args?.Length ?? 0} args)");
            // Return a generic stub JSON envelope
            return Task.FromResult("{\"text\":\"(stub) hello\"}");
#endif
        }

        public (bool ok, T row) FindById<T, K>(Func<K, T> findFunc, K key)
        {
            try { return (true, findFunc(key)); } catch { return (false, default); }
        }

        public string Host => host;
        public string ModuleName => moduleName;
    }
}
