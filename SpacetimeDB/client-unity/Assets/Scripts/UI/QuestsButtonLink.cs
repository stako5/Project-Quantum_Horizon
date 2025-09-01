using UnityEngine;

namespace MMORPG.Client.UI
{
    // Attach to a UI Button and wire its OnClick to CallToggle()
    public class QuestsButtonLink : MonoBehaviour
    {
        [SerializeField] private QuestsMenuController controller;

        void Awake()
        {
            if (!controller) controller = FindObjectOfType<QuestsMenuController>();
        }

        public void CallToggle()
        {
            if (controller) controller.Toggle();
        }
    }
}

