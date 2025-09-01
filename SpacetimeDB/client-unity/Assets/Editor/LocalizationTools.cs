using System.IO;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Client.EditorTools
{
    public static class LocalizationTools
    {
        [MenuItem("MMORPG/Localization/Create/Select Config")] public static void CreateOrSelectConfig()
        {
            string resDir = "Assets/Resources";
            if (!Directory.Exists(resDir)) Directory.CreateDirectory(resDir);
            string assetPath = Path.Combine(resDir, "LocalizationConfig.asset").Replace('\\','/');
            var asset = AssetDatabase.LoadAssetAtPath<MMORPG.Client.Localization.LocalizationConfig>(assetPath);
            if (!asset)
            {
                asset = ScriptableObject.CreateInstance<MMORPG.Client.Localization.LocalizationConfig>();
                asset.languages = new System.Collections.Generic.List<string> { "en", "es" };
                asset.defaultLanguage = "en";
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
            }
            Selection.activeObject = asset; EditorGUIUtility.PingObject(asset);
        }

        [MenuItem("MMORPG/Localization/Create Sample Table")] public static void CreateSampleTable()
        {
            string resLoc = "Assets/Resources/Localization";
            if (!Directory.Exists(resLoc)) Directory.CreateDirectory(resLoc);
            var table = ScriptableObject.CreateInstance<MMORPG.Client.Localization.LocalizationTable>();
            void Add(string key, string en, string es)
            {
                var e = new MMORPG.Client.Localization.LocalizationEntry { key = key };
                e.values.Add(new MMORPG.Client.Localization.LocalizedValue { language = "en", value = en });
                e.values.Add(new MMORPG.Client.Localization.LocalizedValue { language = "es", value = es });
                table.entries.Add(e);
            }
            Add("title", "MMORPG", "MMORPG");
            Add("quality_header", "Quality", "Calidad");
            Add("preset_label", "Preset", "Preajuste");
            Add("display_header", "Display", "Pantalla");
            Add("resolution_label", "Resolution", "Resolución");
            Add("fullscreen_label", "Fullscreen", "Pantalla completa");
            Add("vsync_label", "VSync", "VSync");
            Add("first_person_header", "First-Person", "Primera persona");
            Add("sensitivity_x", "Sensitivity X", "Sensibilidad X");
            Add("sensitivity_y", "Sensitivity Y", "Sensibilidad Y");
            Add("fov", "FOV", "FOV");
            Add("invert_y", "Invert Y", "Invertir Y");
            Add("language_header", "Language", "Idioma");
            Add("language_label", "Language", "Idioma");
            Add("start scene", "Start Scene", "Escena inicial");
            Add("play", "Play", "Jugar");
            Add("continue", "Continue", "Continuar");
            Add("quit", "Quit", "Salir");
            Add("quests", "Quests", "Misiones");
            Add("gold", "Gold", "Oro");
            Add("kill_label", "Kill", "Matar");
            Add("collect_label", "Collect", "Recolectar");
            Add("click_to_claim", "Click to Claim", "Haz clic para reclamar");
            Add("claimed", "Claimed", "Reclamada");

            string path = Path.Combine(resLoc, "SampleTable.asset").Replace('\\','/');
            AssetDatabase.CreateAsset(table, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = table; EditorGUIUtility.PingObject(table);
        }
    }
}
