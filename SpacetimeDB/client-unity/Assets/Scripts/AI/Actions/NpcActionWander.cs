using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.NPC.Cognition;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/NPC/Wander")]
    public class NpcActionWander : UtilityAction
    {
        public float radius = 6f;
        public float moveSpeed = 1.5f;
        private Vector3 _dest;
        private bool _has;

        public override float Score(UtilityContext ctx)
        {
            var cog = ctx.self ? ctx.self.GetComponent<NpcCognition>() : null;
            float explore = cog && cog.personality ? Mathf.Max(0f, 0.5f + cog.personality.openness + 0.5f * cog.personality.prefersExploration) : 0.5f;
            // Avoid midnight
            float hour = cog ? cog.CurrentHour : 12f;
            float dayBias = (hour >= 8 && hour <= 20) ? 1f : 0.5f;
            return explore * dayBias;
        }

        public override void Execute(UtilityContext ctx, float dt)
        {
            if (!_has)
            {
                var p = ctx.self.position;
                var off = Random.insideUnitSphere; off.y = 0f; off = off.normalized * Random.Range(1f, radius);
                _dest = p + off; _has = true;
                var cog = ctx.self.GetComponent<NpcCognition>(); cog?.RecordAction("wander", 0.1f, "explore");
            }
            ctx.self.position = Vector3.MoveTowards(ctx.self.position, _dest, moveSpeed * dt);
            if ((ctx.self.position - _dest).sqrMagnitude < 0.1f) _has = false;
        }

        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag)
        {
            valence = 0.10f; tag = "explore";
        }
    }
}
