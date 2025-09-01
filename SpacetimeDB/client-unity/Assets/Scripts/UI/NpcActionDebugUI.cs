using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.AI;

namespace MMORPG.Client.UI
{
    public class NpcActionDebugUI : MonoBehaviour
    {
        [SerializeField] private UtilityAgent agent;
        [SerializeField] private Text label;

        void Awake()
        {
            if (!agent) agent = FindObjectOfType<UtilityAgent>();
        }

        void OnEnable()
        {
            if (agent != null) agent.ActionChanged += OnChanged;
        }

        void OnDisable()
        {
            if (agent != null) agent.ActionChanged -= OnChanged;
        }

        void OnChanged(UtilityAction prev, UtilityAction next)
        {
            if (!label) return;
            var name = next != null ? next.Name : "(none)";
            label.text = $"Action: {name}";
        }
    }
}
