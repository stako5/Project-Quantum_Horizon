using System.Linq;
using UnityEngine;

namespace MMORPG.Client.Combat
{
    public class LockOnSystem : MonoBehaviour
    {
        [SerializeField] private float radius = 20f;
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private Transform lockOnReticle;

        public Transform CurrentTarget { get; private set; }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (CurrentTarget == null) AcquireTarget();
                else CurrentTarget = null;
            }

            if (lockOnReticle != null)
            {
                lockOnReticle.gameObject.SetActive(CurrentTarget != null);
                if (CurrentTarget != null)
                {
                    lockOnReticle.position = CurrentTarget.position + Vector3.up * 2f;
                }
            }
        }

        void AcquireTarget()
        {
            var candidates = GameObject.FindGameObjectsWithTag(enemyTag)
                .Select(go => go.transform)
                .Where(t => Vector3.Distance(transform.position, t.position) <= radius)
                .OrderBy(t => Vector3.Angle(transform.forward, (t.position - transform.position).normalized));
            CurrentTarget = candidates.FirstOrDefault();
        }
    }
}

