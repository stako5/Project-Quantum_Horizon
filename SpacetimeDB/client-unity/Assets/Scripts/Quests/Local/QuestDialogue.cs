using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Quests.Local
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea]
        public string text;
    }

    [CreateAssetMenu(menuName = "MMORPG/Quests/Quest Dialogue", fileName = "QuestDialogue")]
    public class QuestDialogue : ScriptableObject
    {
        public string questId;
        public string questName;
        [Header("Lines")]
        public List<DialogueLine> intro = new();
        public List<DialogueLine> inProgress = new();
        public List<DialogueLine> completion = new();
        public List<DialogueLine> afterCompletion = new();
    }
}

