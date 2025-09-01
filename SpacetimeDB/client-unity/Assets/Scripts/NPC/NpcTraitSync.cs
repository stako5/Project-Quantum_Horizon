using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using MMORPG.Client.Net;
using MMORPG.Client.NPC.Cognition;

namespace MMORPG.Client.NPC
{
    public class NpcTraitSync : MonoBehaviour
    {
        [SerializeField] private ulong npcId;
        [SerializeField] private NpcCognition cognition;
        [SerializeField] private bool applyOnce = true;
        [SerializeField] private float refreshInterval = 2.0f;

        private float _t;
        private bool _applied;

        void Awake()
        {
            if (!cognition) cognition = GetComponent<NpcCognition>();
        }

        void Update()
        {
            if (applyOnce && _applied) return;
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
            var npcs = BindingsBridge.GetNpcs().ToList();
            var row = npcId != 0 ? npcs.FirstOrDefault(n => n.Id == npcId) : npcs.FirstOrDefault();
            if (row != null && npcId == 0) npcId = row.Id; // lock to a concrete NPC id once found
            if (row == null || string.IsNullOrEmpty(row.Personality)) return;
            EnsurePersonality();
            if (!cognition || !cognition.personality) return;

            // Example string: "Curious; O:0.74 C:0.62 E:0.81"
            ParseTraits(row.Personality, out float o, out float c, out float e);
            cognition.personality.openness = o;
            cognition.personality.conscientiousness = c;
            cognition.personality.extraversion = e;
            // Simple preference mapping for flavor
            cognition.personality.prefersExploration = Mathf.Lerp(-0.25f, 0.5f, o);
            cognition.personality.prefersSocial = Mathf.Lerp(-0.25f, 0.5f, e);
            _applied = true;
        }

        void EnsurePersonality()
        {
            if (!cognition) return;
            if (!cognition.personality)
            {
                cognition.personality = ScriptableObject.CreateInstance<NpcPersonality>();
            }
        }

        static void ParseTraits(string s, out float o, out float c, out float e)
        {
            // Default mid values if parsing fails
            o = 0.5f; c = 0.5f; e = 0.5f;
            try
            {
                // Find tokens like "O:0.74" "C:0.62" "E:0.81"
                var parts = s.Split(new[] {' ', ';', ','}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                {
                    if (p.StartsWith("O:", StringComparison.OrdinalIgnoreCase))
                    {
                        if (float.TryParse(p.Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) o = Mathf.Clamp01(v);
                    }
                    else if (p.StartsWith("C:", StringComparison.OrdinalIgnoreCase))
                    {
                        if (float.TryParse(p.Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) c = Mathf.Clamp01(v);
                    }
                    else if (p.StartsWith("E:", StringComparison.OrdinalIgnoreCase))
                    {
                        if (float.TryParse(p.Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) e = Mathf.Clamp01(v);
                    }
                }
            }
            catch { /* keep defaults */ }
        }

        public void SetNpcId(ulong id) { npcId = id; _applied = false; }
    }
}
