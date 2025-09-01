using UnityEngine;

namespace MMORPG.Client.Enemies
{
    public class EnemyStatScaler : MonoBehaviour
    {
        [Header("Override Caps (optional)")]
        public bool overrideCaps = false;
        public float minHP = 20f, maxHP = 10000f;
        public float minDMG = 5f, maxDMG = 1000f;
        public float minSPD = 0.5f, maxSPD = 12f;

        void Awake()
        {
            var id = GetComponent<EnemyIdentity>();
            if (!id) return;
            // Base values (fallbacks if zero)
            float baseHP = id.BaseHP > 0 ? id.BaseHP : 100f;
            float baseDMG = id.BaseDamage > 0 ? id.BaseDamage : 10f;
            float baseSPD = id.BaseSpeed > 0f ? id.BaseSpeed : 3.5f;

            var cfg = EnemyStatScalingSettings.Instance;

            float tierStepHP = cfg ? cfg.tierStepHP : 0.25f;
            float tierStepDMG = cfg ? cfg.tierStepDMG : 0.25f;
            float tierMulHP = 1f + tierStepHP * Mathf.Max(0, id.Tier - 1);
            float tierMulDMG = 1f + tierStepDMG * Mathf.Max(0, id.Tier - 1);
            float sizeHpMul = SizeHpMul(id.Size, cfg);
            float sizeDmgMul = SizeDmgMul(id.Size, cfg);
            float sizeSpdMul = SizeSpdMul(id.Size, cfg);
            float stageMul = StageMul(id.Stage, cfg);
            float roleDmgMul = RoleDmgMul(id.Role, cfg);

            float capMinHP = overrideCaps ? minHP : (cfg ? cfg.minHP : minHP);
            float capMaxHP = overrideCaps ? maxHP : (cfg ? cfg.maxHP : maxHP);
            float capMinDMG = overrideCaps ? minDMG : (cfg ? cfg.minDMG : minDMG);
            float capMaxDMG = overrideCaps ? maxDMG : (cfg ? cfg.maxDMG : maxDMG);
            float capMinSPD = overrideCaps ? minSPD : (cfg ? cfg.minSPD : minSPD);
            float capMaxSPD = overrideCaps ? maxSPD : (cfg ? cfg.maxSPD : maxSPD);

            float hp = Mathf.Clamp(baseHP * tierMulHP * sizeHpMul * stageMul, capMinHP, capMaxHP);
            float dmg = Mathf.Clamp(baseDMG * tierMulDMG * sizeDmgMul * stageMul * roleDmgMul, capMinDMG, capMaxDMG);
            float spd = Mathf.Clamp(baseSPD * sizeSpdMul, capMinSPD, capMaxSPD);

            var h = GetComponent<MMORPG.Client.Combat.Health>();
            if (h) h.ResetHealth(hp);

            var mover = GetComponent<EnemyMover>();
            if (mover) mover.baseSpeed = spd;

            var melee = GetComponent<EnemyMeleeAttack>();
            if (melee) melee.SetDamage(dmg);
        }

        static float SizeHpMul(string size, EnemyStatScaling cfg)
        {
            if (string.IsNullOrEmpty(size)) return 1f;
            if (cfg)
            {
                switch (size)
                {
                    case "Small": return cfg.hpSmall;
                    case "Big": return cfg.hpBig;
                    case "Large": return cfg.hpLarge;
                    case "Colossal": return cfg.hpColossal;
                }
            }
            switch (size) { case "Small": return 0.8f; case "Big": return 1.2f; case "Large": return 1.4f; case "Colossal": return 1.8f; default: return 1f; }
        }
        static float SizeDmgMul(string size, EnemyStatScaling cfg)
        {
            if (string.IsNullOrEmpty(size)) return 1f;
            if (cfg)
            {
                switch (size)
                {
                    case "Small": return cfg.dmgSmall;
                    case "Big": return cfg.dmgBig;
                    case "Large": return cfg.dmgLarge;
                    case "Colossal": return cfg.dmgColossal;
                }
            }
            switch (size) { case "Small": return 0.9f; case "Big": return 1.1f; case "Large": return 1.25f; case "Colossal": return 1.5f; default: return 1f; }
        }
        static float SizeSpdMul(string size, EnemyStatScaling cfg)
        {
            if (string.IsNullOrEmpty(size)) return 1f;
            if (cfg)
            {
                switch (size)
                {
                    case "Small": return cfg.spdSmall;
                    case "Big": return cfg.spdBig;
                    case "Large": return cfg.spdLarge;
                    case "Colossal": return cfg.spdColossal;
                }
            }
            switch (size) { case "Small": return 1.15f; case "Big": return 0.95f; case "Large": return 0.85f; case "Colossal": return 0.75f; default: return 1f; }
        }
        static float StageMul(string stage, EnemyStatScaling cfg)
        {
            if (string.IsNullOrEmpty(stage)) return 1f;
            if (cfg)
            {
                switch (stage)
                {
                    case "Infant": return cfg.stageInfant;
                    case "Adolescent": return cfg.stageAdolescent;
                    case "Adult": return cfg.stageAdult;
                }
            }
            switch (stage) { case "Infant": return 0.85f; case "Adolescent": return 1.0f; case "Adult": return 1.2f; default: return 1f; }
        }
        static float RoleDmgMul(string role, EnemyStatScaling cfg)
        {
            if (string.IsNullOrEmpty(role)) return 1f;
            if (cfg)
            {
                switch (role)
                {
                    case "assassin": return cfg.roleAssassin;
                    case "bruiser": return cfg.roleBruiser;
                    case "tank": return cfg.roleTank;
                    default: return cfg.roleDefault;
                }
            }
            switch (role) { case "assassin": return 1.2f; case "bruiser": return 1.1f; case "tank": return 0.9f; default: return 1f; }
        }
    }
}
