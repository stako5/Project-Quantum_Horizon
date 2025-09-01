using UnityEngine;

namespace MMORPG.Client.Player
{
    public class FPSCamera : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private Transform playerRoot; // yaw on player, pitch on camera
        [SerializeField] private Camera cam;

        [Header("Look")]
        [SerializeField] private float sensitivityX = 2.4f;
        [SerializeField] private float sensitivityY = 2.0f;
        [SerializeField] private bool invertY = false;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;
        [SerializeField] private bool lockCursor = true;
        [SerializeField] private FirstPersonConfig config;
        [SerializeField] private bool useConfig = true;

        private float _pitch;

        void Awake()
        {
            if (!playerRoot) playerRoot = transform.parent ? transform.parent : transform;
            if (!cam)
            {
                cam = GetComponentInChildren<Camera>();
                if (!cam)
                {
                    var go = new GameObject("FPS_Camera");
                    go.transform.SetParent(transform, false);
                    cam = go.AddComponent<Camera>();
                    cam.nearClipPlane = 0.03f;
                }
            }
            if (useConfig && !config) config = FirstPersonConfigSettings.Instance;
            ApplyCameraFromConfig();
            if (GetLockCursor()) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        }

        void Update()
        {
            float mx = Input.GetAxisRaw("Mouse X") * GetSensitivityX();
            float my = Input.GetAxisRaw("Mouse Y") * GetSensitivityY() * (GetInvertY() ? 1f : -1f);
            if (playerRoot) playerRoot.Rotate(Vector3.up, mx, Space.World);
            _pitch = Mathf.Clamp(_pitch + my, GetMinPitch(), GetMaxPitch());
            transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

            if (Input.GetKeyDown(KeyCode.Escape) && GetLockCursor())
            {
                Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            }
        }

        public void PublicApplyCameraFromConfig() => ApplyCameraFromConfig();

        void ApplyCameraFromConfig()
        {
            if (!(useConfig && config)) return;
            if (cam)
            {
                cam.nearClipPlane = config.nearClip;
                cam.fieldOfView = config.fov;
            }
        }

        float GetSensitivityX() => (useConfig && config) ? config.sensitivityX : sensitivityX;
        float GetSensitivityY() => (useConfig && config) ? config.sensitivityY : sensitivityY;
        bool GetInvertY() => (useConfig && config) ? config.invertY : invertY;
        float GetMinPitch() => (useConfig && config) ? config.minPitch : minPitch;
        float GetMaxPitch() => (useConfig && config) ? config.maxPitch : maxPitch;
        bool GetLockCursor() => (useConfig && config) ? config.lockCursor : lockCursor;
    }
}
