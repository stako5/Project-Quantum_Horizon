using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.Enemies;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/Enemy/Lunge")]
    public class EnemyActionLunge : UtilityAction
    {
        public float preferMax = 5f;
        public float burstSpeed = 6f;
        public EnemyActionLunge(){ cooldownSeconds = 1.2f; minActiveSeconds = 0.2f; windupSeconds = 0.15f; activeSeconds = 0.3f; recoverySeconds = 0.25f; rootDuringWindup = true; }
        public override float Score(UtilityContext ctx)
        {
            if (!ctx.target) return 0f;
            float d = ctx.distToTarget;
            var ep = ctx.self.GetComponent<EnemyPersonality>();
            float o = ep ? ep.openness : 0.5f; float e = ep ? ep.extraversion : 0.5f;
            float mid = Mathf.InverseLerp(1f, preferMax, d);
            float noise = Random.Range(0f, 0.3f) * o;
            return (mid * (0.6f + e*0.6f) + noise) * weight;
        }
        public override void Execute(UtilityContext ctx, float dt)
        {
            if (!ctx.target) return;
            if (ctx.phase == MMORPG.Client.AI.ActionPhase.Windup) return;
            var mover = ctx.self.GetComponent<MMORPG.Client.Enemies.EnemyMover>();
            if (mover) mover.MoveTowards(ctx.target);
            else
            {
                var dir = (ctx.target.position - ctx.self.position); dir.y = 0f;
                ctx.self.position += dir.normalized * burstSpeed * dt;
            }
        }
        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag) { valence = 0.12f; tag = "lunge"; }
    }
}
