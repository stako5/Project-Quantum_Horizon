using System.IO;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Client.EditorTools
{
    public class SampleContentGenerator
    {
        [MenuItem("MMORPG/Generate Sample DropTables")] public static void GenDrops()
        {
            EnsureDir("Assets/Resources/DropTables");
            CreateDropTable("Rift Stag", 1, new[]{
                Entry("stag_hide",1,2,1f,1f), Entry("quill",1,3,0.6f,0.8f)
            }, 0.8f);
            CreateDropTable("Rift Stag", 2, new[]{
                Entry("stag_hide",2,3,1f,1f), Entry("quill",2,4,0.8f,0.8f), Entry("rift_core",1,1,0.4f,0.5f)
            }, 1.0f);
            CreateDropTable("Harmonic Jelly", 1, new[]{
                Entry("shock_gel",1,2,1f,1f), Entry("jelly_core",1,1,0.3f,0.5f)
            }, 0.7f);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        }

        [MenuItem("MMORPG/Generate Sample Local Quests")] public static void GenQuests()
        {
            EnsureDir("Assets/Resources/Quests");
            var q1 = ScriptableObject.CreateInstance<MMORPG.Client.Quests.Local.QuestDefinition>();
            q1.questId = "kill_stags"; q1.displayName = "Cull the Stags"; q1.description = "Thin the Rift Stag population.";
            q1.killFamily = "Rift Stag"; q1.killCount = 5; q1.rewardGold = 100; q1.rewardXP = 50;
            var q1p = PathFor("Assets/Resources/Quests/Quest_KillStags.asset"); AssetDatabase.CreateAsset(q1, q1p);

            var q2 = ScriptableObject.CreateInstance<MMORPG.Client.Quests.Local.QuestDefinition>();
            q2.questId = "collect_stag_hide"; q2.displayName = "Hunter's Request"; q2.description = "Bring back stag hides.";
            q2.collect.Add(new MMORPG.Client.Quests.Local.CollectRequirement{ itemId = "stag_hide", count = 3 });
            q2.rewardGold = 150; q2.rewardXP = 60;
            var q2p = PathFor("Assets/Resources/Quests/Quest_CollectHides.asset"); AssetDatabase.CreateAsset(q2, q2p);

            var list = ScriptableObject.CreateInstance<MMORPG.Client.Quests.Local.LocalQuestList>();
            list.quests.Add(q1); list.quests.Add(q2);
            var lp = PathFor("Assets/Resources/Quests/LocalQuests.asset"); AssetDatabase.CreateAsset(list, lp);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Selection.activeObject = list;
        }

        static void EnsureDir(string path){ if (!Directory.Exists(path)) Directory.CreateDirectory(path); }
        static string PathFor(string p){ return p.Replace('\\','/'); }

        static MMORPG.Client.Items.DropEntry Entry(string id,int min,int max,float weight,float chance)
        {
            return new MMORPG.Client.Items.DropEntry{ itemId = id, min = min, max = max, weight = weight, chance = chance };
        }

        static void CreateDropTable(string family,int tier, MMORPG.Client.Items.DropEntry[] entries, float avgDrops)
        {
            var t = ScriptableObject.CreateInstance<MMORPG.Client.Items.DropTable>();
            t.family = family; t.tier = tier; t.entries.AddRange(entries); t.avgDrops = avgDrops;
            var p = PathFor($"Assets/Resources/DropTables/{San(family)}_T{tier}.asset");
            AssetDatabase.CreateAsset(t, p);
        }
        static string San(string s){ foreach (var c in System.IO.Path.GetInvalidFileNameChars()){ s = s.Replace(c,'_'); } return s.Replace(' ','_'); }
    }
}

