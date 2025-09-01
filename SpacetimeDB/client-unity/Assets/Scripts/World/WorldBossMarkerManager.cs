using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MMORPG.Client.Net;

namespace MMORPG.Client.World
{
    public class WorldBossMarkerManager : MonoBehaviour
    {
        [SerializeField] private Transform markersRoot;
        [SerializeField] private GameObject markerPrefab;
        [SerializeField] private float refreshInterval = 1.0f;

        private readonly Dictionary<ulong, GameObject> _markers = new();
        private float _timer;

        void Awake()
        {
            if (!markersRoot)
            {
                var go = new GameObject("WorldBossMarkers"); go.transform.SetParent(transform, false); markersRoot = go.transform;
            }
        }

        void Update()
        {
            _timer -= Time.deltaTime; if (_timer > 0f) return; _timer = refreshInterval;
#if SPACETIMEDB_SDK
            var spawns = BindingsBridge.GetWorldBossSpawns().ToList();
            var alive = new HashSet<ulong>(spawns.Where(s => s.Alive).Select(s => s.Id));
            // Remove dead markers
            var toRemove = _markers.Keys.Where(id => !alive.Contains(id)).ToList();
            foreach (var id in toRemove) { Destroy(_markers[id]); _markers.Remove(id); }
            // Ensure alive markers exist
            foreach (var s in spawns)
            {
                if (!s.Alive) continue;
                if (!_markers.ContainsKey(s.Id))
                {
                    var go = Instantiate(markerPrefab, markersRoot);
                    go.transform.position = new Vector3(s.X, 0f, s.Z);
                    _markers[s.Id] = go;
                }
                // Highlight tracked boss
                var isTracked = MMORPG.Client.UI.WorldBossTracking.TrackedBossId == s.BossId;
                var rend = _markers[s.Id].GetComponentInChildren<Renderer>();
                if (rend && rend.material)
                {
                    rend.material.color = isTracked ? new Color(1f,0.3f,0f,1f) : new Color(1f,0.5f,0f,1f);
                }
            }
#endif
        }
    }
}
