using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MMORPG.Client.Net;

namespace MMORPG.Client.Net
{
    public class RemotePlayersManager : MonoBehaviour
    {
        [SerializeField] private GameObject ghostPrefab;
        [SerializeField] private byte localRegionId = 0;
        [SerializeField] private float refreshInterval = 0.25f;
        [SerializeField] private bool showNameplate = true;

        private readonly Dictionary<string, GameObject> _ghosts = new();
        private float _t;

        void Awake()
        {
            if (!ghostPrefab)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Capsule); go.name = "GhostTemplate"; go.SetActive(false);
                ghostPrefab = go;
            }
        }

        void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
#if SPACETIMEDB_SDK
            var states = BindingsBridge.GetPlayerStates().ToList();
            if (states.Count == 0) return;
            // Filter to local region if provided
            states = states.Where(s => s.RegionId == localRegionId).ToList();
            var ids = new HashSet<string>(states.Select(s => s.Identity));
            // Remove stale ghosts
            foreach (var id in _ghosts.Keys.ToList()) if (!ids.Contains(id)) { Destroy(_ghosts[id]); _ghosts.Remove(id); }
            // Update/add ghosts
            foreach (var s in states)
            {
                if (!_ghosts.TryGetValue(s.Identity, out var go))
                {
                    go = Instantiate(ghostPrefab, transform);
                    go.name = $"Ghost:{s.Identity}"; go.SetActive(true);
                    _ghosts[s.Identity] = go;
                    if (showNameplate)
                    {
                        var nameGo = new GameObject("Name"); nameGo.transform.SetParent(go.transform, false); nameGo.transform.localPosition = new Vector3(0,1.2f,0);
                        var tm = nameGo.AddComponent<TextMesh>(); tm.color = Color.white; tm.fontSize = 32; tm.text = s.Identity?.Substring(0, System.Math.Min(6, s.Identity.Length)) ?? "?";
                    }
                }
                go.transform.position = new Vector3(s.X, s.Y, s.Z);
                var rot = go.transform.eulerAngles; rot.y = s.YawDeg; go.transform.eulerAngles = rot;
            }
#endif
        }
    }
}
