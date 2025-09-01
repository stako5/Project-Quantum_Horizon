using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class InstanceHUD : MonoBehaviour
    {
        [SerializeField] private Text statusText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeSpeed = 4f;
        [SerializeField] private KeyCode exitKey = KeyCode.K;

        private float _targetAlpha = 0f;
        private bool _inInstance = false;

        void Awake()
        {
            if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        void Update()
        {
            if (canvasGroup) canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, _targetAlpha, Time.deltaTime * fadeSpeed);
            if (_inInstance && Input.GetKeyDown(exitKey))
            {
                var mgr = MMORPG.Client.Dungeon.DungeonInstanceManager.Instance;
                if (mgr) mgr.ExitInstance();
            }
        }

        public void ShowEnter(string bossName)
        {
            _inInstance = true;
            if (statusText) statusText.text = $"Entering Dungeon: {bossName}\nPress [{exitKey}] to Exit";
            _targetAlpha = 1f;
        }

        public void ShowExit()
        {
            _inInstance = false;
            if (statusText) statusText.text = "Exited Dungeon";
            _targetAlpha = 1f;
            Invoke(nameof(Hide), 1.5f);
        }

        public void Hide()
        {
            _targetAlpha = 0f;
        }
    }
}

