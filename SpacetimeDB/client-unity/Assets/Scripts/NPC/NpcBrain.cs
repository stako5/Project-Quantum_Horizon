using UnityEngine;
using MMORPG.Client.AI;

namespace MMORPG.Client.NPC
{
    public class NpcBrain : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] private UtilityAgent agent;
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float chaseSpeed = 3.5f;
        [SerializeField] private float attackRange = 2.2f;

        private void Reset()
        {
            agent = GetComponent<UtilityAgent>();
        }

        private void OnValidate()
        {
            if (!agent) agent = GetComponent<UtilityAgent>();
        }
    }
}

