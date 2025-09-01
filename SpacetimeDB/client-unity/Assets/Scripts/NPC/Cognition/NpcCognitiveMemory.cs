using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MMORPG.Client.NPC.Cognition
{
    [Serializable]
    public class MemoryEvent
    {
        public long timestampMicros;
        public string kind;
        public string description;
        public float valence;   // -1..1 negative..positive
        public float arousal;   // 0..1
        public float importance; // 0..1
        public string[] tags;
    }

    public class NpcCognitiveMemory : MonoBehaviour
    {
        [SerializeField] private int maxEvents = 256;
        [SerializeField] private float decayPerMinute = 0.05f;

        private readonly List<MemoryEvent> _events = new();

        public void Add(string kind, string description, float valence, float arousal, float importance, params string[] tags)
        {
            var ev = new MemoryEvent
            {
                timestampMicros = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000L,
                kind = kind,
                description = description,
                valence = Mathf.Clamp(valence, -1f, 1f),
                arousal = Mathf.Clamp01(arousal),
                importance = Mathf.Clamp01(importance),
                tags = tags ?? Array.Empty<string>(),
            };
            _events.Add(ev);
            if (_events.Count > maxEvents) _events.RemoveAt(0);
        }

        public IEnumerable<MemoryEvent> Recent(int count = 5)
            => _events.OrderByDescending(e => e.timestampMicros).Take(count);

        public IEnumerable<MemoryEvent> QueryByTag(string tag, int count = 5)
            => _events.Where(e => e.tags != null && e.tags.Contains(tag)).OrderByDescending(e => e.timestampMicros).Take(count);

        public (float valence, float arousal) Affect()
        {
            // Weighted average with time decay
            if (_events.Count == 0) return (0f, 0f);
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000L;
            float sumW = 0f, v = 0f, a = 0f;
            foreach (var e in _events)
            {
                float minutes = Mathf.Max(0f, (now - e.timestampMicros) / 1_000_000f / 60f);
                float decay = Mathf.Exp(-decayPerMinute * minutes);
                float w = decay * Mathf.Lerp(0.5f, 1.5f, e.importance);
                sumW += w; v += e.valence * w; a += e.arousal * w;
            }
            if (sumW <= 0f) return (0f, 0f);
            return (v / sumW, a / sumW);
        }
    }
}
