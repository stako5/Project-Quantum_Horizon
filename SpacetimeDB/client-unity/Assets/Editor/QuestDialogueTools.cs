using System.IO;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Client.EditorTools
{
    public static class QuestDialogueTools
    {
        [MenuItem("MMORPG/Quests/Create Sample Dialogues")] public static void CreateSampleDialogues()
        {
            string dir = "Assets/Resources/Quests/Dialogues";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            void Make(string questId, string questName, (string,string)[] intro, (string,string)[] inProg, (string,string)[] complete)
            {
                var qd = ScriptableObject.CreateInstance<MMORPG.Client.Quests.Local.QuestDialogue>();
                qd.questId = questId; qd.questName = questName;
                foreach (var l in intro) qd.intro.Add(new MMORPG.Client.Quests.Local.DialogueLine{ speaker = l.Item1, text = l.Item2 });
                foreach (var l in inProg) qd.inProgress.Add(new MMORPG.Client.Quests.Local.DialogueLine{ speaker = l.Item1, text = l.Item2 });
                foreach (var l in complete) qd.completion.Add(new MMORPG.Client.Quests.Local.DialogueLine{ speaker = l.Item1, text = l.Item2 });
                string path = Path.Combine(dir, questId + ".asset").Replace('\\','/');
                AssetDatabase.CreateAsset(qd, path);
            }

            Make("kill_stags", "Cull the Stags",
                new[]{ ("Hunter", "The rift stags breed too fast. Thin their numbers."), ("You", "I'll cull a few and be back.") },
                new[]{ ("Hunter", "How goes the cull? Keep your distance and strike true.") },
                new[]{ ("Hunter", "Good work. Fewer hooves out there means safer trails."), ("You", "Here's what I managed.") });

            Make("collect_stag_hide", "Hunter's Request",
                new[]{ ("Hunter", "Bring me stag hides. I'll put them to use."), ("You", "How many do you need?") },
                new[]{ ("Hunter", "Hides toughen with age. Find the sturdier ones if you can.") },
                new[]{ ("Hunter", "These will do nicely. Take this for your trouble."), ("You", "Pleasure doing business.") });

            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Quest Dialogues", "Created sample quest dialogues in Resources/Quests/Dialogues.", "OK");
        }
    }
}

