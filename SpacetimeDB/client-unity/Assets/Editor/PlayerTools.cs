using UnityEditor;
using UnityEngine;

namespace MMORPG.Client.EditorTools
{
    public static class PlayerTools
    {
        [MenuItem("MMORPG/Player/Convert Selected To FPS", true)]
        static bool ValidateConvert() => Selection.activeGameObject != null;

        [MenuItem("MMORPG/Player/Convert Selected To FPS")]
        static void ConvertSelected()
        {
            var go = Selection.activeGameObject;
            if (!go) return;
            Undo.RecordObject(go, "Convert To FPS");
            // Disable 3rd-person controller if present
            var tpc = go.GetComponent<MMORPG.Client.Player.PlayerController>();
            if (tpc) tpc.enabled = false;

            // Ensure CharacterController
            if (!go.GetComponent<CharacterController>()) go.AddComponent<CharacterController>();

            // Add FirstPersonController
            if (!go.GetComponent<MMORPG.Client.Player.FirstPersonController>()) go.AddComponent<MMORPG.Client.Player.FirstPersonController>();

            // Create/attach head and FPSCamera
            Transform head = go.transform.Find("Head");
            if (!head)
            {
                var headGo = new GameObject("Head");
                headGo.transform.SetParent(go.transform, false);
                headGo.transform.localPosition = new Vector3(0f, 1.65f, 0f);
                head = headGo.transform;
            }
            if (!head.GetComponent<MMORPG.Client.Player.FPSCamera>()) head.gameObject.AddComponent<MMORPG.Client.Player.FPSCamera>();

            EditorUtility.DisplayDialog("Convert To FPS", $"Converted '{go.name}' to FPS controller. You may want to disable old camera rigs.", "OK");
        }
    }
}

