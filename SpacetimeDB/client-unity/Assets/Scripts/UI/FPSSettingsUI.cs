using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class FPSSettingsUI : MonoBehaviour
    {
        [SerializeField] private MMORPG.Client.Player.FirstPersonConfig config;
        [Header("Camera")]
        [SerializeField] private Slider sensitivityX;
        [SerializeField] private Slider sensitivityY;
        [SerializeField] private Toggle invertY;
        [SerializeField] private Slider fov;
        [Header("Movement")]
        [SerializeField] private Slider walkSpeed;
        [SerializeField] private Slider sprintSpeed;
        [Header("Prefs Keys")] public string pSensX = "fps.sensX"; public string pSensY = "fps.sensY"; public string pInvY = "fps.invertY"; public string pFov = "fps.fov"; public string pWalk = "fps.walk"; public string pSprint = "fps.sprint";

        void Awake()
        {
            if (!config) config = MMORPG.Client.Player.FirstPersonConfigSettings.Instance;
            if (!config) return;
            // Load saved overrides
            if (PlayerPrefs.HasKey(pSensX)) config.sensitivityX = PlayerPrefs.GetFloat(pSensX);
            if (PlayerPrefs.HasKey(pSensY)) config.sensitivityY = PlayerPrefs.GetFloat(pSensY);
            if (PlayerPrefs.HasKey(pInvY)) config.invertY = PlayerPrefs.GetInt(pInvY) != 0;
            if (PlayerPrefs.HasKey(pFov)) config.fov = PlayerPrefs.GetFloat(pFov);
            if (PlayerPrefs.HasKey(pWalk)) config.walkSpeed = PlayerPrefs.GetFloat(pWalk);
            if (PlayerPrefs.HasKey(pSprint)) config.sprintSpeed = PlayerPrefs.GetFloat(pSprint);

            // Initialize UI and bind
            if (sensitivityX) { sensitivityX.value = config.sensitivityX; sensitivityX.onValueChanged.AddListener(v => { config.sensitivityX = v; PlayerPrefs.SetFloat(pSensX, v); NotifyCameras(); }); }
            if (sensitivityY) { sensitivityY.value = config.sensitivityY; sensitivityY.onValueChanged.AddListener(v => { config.sensitivityY = v; PlayerPrefs.SetFloat(pSensY, v); NotifyCameras(); }); }
            if (invertY) { invertY.isOn = config.invertY; invertY.onValueChanged.AddListener(v => { config.invertY = v; PlayerPrefs.SetInt(pInvY, v ? 1 : 0); NotifyCameras(); }); }
            if (fov) { fov.value = config.fov; fov.onValueChanged.AddListener(v => { config.fov = v; PlayerPrefs.SetFloat(pFov, v); NotifyCameras(); }); }
            if (walkSpeed) { walkSpeed.value = config.walkSpeed; walkSpeed.onValueChanged.AddListener(v => { config.walkSpeed = v; PlayerPrefs.SetFloat(pWalk, v); }); }
            if (sprintSpeed) { sprintSpeed.value = config.sprintSpeed; sprintSpeed.onValueChanged.AddListener(v => { config.sprintSpeed = v; PlayerPrefs.SetFloat(pSprint, v); }); }
        }

        void NotifyCameras()
        {
            var cams = FindObjectsOfType<MMORPG.Client.Player.FPSCamera>();
            foreach (var c in cams) c.PublicApplyCameraFromConfig();
        }
    }
}
