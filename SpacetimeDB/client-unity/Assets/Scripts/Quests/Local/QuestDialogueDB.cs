using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Quests.Local
{
    public static class QuestDialogueDB
    {
        private static Dictionary<string, QuestDialogue> _byId;

        static void Ensure()
        {
            if (_byId != null) return;
            _byId = new Dictionary<string, QuestDialogue>();
            var all = Resources.LoadAll<QuestDialogue>("Quests/Dialogues");
            foreach (var qd in all)
            {
                if (!qd || string.IsNullOrEmpty(qd.questId)) continue;
                _byId[qd.questId] = qd;
            }
        }

        public static QuestDialogue Get(string questId)
        {
            Ensure();
            if (string.IsNullOrEmpty(questId)) return null;
            _byId.TryGetValue(questId, out var qd); return qd;
        }
    }
}

