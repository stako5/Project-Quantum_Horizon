using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Quests.Local
{
    [Serializable]
    public class CollectRequirement
    {
        public string itemId;
        public int count = 1;
    }

    [CreateAssetMenu(menuName = "MMORPG/Quests/Local Quest", fileName = "Quest")]
    public class QuestDefinition : ScriptableObject
    {
        public string questId;
        public string displayName;
        [TextArea] public string description;
        [Header("Objectives")]
        public string killFamily;
        public int killCount = 0;
        public List<CollectRequirement> collect = new();
        [Header("Reward")]
        public int rewardGold = 0;
        public int rewardXP = 0;
    }
}

