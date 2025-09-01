using System.Collections.Generic;
using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.AI.Actions;

namespace MMORPG.Client.Enemies
{
    // Ensures an enemy has a UtilityAgent + default actions and targeting.
    public class EnemyBrain : MonoBehaviour
    {
        [Tooltip("If true, creates a default action set at runtime.")]
        [SerializeField] private bool spawnDefaultActions = true;
        [SerializeField] private float thinkInterval = 0.25f;

        private UtilityAgent _agent;

        void Awake()
        {
            _agent = GetComponent<UtilityAgent>();
            if (!_agent) _agent = gameObject.AddComponent<UtilityAgent>();

            // Ensure targeting exists so the agent can chase a player.
            if (!GetComponent<EnemyPerception>()) gameObject.AddComponent<EnemyPerception>();
            if (!GetComponent<ThreatTable>()) gameObject.AddComponent<ThreatTable>();
            if (!GetComponent<EnemyMover>()) gameObject.AddComponent<EnemyMover>();
            if (!GetComponent<EnemyTargeting>()) gameObject.AddComponent<EnemyTargeting>();
            if (!GetComponent<EnemyMeleeAttack>()) gameObject.AddComponent<EnemyMeleeAttack>();
            if (!GetComponent<EnemyStatScaler>()) gameObject.AddComponent<EnemyStatScaler>();

            // Set think interval if exposed.
            TrySetThinkInterval(_agent, thinkInterval);

            if (spawnDefaultActions)
            {
                var acts = CreateDefaultActions();
                _agent.SetActions(acts);
            }
        }

        List<UtilityAction> CreateDefaultActions()
        {
            var list = new List<UtilityAction>();
            // Instantiate runtime ScriptableObjects for actions
            var strike = ScriptableObject.CreateInstance<EnemyActionStrike>();
            strike.weight = 1f;
            var lunge = ScriptableObject.CreateInstance<EnemyActionLunge>();
            lunge.weight = 0.95f;
            var backstep = ScriptableObject.CreateInstance<EnemyActionBackstep>();
            backstep.weight = 0.9f;
            var roar = ScriptableObject.CreateInstance<EnemyActionRoar>();
            roar.weight = 0.6f;
            list.Add(strike);
            list.Add(lunge);
            list.Add(backstep);
            list.Add(roar);
            // Optional: add ranged shot if available in project
            var ranged = ScriptableObject.CreateInstance<EnemyActionRangedShot>();
            ranged.weight = 0.7f;
            list.Add(ranged);
            return list;
        }

        static void TrySetThinkInterval(UtilityAgent agent, float interval)
        {
            // UtilityAgent.thinkInterval is private; provide setter via reflection fallback
            var f = typeof(UtilityAgent).GetField("thinkInterval", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (f != null) f.SetValue(agent, interval);
        }
    }
}
