using System.Linq;
using UnityEngine;
using MMORPG.Client.AI;

namespace MMORPG.Client.Enemies
{
    public class EnemyTargeting : MonoBehaviour
    {
        [SerializeField] private UtilityAgent agent;
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private float refreshInterval = 0.5f;
        [Header("Systems")]
        [SerializeField] private ThreatTable threat;
        [SerializeField] private EnemyPerception perception;
        [SerializeField] private EnemyMover mover;
        [Header("Leash")]
        [SerializeField] private float leashRadius = 35f;
        private Vector3 _home;
        private float _t;

        void Awake()
        {
            if (!agent) agent = GetComponent<UtilityAgent>();
            if (!threat) threat = GetComponent<ThreatTable>();
            if (!perception) perception = GetComponent<EnemyPerception>();
            if (!mover) mover = GetComponent<EnemyMover>();
            _home = transform.position;
        }

        void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
            if (!agent) return;
            // Leash reset: if too far, drop target and move home via mover
            if (_home != Vector3.zero && leashRadius > 0f)
            {
                if (Vector3.Distance(_home, transform.position) > leashRadius)
                {
                    agent.SetTarget(null);
                    if (mover) mover.SetTarget(null);
                }
            }

            Transform newTarget = null;
            // Prefer threat-selected target
            if (threat) newTarget = threat.GetHighest();
            // Else use perception to acquire
            if (!newTarget)
            {
                if (!perception) perception = GetComponent<EnemyPerception>();
                newTarget = perception ? perception.FindPlayerTarget() : null;
            }
            // If none, fallback to nearest player
            if (!newTarget)
            {
                var players = GameObject.FindGameObjectsWithTag(playerTag);
                if (players != null && players.Length > 0)
                {
                    var self = transform.position;
                    newTarget = players.OrderBy(p => (p.transform.position - self).sqrMagnitude).FirstOrDefault()?.transform;
                }
            }
            if (newTarget)
            {
                agent.SetTarget(newTarget);
                if (mover) mover.SetTarget(newTarget);
            }
        }
    }
}
