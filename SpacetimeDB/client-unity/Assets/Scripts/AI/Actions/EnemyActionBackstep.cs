using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.Enemies;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/Enemy/Backstep")]
    public class EnemyActionBackstep : UtilityAction
    {
        public float tooClose = 1.2f;
        public float stepSpeed = 3f;
        public EnemyActionBackstep(){ cooldownSeconds = 0.8f; minActiveSeconds = 0.15f; windupSeconds = 0.05f; activeSeconds = 0.2f; recoverySeconds = 0.15f; rootDuringWindup = true; rootDuringRecovery = false; }
        public override float Score(UtilityContext ctx)
        {
            if (!ctx.target) return 0f;
            float d = ctx.distToTarget;
            var ep = ctx.self.GetComponent<EnemyPersonality>();
            float c = ep ? ep.conscientiousness : 0.5f;
            float baseScore = d < tooClose ? 1f : 0f;
            float noise = Random.Range(0f, 0.15f) * (ep ? ep.openness : 0.5f);
            return (baseScore * (0.6f + c*0.6f) + noise) * weight;
        }
        public override void Execute(UtilityContext ctx, float dt)
        {
            if (!ctx.target) return;
            var dir = (ctx.self.position - ctx.target.position); dir.y = 0f;
            var mover = ctx.self.GetComponent<MMORPG.Client.Enemies.EnemyMover>();
            if (mover) mover.MoveInDirection(dir.normalized);
            else ctx.self.position += dir.normalized * stepSpeed * dt;
        }
        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag) { valence = 0.08f; tag = "backstep"; }
    }
}
