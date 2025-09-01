using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Enemies
{
    public class ThreatTable : MonoBehaviour
    {
        private readonly Dictionary<Transform, float> _threat = new();
        public float decayPerSecond = 2f;
        public float shareRadius = 12f; // group aggro share

        void Update()
        {
            if (_threat.Count == 0) return;
            float dec = decayPerSecond * Time.deltaTime;
            var keys = new List<Transform>(_threat.Keys);
            foreach (var k in keys)
            {
                _threat[k] -= dec; if (_threat[k] <= 0f) _threat.Remove(k);
            }
        }

        public void AddThreat(Transform source, float amount)
        {
            if (!source) return;
            if (!_threat.ContainsKey(source)) _threat[source] = 0f;
            _threat[source] += Mathf.Max(0f, amount);

            // Share to nearby allies
            var hits = Physics.OverlapSphere(transform.position, shareRadius);
            foreach (var h in hits)
            {
                if (h.attachedRigidbody && h.attachedRigidbody.transform == transform) continue;
                var ally = h.GetComponentInParent<ThreatTable>();
                if (ally && ally != this)
                {
                    ally.AddThreatLocal(source, amount * 0.25f);
                }
            }
        }

        public void AddThreatLocal(Transform source, float amount)
        {
            if (!source) return;
            if (!_threat.ContainsKey(source)) _threat[source] = 0f;
            _threat[source] += Mathf.Max(0f, amount);
        }

        public Transform GetHighest() { Transform t = null; float best = -1f; foreach (var kv in _threat){ if (kv.Value > best){ best = kv.Value; t = kv.Key; } } return t; }
        public void Clear() { _threat.Clear(); }
    }
}

