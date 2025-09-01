using System.IO;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Client.EditorTools
{
    public static class FPSConfigTools
    {
        [MenuItem("MMORPG/Player/Create/Select FPS Config")] public static void CreateOrSelect()
        {
            string resDir = "Assets/Resources";
            if (!Directory.Exists(resDir)) Directory.CreateDirectory(resDir);
            string assetPath = Path.Combine(resDir, "FirstPersonConfig.asset").Replace('\\','/');
            var asset = AssetDatabase.LoadAssetAtPath<MMORPG.Client.Player.FirstPersonConfig>(assetPath);
            if (!asset)
            {
                asset = ScriptableObject.CreateInstance<MMORPG.Client.Player.FirstPersonConfig>();
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
            }
            Selection.activeObject = asset; EditorGUIUtility.PingObject(asset);
        }
    }
}

