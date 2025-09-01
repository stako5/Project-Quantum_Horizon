using UnityEngine;

namespace MMORPG.Client.Player
{
    [CreateAssetMenu(menuName = "MMORPG/Player/First Person Config", fileName = "FirstPersonConfig")]
    public class FirstPersonConfig : ScriptableObject
    {
        [Header("Movement")]
        public float walkSpeed = 4.2f;
        public float sprintSpeed = 6.8f;
        public float gravity = -18f;
        public float jumpSpeed = 5.2f;

        [Header("Stamina")]
        public float maxStamina = 100f;
        public float staminaRegen = 20f;
        public float jumpCost = 15f;
        public float attackCost = 20f;

        [Header("Camera")]
        public float sensitivityX = 2.4f;
        public float sensitivityY = 2.0f;
        public bool invertY = false;
        public float minPitch = -80f;
        public float maxPitch = 80f;
        public bool lockCursor = true;
        public float nearClip = 0.03f;
        public float fov = 70f;
    }

    public static class FirstPersonConfigSettings
    {
        private static FirstPersonConfig _instance;
        public static FirstPersonConfig Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = Resources.Load<FirstPersonConfig>("FirstPersonConfig");
                return _instance;
            }
        }
    }
}

