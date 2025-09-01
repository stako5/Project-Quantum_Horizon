using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MMORPG.Client.Net;

namespace MMORPG.Client.World
{
    public class WorldEventMarkerManager : MonoBehaviour
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
                var go = new GameObject("WorldEventMarkers"); go.transform.SetParent(transform, false); markersRoot = go.transform;
            }
        }

        void Update()
        {
            _timer -= Time.deltaTime; if (_timer > 0f) return; _timer = refreshInterval;
#if SPACETIMEDB_SDK
            var events = BindingsBridge.GetActiveBiomeEvents().ToList();
            var activeIds = new HashSet<ulong>(events.Select(e => e.Id));
            // Remove expired
            var toRemove = _markers.Keys.Where(id => !activeIds.Contains(id)).ToList();
            foreach (var id in toRemove) { Destroy(_markers[id]); _markers.Remove(id); }
            // Ensure markers exist
            foreach (var e in events)
            {
                if (!_markers.ContainsKey(e.Id))
                {
                    var go = Instantiate(markerPrefab, markersRoot);
                    go.transform.position = new Vector3(e.X, 0f, e.Z);
                    _markers[e.Id] = go;
                }
            }
#endif
        }
    }
}

