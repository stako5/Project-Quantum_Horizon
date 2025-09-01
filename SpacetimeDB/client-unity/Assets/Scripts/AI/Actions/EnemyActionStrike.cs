using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.Enemies;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/Enemy/Strike")]
    public class EnemyActionStrike : UtilityAction
    {
        public float preferredRange = 1.8f;
        public float moveSpeed = 3.5f;
        public EnemyActionStrike(){ cooldownSeconds = 0.9f; minActiveSeconds = 0.25f; windupSeconds = 0.2f; activeSeconds = 0.25f; recoverySeconds = 0.25f; rootDuringWindup = true; rootDuringRecovery = true; }
        public override float Score(UtilityContext ctx)
        {
            if (!ctx.target) return 0f;
            float d = ctx.distToTarget;
            // Prefer close-in engagements; extraversion increases aggression
            var ep = ctx.self.GetComponent<EnemyPersonality>();
            float e = ep ? ep.extraversion : 0.5f;
            float baseScore = Mathf.Clamp01(1f - Mathf.Abs(d - preferredRange)/preferredRange);
            float noise = Random.Range(0f, 0.2f) * (ep ? ep.openness : 0.5f);
            return (baseScore * (0.8f + e*0.4f) + noise) * weight;
        }
        public override void Execute(UtilityContext ctx, float dt)
        {
            if (!ctx.target) return;
            if (ctx.phase == MMORPG.Client.AI.ActionPhase.Windup) return; // brief telegraph
            var mover = ctx.self.GetComponent<MMORPG.Client.Enemies.EnemyMover>();
            if (mover) mover.MoveTowards(ctx.target);
            else
            {
                var dir = (ctx.target.position - ctx.self.position); dir.y = 0f;
                ctx.self.position += dir.normalized * moveSpeed * dt;
            }
        }
        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag) { valence = 0.1f; tag = "strike"; }
    }
}
