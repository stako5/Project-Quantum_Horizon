using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.Combat;
using MMORPG.Client.VFX;

namespace MMORPG.Client.Enemies
{
    // Attaches a melee hitbox for Strike/Lunge actions and shows windup telegraphs.
    public class EnemyMeleeAttack : MonoBehaviour
    {
        [Header("Hitbox")]
        public float hitboxRadius = 0.75f;
        public float forwardOffset = 1.0f;
        public float damage = 18f;
        [Header("Telegraph")]
        public float meleeRange = 2.0f;
        public Color telegraphColor = new Color(1f, 0f, 0f, 0.85f);

        private UtilityAgent _agent;
        private Health _ownerHealth;
        private BuffManager _ownerBuffs;
        private GameObject _hitboxGO;
        private SphereCollider _col;
        private MeleeHitboxDamage _mhd;
        private bool _telegraphedThisWindup;

        void Awake()
        {
            _agent = GetComponent<UtilityAgent>();
            _ownerHealth = GetComponent<Health>();
            _ownerBuffs = GetComponent<BuffManager>();
            EnsureHitbox();
            if (_agent) _agent.ActionChanged += OnActionChanged;
        }

        void OnDestroy()
        {
            if (_agent) _agent.ActionChanged -= OnActionChanged;
        }

        void EnsureHitbox()
        {
            _hitboxGO = new GameObject("EnemyHitbox");
            _hitboxGO.transform.SetParent(transform, false);
            _hitboxGO.transform.localPosition = new Vector3(0f, 0.9f, forwardOffset);
            _col = _hitboxGO.AddComponent<SphereCollider>();
            _col.isTrigger = true; _col.radius = hitboxRadius;
            _mhd = _hitboxGO.AddComponent<MeleeHitboxDamage>();
            _mhd.SetOwner(transform, _ownerBuffs, _ownerHealth);
            _mhd.Configure(damage, 0.05f, 1.5f);
            _col.enabled = false;
        }

        void OnActionChanged(UtilityAction prev, UtilityAction next)
        {
            _telegraphedThisWindup = false;
            DisableHitbox();
        }

        void Update()
        {
            if (_agent == null) return;
            var act = _agent.Current;
            var ctx = _agent.Context;
            bool isMelee = act is MMORPG.Client.AI.Actions.EnemyActionStrike || act is MMORPG.Client.AI.Actions.EnemyActionLunge;
            if (!isMelee)
            {
                DisableHitbox(); return;
            }
            if (ctx.phase == ActionPhase.Windup && !_telegraphedThisWindup)
            {
                _telegraphedThisWindup = true;
                var from = transform.position;
                var to = from + transform.forward * Mathf.Max(meleeRange, forwardOffset + hitboxRadius);
                Telegraph.FlashLine(from, to, telegraphColor, 0.08f, Mathf.Max(0.05f, act.windupSeconds));
                // Ground cone telegraph for clarity
                MMORPG.Client.VFX.GroundTelegraph.Cone(transform.position, transform.forward, 60f, Mathf.Max(meleeRange, forwardOffset + hitboxRadius + 0.5f), new Color(1f,0.1f,0f,0.65f), Mathf.Max(0.05f, act.windupSeconds));
            }
            if (ctx.phase == ActionPhase.Active) EnableHitbox(); else DisableHitbox();
        }

        void EnableHitbox()
        {
            if (_col) _col.enabled = true;
        }
        void DisableHitbox()
        {
            if (_col) _col.enabled = false;
        }

        public void SetDamage(float dmg)
        {
            damage = Mathf.Max(0f, dmg);
            if (_mhd) _mhd.Configure(damage);
        }
    }
}
