using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Items
{
    public static class DropsRegistry
    {
        private static List<DropTable> _tables;

        static void EnsureLoaded()
        {
            if (_tables != null) return;
            _tables = new List<DropTable>(Resources.LoadAll<DropTable>("DropTables"));
        }

        public static DropTable Find(string family, int tier)
        {
            EnsureLoaded();
            DropTable best = null;
            foreach (var t in _tables)
            {
                if (!t) continue;
                bool famOk = string.IsNullOrEmpty(t.family) || t.family == family;
                bool tierOk = t.tier == 0 || t.tier == tier;
                if (famOk && tierOk)
                {
                    if (best == null) best = t;
                    else
                    {
                        // Prefer more specific (both family and tier specified)
                        int scoreBest = (string.IsNullOrEmpty(best.family) ? 0 : 1) + (best.tier == 0 ? 0 : 1);
                        int scoreCur = (string.IsNullOrEmpty(t.family) ? 0 : 1) + (t.tier == 0 ? 0 : 1);
                        if (scoreCur > scoreBest) best = t;
                    }
                }
            }
            return best;
        }
    }
}

