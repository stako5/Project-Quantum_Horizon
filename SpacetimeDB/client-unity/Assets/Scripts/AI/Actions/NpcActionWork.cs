using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.NPC.Cognition;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/NPC/Work")]
    public class NpcActionWork : UtilityAction
    {
        public float moveSpeed = 2f;
        public Vector3 localWorkOffset = new Vector3(4,0,4);

        public override float Score(UtilityContext ctx)
        {
            var cog = ctx.self ? ctx.self.GetComponent<NpcCognition>() : null;
            if (!cog || !cog.personality) return 0.3f * weight;
            float hour = cog.CurrentHour;
            float scheduleBias = (hour >= 9 && hour <= 16) ? 1f : 0.3f;
            float conscientious = cog.personality.conscientiousness;
            return scheduleBias * Mathf.Lerp(0.2f, 1.0f, conscientious) * weight;
        }

        public override void Execute(UtilityContext ctx, float dt)
        {
            var target = ctx.self.position + localWorkOffset;
            ctx.self.position = Vector3.MoveTowards(ctx.self.position, target, moveSpeed * dt);
            if ((ctx.self.position - target).sqrMagnitude < 0.25f)
            {
                var cog = ctx.self.GetComponent<NpcCognition>();
                cog?.RecordAction("work", 0.2f, "work");
            }
        }

        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag)
        {
            valence = 0.20f; tag = "work";
        }
    }
}
