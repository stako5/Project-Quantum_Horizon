using UnityEngine;
using UnityEngine.AI;
using MMORPG.Client.AI;
using MMORPG.Client.Enemies;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/Enemy/RangedShot")]
    public class EnemyActionRangedShot : UtilityAction
    {
        public float preferredMin = 4f;
        public float preferredMax = 12f;
        public float strafeSpeed = 2.2f;
        public float fireRate = 0.8f; // shots per second
        public float projectileSpeed = 14f;
        public float projectileDamage = 10f;
        public float telegraphLead = 0.15f;
        private float _nextFire;
        private bool _telegraphed;
        public EnemyActionRangedShot(){ cooldownSeconds = 0.6f; minActiveSeconds = 0.25f; windupSeconds = 0.1f; activeSeconds = 0.3f; recoverySeconds = 0.2f; rootDuringWindup = true; }

        public override float Score(UtilityContext ctx)
        {
            if (!ctx.target) return 0f;
            float d = ctx.distToTarget;
            var ep = ctx.self.GetComponent<EnemyPersonality>();
            float o = ep ? ep.openness : 0.5f; float c = ep ? ep.conscientiousness : 0.5f;
            // Prefer mid-long range; conscientiousness reduces spam a bit; openness adds variance
            float inRange = Mathf.InverseLerp(preferredMax, preferredMin, Mathf.Abs(d - (preferredMin + preferredMax) * 0.5f));
            float baseScore = Mathf.Clamp01(1f - inRange);
            float noise = Random.Range(0f, 0.25f) * o;
            return (baseScore * (0.7f + (1f - c) * 0.3f) + noise) * weight;
        }

        public override void Execute(UtilityContext ctx, float dt)
        {
            if (!ctx.target) return;
            // Simple strafing with NavMesh sampling to avoid walls (choose better side)
            var toTarget = (ctx.target.position - ctx.self.position); toTarget.y = 0f;
            var baseDir = toTarget.sqrMagnitude > 1e-4f ? toTarget.normalized : ctx.self.forward;
            var left = Vector3.Cross(Vector3.up, baseDir);
            var right = -left;
            float dist = Mathf.Max(1.5f, strafeSpeed * 0.5f);
            float scoreL = SampleNavScore(ctx.self.position + left * dist);
            float scoreR = SampleNavScore(ctx.self.position + right * dist);
            var side = scoreL >= scoreR ? left : right;
            var mover = ctx.self.GetComponent<MMORPG.Client.Enemies.EnemyMover>();
            if (mover) mover.MoveInDirection(side);
            else ctx.self.position += side * strafeSpeed * dt;
            // Telegraph shortly before firing
            if (!_telegraphed && Time.time >= _nextFire - Mathf.Max(0f, telegraphLead))
            {
                _telegraphed = true;
                var from = ctx.self.position;
                var to = ctx.target.position;
                MMORPG.Client.VFX.Telegraph.FlashLine(from, to, new Color(1f, 0.3f, 0f, 0.8f), 0.06f, telegraphLead);
            }
            // Fire projectiles on cadence
            if (Time.time >= _nextFire)
            {
                _nextFire = Time.time + Mathf.Max(0.1f, 1f / Mathf.Max(0.01f, fireRate));
                _telegraphed = false;
                MMORPG.Client.VFX.MuzzleFlash.Spawn(ctx.self);
                MMORPG.Client.Combat.ProjectileShooter.FireSimple(ctx.self, toTarget.normalized, projectileSpeed, projectileDamage);
            }
        }

        float SampleNavScore(Vector3 pos)
        {
            NavMeshHit hit; if (NavMesh.SamplePosition(pos, out hit, 1.0f, NavMesh.AllAreas)) return 1f - hit.distance; return 0f;
        }

        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag) { valence = 0.09f; tag = "rshot"; }
    }
}
