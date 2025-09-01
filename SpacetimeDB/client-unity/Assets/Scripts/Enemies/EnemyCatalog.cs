using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MMORPG.Client.Enemies
{
    public interface IEnemyDataSource
    {
        List<EnemyTypeModel> GetAll();
    }

    public class LocalJsonEnemySource : IEnemyDataSource
    {
        private List<EnemyTypeModel> _list = new List<EnemyTypeModel>();
        public LocalJsonEnemySource(TextAsset json)
        {
            if (json != null && !string.IsNullOrEmpty(json.text))
            {
                try
                {
                    var env = JsonUtility.FromJson<EnemyEnvelope>(json.text);
                    if (env != null && env.enemies != null) _list = env.enemies;
                }
                catch { /* ignore */ }
            }
        }
        public List<EnemyTypeModel> GetAll() => _list ?? new List<EnemyTypeModel>();
    }

    public static class EnemyCatalog
    {
        private static IEnemyDataSource _source;
        public static void Use(IEnemyDataSource source) { _source = source; }
        public static List<EnemyTypeModel> All => _source != null ? _source.GetAll() : new List<EnemyTypeModel>();
        public static List<EnemyTypeModel> Filter(string family, int minTier, int maxTier)
        {
            return All.Where(e => (string.IsNullOrEmpty(family) || e.family == family) && e.tier >= minTier && e.tier <= maxTier).ToList();
        }
    }
}
