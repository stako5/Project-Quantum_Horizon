using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.InputSystem
{
    [Serializable]
    public class InputBindings
    {
        public Dictionary<string, KeyCode> Keys = new();
        public static readonly string[] Actions = new[] { "Attack", "Roll", "LockOn", "Quick1", "Quick2", "Quick3", "Quick4" };

        public static InputBindings CreateDefault()
        {
            var b = new InputBindings();
            b.Keys["Attack"] = KeyCode.Mouse0;
            b.Keys["Roll"] = KeyCode.Space;
            b.Keys["LockOn"] = KeyCode.LeftShift;
            b.Keys["Quick1"] = KeyCode.Alpha1;
            b.Keys["Quick2"] = KeyCode.Alpha2;
            b.Keys["Quick3"] = KeyCode.Alpha3;
            b.Keys["Quick4"] = KeyCode.Alpha4;
            return b;
        }

        const string PREFS_KEY = "mmorpg_input_bindings";
        public void Save()
        {
            var json = JsonUtility.ToJson(new Wrapper { entries = ToList() });
            PlayerPrefs.SetString(PREFS_KEY, json);
        }

        public static InputBindings Load()
        {
            var json = PlayerPrefs.GetString(PREFS_KEY, null);
            if (string.IsNullOrEmpty(json)) return CreateDefault();
            try
            {
                var w = JsonUtility.FromJson<Wrapper>(json);
                var b = CreateDefault();
                if (WValid(w))
                {
                    foreach (var e in w.entries)
                    {
                        if (Enum.TryParse<KeyCode>(e.key, out var kc)) b.Keys[e.action] = kc;
                    }
                }
                return b;
            }
            catch { return CreateDefault(); }
        }

        static bool WValid(Wrapper w) => w != null && w.entries != null;
        [Serializable] class Entry { public string action; public string key; }
        [Serializable] class Wrapper { public List<Entry> entries; }
        List<Entry> ToList() { var l = new List<Entry>(); foreach (var kv in Keys) l.Add(new Entry { action = kv.Key, key = kv.Value.ToString() }); return l; }
    }
}
