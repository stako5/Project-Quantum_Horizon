using UnityEngine;

namespace MMORPG.Client.Player
{
    public class PlayerHitReact : MonoBehaviour
    {
        [SerializeField] private bool enableHitReact = true;
        [SerializeField] private string triggerName = "HitReact";
        [SerializeField] private float cooldown = 0.1f;

        private Animator _anim;
        private Combat.Health _hp;
        private float _nextAllow;

        void Awake()
        {
            _anim = GetComponentInChildren<Animator>();
            _hp = GetComponentInChildren<Combat.Health>();
            if (_hp) _hp.OnDamaged += OnDamaged;
        }

        void OnDestroy()
        {
            if (_hp) _hp.OnDamaged -= OnDamaged;
        }

        void OnDamaged(float amt)
        {
            if (!enableHitReact) return;
            if (Time.time < _nextAllow) return;
            _nextAllow = Time.time + cooldown;
            if (_anim && !string.IsNullOrEmpty(triggerName)) _anim.SetTrigger(triggerName);
        }
    }
}

