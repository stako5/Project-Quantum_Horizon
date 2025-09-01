using System.Linq;
using UnityEngine;

namespace MMORPG.Client.Enemies
{
    public class EnemyPerception : MonoBehaviour
    {
        [Header("Vision")] public float viewRadius = 20f; public float viewAngle = 120f;
        [Header("Hearing")] public float hearRadius = 10f;
        [Header("Layers")] public LayerMask obstacleMask = ~0; public string playerTag = "Player";

        public bool HasLineOfSight(Transform target)
        {
            if (!target) return false;
            Vector3 dir = (target.position - transform.position); dir.y = 0f;
            if (dir.sqrMagnitude > viewRadius * viewRadius) return false;
            if (Vector3.Angle(transform.forward, dir) > viewAngle * 0.5f) return false;
            if (Physics.Raycast(transform.position + Vector3.up * 1.6f, dir.normalized, out var hit, viewRadius, obstacleMask))
            {
                return hit.transform == target || hit.transform.IsChildOf(target);
            }
            return true;
        }

        public Transform FindPlayerTarget()
        {
            var players = GameObject.FindGameObjectsWithTag(playerTag);
            if (players == null || players.Length == 0) return null;
            var self = transform.position;
            // Prefer LOS, else nearest by hearing
            var vis = players.Select(p => p.transform).Where(t => HasLineOfSight(t));
            var bestVis = vis.OrderBy(t => (t.position - self).sqrMagnitude).FirstOrDefault();
            if (bestVis) return bestVis;
            var hear = players.Select(p => p.transform).Where(t => (t.position - self).sqrMagnitude <= hearRadius * hearRadius)
                .OrderBy(t => (t.position - self).sqrMagnitude).FirstOrDefault();
            return hear;
        }
    }
}

