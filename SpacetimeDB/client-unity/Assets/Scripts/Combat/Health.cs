using UnityEngine;

namespace MMORPG.Client.Combat
{
    public class Health : MonoBehaviour, IShieldConsumer
    {
        [SerializeField] public float maxHealth = 100f;
        [SerializeField] public float currentHealth = 100f;
        [SerializeField] public float iFrameDuration = 0.3f;
        [SerializeField] public float currentShield = 0f;
        [SerializeField] private BuffManager buffManager;

        private float _iFrameUntil;

        public bool IsAlive => currentHealth > 0f;
        public bool IsInvulnerable => Time.time < _iFrameUntil;

        public System.Action<float> OnDamaged; // amount post-shield
        public System.Action OnDied;

        void Awake()
        {
            if (!buffManager) buffManager = GetComponent<BuffManager>();
        }

        void Update()
        {
            if (buffManager != null && IsAlive)
            {
                var mods = buffManager.GetModifiers();
                if (mods.healthRegenFlat > 0f)
                {
                    currentHealth = Mathf.Min(maxHealth, currentHealth + mods.healthRegenFlat * Time.deltaTime);
                }
            }
        }

        public void ResetHealth(float value)
        {
            maxHealth = Mathf.Max(1f, value);
            currentHealth = maxHealth;
            _iFrameUntil = 0f;
        }

        public bool TryDamage(float amount)
        {
            if (IsInvulnerable || !IsAlive) return false;
            float incoming = Mathf.Max(0f, amount);
            if (buffManager != null)
            {
                var mods = buffManager.GetModifiers();
                // Apply shield first
                if (currentShield > 0f)
                {
                    float absorb = Mathf.Min(currentShield, incoming);
                    currentShield -= absorb; incoming -= absorb;
                }
                if (incoming > 0f)
                {
                    // Defense multiplier reduces incoming damage
                    if (mods.defenseMult > 0f) incoming /= mods.defenseMult;
                }
            }
            float prev = currentHealth;
            currentHealth = Mathf.Max(0f, currentHealth - incoming);
            float dealt = Mathf.Max(0f, prev - currentHealth);
            if (dealt > 0f) OnDamaged?.Invoke(dealt);
            if (prev > 0f && currentHealth <= 0f) OnDied?.Invoke();
            return dealt > 0f;
        }

        public void GrantIFrames()
        {
            _iFrameUntil = Time.time + Mathf.Max(0f, iFrameDuration);
        }

        public void ApplyShield(float amount)
        {
            currentShield += Mathf.Max(0f, amount);
        }
    }
}
