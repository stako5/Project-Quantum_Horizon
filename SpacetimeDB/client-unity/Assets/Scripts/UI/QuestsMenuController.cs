using UnityEngine;

namespace MMORPG.Client.UI
{
    public class QuestsMenuController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup panel;
        [SerializeField] private KeyCode toggleKey = KeyCode.J;
        [SerializeField] private bool startHidden = true;

        void Awake()
        {
            if (!panel) panel = GetComponent<CanvasGroup>();
            if (panel && startHidden) Hide();
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(toggleKey)) Toggle();
        }

        public void Toggle()
        {
            if (!panel) return;
            if (panel.alpha > 0.5f) Hide(); else Show();
        }

        public void Show()
        {
            if (!panel) return;
            panel.alpha = 1f; panel.blocksRaycasts = true; panel.interactable = true;
        }

        public void Hide()
        {
            if (!panel) return;
            panel.alpha = 0f; panel.blocksRaycasts = false; panel.interactable = false;
        }
    }
}

