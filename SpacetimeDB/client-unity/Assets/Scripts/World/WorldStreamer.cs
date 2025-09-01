using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.World
{
    public class WorldStreamer : MonoBehaviour
    {
        [SerializeField] private ProcGenConfig config;
        [SerializeField] private Transform player;
        [SerializeField] private GameObject chunkPrefab; // must have Chunk component

        private readonly Dictionary<Vector2Int, Chunk> _loaded = new();
        private int _nChunksPerAxis;

        void Start()
        {
            if (!config) { Debug.LogError("WorldStreamer: Missing ProcGenConfig"); enabled = false; return; }
            _nChunksPerAxis = Mathf.Max(1, Mathf.FloorToInt(config.worldSizeMeters / config.chunkSizeMeters));
        }

        void Update()
        {
            if (!player) return;
            var center = WorldToChunk(player.position);
            var r = Mathf.Max(0, config.viewDistanceChunks);

            // Load needed
            for (int dy = -r; dy <= r; dy++)
            for (int dx = -r; dx <= r; dx++)
            {
                var c = new Vector2Int(Mathf.Clamp(center.x + dx, 0, _nChunksPerAxis - 1),
                                        Mathf.Clamp(center.y + dy, 0, _nChunksPerAxis - 1));
                if (_loaded.ContainsKey(c)) continue;
                SpawnChunk(c);
            }

            // Unload far
            var toRemove = new List<Vector2Int>();
            foreach (var kv in _loaded)
            {
                var c = kv.Key;
                if (Mathf.Abs(c.x - center.x) > r || Mathf.Abs(c.y - center.y) > r)
                    toRemove.Add(c);
            }
            foreach (var key in toRemove)
            {
                Destroy(_loaded[key].gameObject);
                _loaded.Remove(key);
            }
        }

        Vector2Int WorldToChunk(Vector3 pos)
        {
            var cs = config.chunkSizeMeters;
            int cx = Mathf.Clamp(Mathf.FloorToInt(pos.x / cs), 0, _nChunksPerAxis - 1);
            int cy = Mathf.Clamp(Mathf.FloorToInt(pos.z / cs), 0, _nChunksPerAxis - 1);
            return new Vector2Int(cx, cy);
        }

        void SpawnChunk(Vector2Int coord)
        {
            GameObject go;
            if (chunkPrefab)
                go = Instantiate(chunkPrefab, transform);
            else
                go = new GameObject($"Chunk_{coord.x}_{coord.y}");

            var chunk = go.GetComponent<Chunk>();
            if (!chunk) chunk = go.AddComponent<Chunk>();
            chunk.Build(config, coord);
            _loaded[coord] = chunk;
        }
    }
}

