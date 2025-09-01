using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Dungeon
{
    [Serializable]
    public class BossEnvelope { public int count; public List<BossType> bosses; }
    [Serializable]
    public class BossType { public string id; public string name; public string biome; public int tier; public List<string> mechanics; public List<string> puzzle_modules; public List<string> phases; }

    public static class BossCatalog
    {
        public static List<BossType> FromJson(TextAsset json)
        {
            if (!json || string.IsNullOrEmpty(json.text)) return new List<BossType>();
            try { var env = JsonUtility.FromJson<BossEnvelope>(json.text); return env != null && env.bosses != null ? env.bosses : new List<BossType>(); }
            catch { return new List<BossType>(); }
        }
    }
}

