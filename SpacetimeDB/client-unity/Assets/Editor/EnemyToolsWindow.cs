using System.IO;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Client.EditorTools
{
    public class EnemyToolsWindow : EditorWindow
    {
        private const string ResourcesPath = "Assets/Resources";
        private const string AssetName = "EnemyStatScaling.asset";

        [MenuItem("MMORPG/Enemy Tools")] public static void Open() { GetWindow<EnemyToolsWindow>(false, "Enemy Tools", true); }

        void OnGUI()
        {
            GUILayout.Label("Enemy Utilities", EditorStyles.boldLabel);
            GUILayout.Space(6);
            if (GUILayout.Button("Create/Select Stat Scaling Asset"))
            {
                CreateOrSelectScalingAsset();
            }
            GUILayout.Space(6);
            EditorGUILayout.HelpBox("Creates a Resources/EnemyStatScaling.asset used by EnemyStatScaler. Select to edit multipliers and caps.", MessageType.Info);
        }

        static void CreateOrSelectScalingAsset()
        {
            string resPath = ResourcesPath;
            if (!Directory.Exists(resPath)) Directory.CreateDirectory(resPath);
            string assetPath = Path.Combine(resPath, AssetName).Replace('\\','/');
            var asset = AssetDatabase.LoadAssetAtPath<MMORPG.Client.Enemies.EnemyStatScaling>(assetPath);
            if (!asset)
            {
                asset = ScriptableObject.CreateInstance<MMORPG.Client.Enemies.EnemyStatScaling>();
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
            }
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
    }
}

