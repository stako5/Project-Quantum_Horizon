using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Quests.Local
{
    [CreateAssetMenu(menuName = "MMORPG/Quests/Local Quest List", fileName = "LocalQuests")]
    public class LocalQuestList : ScriptableObject
    {
        public List<QuestDefinition> quests = new();
    }
}

