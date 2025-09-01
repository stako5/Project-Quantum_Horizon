using UnityEditor;
using UnityEngine;

public static class ConsumablesEditorTools
{
    [MenuItem("Tools/MMORPG/Add Consumables Bootstrapper")]
    public static void AddBootstrapper()
    {
        var existing = Object.FindObjectOfType<MMORPG.Client.Items.ConsumablesBootstrapper>();
        if (existing != null)
        {
            EditorUtility.DisplayDialog("Consumables", "A ConsumablesBootstrapper already exists in the scene.", "OK");
            return;
        }
        var go = new GameObject("ConsumablesBootstrapper");
        go.AddComponent<MMORPG.Client.Items.ConsumablesBootstrapper>();
        Selection.activeGameObject = go;
        EditorUtility.DisplayDialog("Consumables", "Added ConsumablesBootstrapper to the scene.", "OK");
    }
}

