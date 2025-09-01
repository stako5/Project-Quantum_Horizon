using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Items
{
    [Serializable]
    public class DropEntry
    {
        public string itemId;
        public int min = 1;
        public int max = 1;
        public float weight = 1f;
        public float chance = 1f; // independent chance to drop this entry (post-roll)
    }

    [CreateAssetMenu(menuName = "MMORPG/Items/Drop Table", fileName = "DropTable")]
    public class DropTable : ScriptableObject
    {
        public string family = ""; // optional
        public int tier = 0; // 0 = any
        public List<DropEntry> entries = new();
        [Range(0f, 10f)] public float avgDrops = 0.6f; // average number of items per kill
    }
}

