using UnityEngine;
using UnityEngine.AI;

namespace MMORPG.Client.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMover : MonoBehaviour
    {
        [Header("Defaults")] public float baseSpeed = 3.5f;
        public float rotationSpeed = 720f;
        public float stoppingDistance = 1.2f;
        [Header("Leash")]
        public float leashRadius = 35f;
        public Vector3 home;

        private NavMeshAgent _agent;
        private Combat.BuffManager _buffs;
        private Transform _target;
        private bool _returningHome;
        private bool _rooted;
        private Animator _anim;

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _buffs = GetComponent<Combat.BuffManager>();
            _anim = GetComponentInChildren<Animator>();
            if (home == Vector3.zero) home = transform.position;
            _agent.angularSpeed = rotationSpeed; _agent.stoppingDistance = stoppingDistance;
            UpdateSpeed();
        }

        void Update()
        {
            UpdateSpeed();
            HandleLeash();
        }

        void UpdateSpeed()
        {
            float spd = baseSpeed;
            if (_buffs != null)
            {
                var mods = _buffs.GetModifiers();
                spd *= Mathf.Clamp(mods.moveSpeedMult, 0.25f, 2.0f);
            }
            _agent.speed = spd;
        }

        void HandleLeash()
        {
            if (leashRadius <= 0f) return;
            float distHome = Vector3.Distance(transform.position, home);
            if (distHome > leashRadius && !_returningHome)
            {
                _returningHome = true; _target = null;
                _agent.SetDestination(home);
            }
            if (_returningHome && distHome <= Mathf.Max(1f, _agent.stoppingDistance + 0.2f))
            {
                _returningHome = false; _agent.ResetPath();
            }
            _agent.isStopped = _rooted || _returningHome ? true : _agent.isStopped;
            if (_anim) _anim.SetBool("ReturningHome", _returningHome);
            if (_anim) _anim.SetFloat("Speed", _agent.velocity.magnitude);
        }

        public void SetTarget(Transform t)
        {
            _target = t;
            if (t != null) _agent.SetDestination(t.position);
        }

        public void MoveTowards(Transform t)
        {
            if (t == null || _returningHome || _rooted) return;
            _agent.isStopped = false;
            _agent.SetDestination(t.position);
        }

        public void MoveInDirection(Vector3 worldDir)
        {
            if (_returningHome || _rooted) return;
            worldDir.y = 0f; if (worldDir.sqrMagnitude < 1e-4f) return;
            Vector3 dest = transform.position + worldDir.normalized * 2f; // short stride
            NavMeshHit hit;
            if (NavMesh.SamplePosition(dest, out hit, 1.5f, NavMesh.AllAreas))
            {
                _agent.isStopped = false; _agent.SetDestination(hit.position);
            }
        }

        public void Stop(float duration = 0f)
        {
            _agent.isStopped = true; if (duration > 0f) Invoke(nameof(Resume), duration);
        }
        void Resume() { _agent.isStopped = false; }

        public bool ReturningHome => _returningHome;

        public void SetRooted(bool rooted)
        {
            _rooted = rooted;
            _agent.isStopped = _rooted || _returningHome;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.6f);
            var center = new Vector3(home.x, transform.position.y, home.z);
            Gizmos.DrawWireSphere(center, leashRadius);
            Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.9f);
            Gizmos.DrawSphere(center, 0.25f);
            Gizmos.color = new Color(1f, 1f, 0.2f, 0.8f);
            Gizmos.DrawLine(transform.position, center);
        }
    }
}
