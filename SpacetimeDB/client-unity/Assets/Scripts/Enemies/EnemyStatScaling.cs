using UnityEngine;

namespace MMORPG.Client.Enemies
{
    [CreateAssetMenu(menuName = "MMORPG/Enemies/Stat Scaling", fileName = "EnemyStatScaling")]
    public class EnemyStatScaling : ScriptableObject
    {
        [Header("Tier Steps (per tier above 1)")]
        public float tierStepHP = 0.25f;
        public float tierStepDMG = 0.25f;

        [Header("Size Multipliers — HP")]
        public float hpSmall = 0.8f;
        public float hpBig = 1.2f;
        public float hpLarge = 1.4f;
        public float hpColossal = 1.8f;

        [Header("Size Multipliers — DMG")]
        public float dmgSmall = 0.9f;
        public float dmgBig = 1.1f;
        public float dmgLarge = 1.25f;
        public float dmgColossal = 1.5f;

        [Header("Size Multipliers — SPD")]
        public float spdSmall = 1.15f;
        public float spdBig = 0.95f;
        public float spdLarge = 0.85f;
        public float spdColossal = 0.75f;

        [Header("Stage Multipliers (HP & DMG)")]
        public float stageInfant = 0.85f;
        public float stageAdolescent = 1.0f;
        public float stageAdult = 1.2f;

        [Header("Role Damage Multipliers")]
        public float roleAssassin = 1.2f;
        public float roleBruiser = 1.1f;
        public float roleTank = 0.9f;
        public float roleDefault = 1.0f;

        [Header("Hard Caps")]
        public float minHP = 20f, maxHP = 10000f;
        public float minDMG = 5f, maxDMG = 1000f;
        public float minSPD = 0.5f, maxSPD = 12f;
    }

    public static class EnemyStatScalingSettings
    {
        private static EnemyStatScaling _instance;
        public static EnemyStatScaling Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = Resources.Load<EnemyStatScaling>("EnemyStatScaling");
                return _instance;
            }
        }
    }
}

