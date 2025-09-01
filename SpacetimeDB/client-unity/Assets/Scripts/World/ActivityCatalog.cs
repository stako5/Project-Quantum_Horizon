using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MMORPG.Client.World
{
    [Serializable] public class ActivitiesEnvelope { public List<PoiDef> pois; public List<EventDef> events; }
    [Serializable] public class PoiDef { public string biome; public string name; public float density_per_hectare; public List<string> tags; }
    [Serializable] public class EventDef { public string biome; public string name; public int weight; public int cooldown_chunks; }

    public static class ActivityCatalog
    {
        static ActivitiesEnvelope _env;
        public static void Load(TextAsset json)
        {
            if (!json || string.IsNullOrEmpty(json.text)) { _env = new ActivitiesEnvelope{ pois=new List<PoiDef>(), events=new List<EventDef>()}; return; }
            try { _env = JsonUtility.FromJson<ActivitiesEnvelope>(json.text) ?? new ActivitiesEnvelope(); }
            catch { _env = new ActivitiesEnvelope(); }
            if (_env.pois == null) _env.pois = new List<PoiDef>();
            if (_env.events == null) _env.events = new List<EventDef>();
        }
        public static List<PoiDef> PoisFor(string biome) => _env?.pois?.Where(p => p.biome == biome).ToList() ?? new List<PoiDef>();
        public static List<EventDef> EventsFor(string biome) => _env?.events?.Where(e => e.biome == biome).ToList() ?? new List<EventDef>();
    }
}

