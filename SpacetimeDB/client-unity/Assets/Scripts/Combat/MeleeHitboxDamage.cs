using System.Collections.Generic;
using UnityEngine;
using MMORPG.Client.Combat;

namespace MMORPG.Client.Combat
{
    [RequireComponent(typeof(Collider))]
    public class MeleeHitboxDamage : MonoBehaviour
    {
        [Header("Owner")]
        [SerializeField] private Transform ownerRoot;
        [SerializeField] private BuffManager ownerBuffs;
        [SerializeField] private Health ownerHealth;

        [Header("Damage")]
        [SerializeField] private float baseDamage = 25f;
        [SerializeField] private float baseCritChance = 0.05f; // 5%
        [SerializeField] private float baseCritDamage = 1.5f; // x1.5

        private readonly HashSet<Health> _hitThisSwing = new();

        void Awake()
        {
            if (!ownerBuffs && ownerRoot) ownerBuffs = ownerRoot.GetComponentInChildren<BuffManager>();
            if (!ownerHealth && ownerRoot) ownerHealth = ownerRoot.GetComponentInChildren<Health>();
            var col = GetComponent<Collider>(); col.isTrigger = true; // ensure trigger
        }

        void OnEnable() { _hitThisSwing.Clear(); }

        void OnTriggerEnter(Collider other)
        {
            var target = other.GetComponentInParent<Health>();
            if (!target || (ownerRoot && target.transform.IsChildOf(ownerRoot))) return;
            if (_hitThisSwing.Contains(target)) return;
            _hitThisSwing.Add(target);

            var mods = ownerBuffs ? ownerBuffs.GetModifiers() : StatsModifiers.Zero;
            float dmg = baseDamage * (mods.damageMult <= 0f ? 1f : mods.damageMult);
            float critChance = Mathf.Clamp01(baseCritChance + Mathf.Max(0f, mods.critChanceAdd));
            float critMult = Mathf.Max(1f, baseCritDamage * (mods.critDamageMult <= 0f ? 1f : mods.critDamageMult));
            bool crit = Random.value < critChance;
            float final = dmg * (crit ? critMult : 1f);

            if (target.TryDamage(final))
            {
                MMORPG.Client.Combat.HitStop.Impact(0.05f, 0.05f);
                // Threat generation to nearest ThreatTable on target
                var tt = target.GetComponentInParent<MMORPG.Client.Enemies.ThreatTable>();
                if (tt && ownerRoot) tt.AddThreat(ownerRoot, final);
                if (ownerHealth && mods.lifeStealPct > 0f)
                {
                    ownerHealth.currentHealth = Mathf.Min(ownerHealth.maxHealth, ownerHealth.currentHealth + final * Mathf.Clamp01(mods.lifeStealPct));
                }
            }
        }

        public void SetOwner(Transform root, BuffManager buffs, Health health)
        {
            ownerRoot = root; ownerBuffs = buffs; ownerHealth = health;
        }

        public void Configure(float dmg, float critChance = 0.05f, float critMult = 1.5f)
        {
            baseDamage = Mathf.Max(0f, dmg);
            baseCritChance = Mathf.Clamp01(critChance);
            baseCritDamage = Mathf.Max(1f, critMult);
        }
    }
}
