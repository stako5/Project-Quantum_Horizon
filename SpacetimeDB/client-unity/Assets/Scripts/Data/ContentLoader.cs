using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Data
{
    public static class ContentLoader
    {
        public static List<AbilityModel> LoadAbilitiesFromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<AbilityModel>();
            try
            {
                // Envelope or raw array
                if (json.TrimStart().StartsWith("{"))
                {
                    var env = JsonUtility.FromJson<AbilityEnvelope>(json);
                    if (env != null && env.abilities != null) return env.abilities;
                }
                return FromArray<AbilityModel>(json);
            }
            catch (Exception)
            {
                return new List<AbilityModel>();
            }
        }

        public static List<PerkModel> LoadPerksFromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<PerkModel>();
            try
            {
                if (json.TrimStart().StartsWith("{"))
                {
                    var env = JsonUtility.FromJson<PerkEnvelope>(json);
                    if (env != null && env.perks != null) return env.perks;
                }
                return FromArray<PerkModel>(json);
            }
            catch (Exception)
            {
                return new List<PerkModel>();
            }
        }

        static List<T> FromArray<T>(string json)
        {
            // Unity JsonUtility requires wrapper for arrays
            var wrapper = JsonUtility.FromJson<Wrapper<T>>(WrapArray(json));
            return wrapper != null && wrapper.items != null ? wrapper.items : new List<T>();
        }

        static string WrapArray(string json) => $"{{\"items\":{json}}}";

        [Serializable]
        class Wrapper<T> { public List<T> items; }
    }
}

