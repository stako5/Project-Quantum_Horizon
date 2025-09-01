using UnityEngine;

namespace MMORPG.Client.Boot
{
    // Optional runtime helper to ensure a selected player uses FPS components.
    public class AutoFPSSetup : MonoBehaviour
    {
        [Tooltip("If not assigned, tries to find by Player tag or PlayerController in scene.")]
        public Transform playerRoot;
        public bool disableThirdPersonController = true;
        public bool addFPSCameraHead = true;
        public Vector3 headLocalPosition = new Vector3(0f, 1.65f, 0f);

        void Awake()
        {
            if (!playerRoot)
            {
                var tagged = GameObject.FindGameObjectWithTag("Player");
                if (tagged) playerRoot = tagged.transform;
            }
            if (!playerRoot)
            {
                var tpc = FindObjectOfType<MMORPG.Client.Player.PlayerController>();
                if (tpc) playerRoot = tpc.transform;
            }
            if (!playerRoot) return;

            // Disable third-person controller if present
            if (disableThirdPersonController)
            {
                var tpc = playerRoot.GetComponent<MMORPG.Client.Player.PlayerController>();
                if (tpc) tpc.enabled = false;
            }

            // Ensure CharacterController
            var cc = playerRoot.GetComponent<CharacterController>();
            if (!cc) cc = playerRoot.gameObject.AddComponent<CharacterController>();

            // Ensure FirstPersonController
            if (!playerRoot.GetComponent<MMORPG.Client.Player.FirstPersonController>())
            {
                playerRoot.gameObject.AddComponent<MMORPG.Client.Player.FirstPersonController>();
            }

            // Ensure FPS head + camera
            if (addFPSCameraHead)
            {
                Transform head = playerRoot.Find("Head");
                if (!head)
                {
                    var headGo = new GameObject("Head");
                    headGo.transform.SetParent(playerRoot, false);
                    headGo.transform.localPosition = headLocalPosition;
                    head = headGo.transform;
                }
                if (!head.GetComponent<MMORPG.Client.Player.FPSCamera>()) head.gameObject.AddComponent<MMORPG.Client.Player.FPSCamera>();
            }
        }
    }
}

