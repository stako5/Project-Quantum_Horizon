using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MMORPG.Client.Net;

namespace MMORPG.Client.World
{
    public class WorldBossEffectApplier : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float checkInterval = 0.5f;
        [SerializeField] private float lerpSpeed = 2.0f;
        [SerializeField] private bool applyFog = true;
        [SerializeField] private bool applyAmbient = true;

        private float _timer;
        private Color _baseFogColor;
        private bool _baseFogEnabled;
        private float _baseFogDensity;
        private float _baseAmbientIntensity;

        private Color _targetFogColor;
        private float _targetFogDensity;
        private float _targetAmbient;
        private float _currentRadius;

        void Start()
        {
            if (!player) player = Camera.main ? Camera.main.transform : transform;
            _baseFogEnabled = RenderSettings.fog;
            _baseFogColor = RenderSettings.fogColor;
            _baseFogDensity = RenderSettings.fogDensity;
            _baseAmbientIntensity = RenderSettings.ambientIntensity;
            _targetFogColor = _baseFogColor; _targetFogDensity = _baseFogDensity; _targetAmbient = _baseAmbientIntensity;
        }

        void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = checkInterval;
                EvaluateNearestBoss();
            }

            // Lerp visuals
            if (applyFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, _targetFogColor, Time.deltaTime * lerpSpeed);
                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, _targetFogDensity, Time.deltaTime * lerpSpeed);
            }
            if (applyAmbient)
            {
                RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, _targetAmbient, Time.deltaTime * lerpSpeed);
            }
        }

        void OnDisable()
        {
            // restore base
            RenderSettings.fog = _baseFogEnabled;
            RenderSettings.fogColor = _baseFogColor;
            RenderSettings.fogDensity = _baseFogDensity;
            RenderSettings.ambientIntensity = _baseAmbientIntensity;
        }

        void EvaluateNearestBoss()
        {
#if SPACETIMEDB_SDK
            var types = BindingsBridge.GetWorldBossTypes().ToDictionary(t => t.Id, t => t);
            var spawns = BindingsBridge.GetWorldBossSpawns().Where(s => s.Alive).ToList();
            if (spawns.Count == 0 || types.Count == 0) { ResetTargets(); return; }
            Vector3 p = player ? player.position : Vector3.zero;
            float bestDist = float.PositiveInfinity; BindingsBridge.WorldBossTypeRow bestType = null; float radius = 0f; Vector2 bestPos = Vector2.zero;
            foreach (var s in spawns)
            {
                if (!types.TryGetValue(s.BossId, out var t)) continue;
                // Parse radius from env json
                float threat = 0f; Color fogColor = _baseFogColor; float fogDensity = _baseFogDensity; float exposure = _baseAmbientIntensity;
                try
                {
                    var env = JsonUtility.FromJson<EnvWrap>(t.EnvJson);
                    if (env != null && env.dict != null)
                    {
                        if (env.dict.TryGetValue("threat_radius_m", out string tr)) float.TryParse(tr, out threat);
                    }
                }
                catch {}
                Vector2 pos = new Vector2(s.X, s.Z);
                float dist = Vector2.Distance(new Vector2(p.x, p.z), pos);
                if (threat > 0f && dist < bestDist)
                {
                    bestDist = dist; bestType = t; radius = threat; bestPos = pos;
                }
            }
            if (bestType == null) { ResetTargets(); return; }
            float f = Mathf.Clamp01(1f - (bestDist / Mathf.Max(1f, radius)));
            ApplyEnv(bestType.EnvJson, f);
#else
            ResetTargets();
#endif
        }

        void ApplyEnv(string json, float strength)
        {
            // default targets
            _targetFogColor = _baseFogColor; _targetFogDensity = _baseFogDensity; _targetAmbient = _baseAmbientIntensity;
            try
            {
                var env = EnvDict.FromJson(json);
                if (env != null)
                {
                    if (env.TryGetColor("fog_color", out var c)) _targetFogColor = Color.Lerp(_baseFogColor, c, strength);
                    if (env.TryGetFloat("fog_density", out var d)) _targetFogDensity = Mathf.Lerp(_baseFogDensity, d, strength);
                    if (env.TryGetFloat("exposure", out var exp)) _targetAmbient = Mathf.Lerp(_baseAmbientIntensity, exp, strength);
                }
            }
            catch { }
        }

        void ResetTargets()
        {
            _targetFogColor = _baseFogColor; _targetFogDensity = _baseFogDensity; _targetAmbient = _baseAmbientIntensity;
        }

        // Minimal JSON helper
        [Serializable]
        class EnvWrap { public Dictionary<string, string> dict; }

        class EnvDict
        {
            private Dictionary<string, object> _d = new Dictionary<string, object>();
            public static EnvDict FromJson(string json)
            {
                if (string.IsNullOrEmpty(json)) return null;
                try { return new EnvDict{ _d = MiniJson.Deserialize(json) as Dictionary<string, object> ?? new Dictionary<string, object>()}; }
                catch { return null; }
            }
            public bool TryGetFloat(string key, out float v)
            {
                v = 0f; if (!_d.ContainsKey(key)) return false; var o = _d[key];
                try { v = Convert.ToSingle(o); return true; } catch { return false; }
            }
            public bool TryGetColor(string key, out Color c)
            {
                c = Color.black; if (!_d.ContainsKey(key)) return false; var o = _d[key] as string; if (string.IsNullOrEmpty(o)) return false;
                if (ColorUtility.TryParseHtmlString(o, out var col)) { c = col; return true; }
                return false;
            }
        }
    }
}

