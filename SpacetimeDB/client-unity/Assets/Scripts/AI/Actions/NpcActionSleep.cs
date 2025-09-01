using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.NPC.Cognition;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/NPC/Sleep")]
    public class NpcActionSleep : UtilityAction
    {
        public float moveSpeed = 1.0f;
        public Vector3 localSleepOffset = new Vector3(-4,0,-4);

        public override float Score(UtilityContext ctx)
        {
            var cog = ctx.self ? ctx.self.GetComponent<NpcCognition>() : null;
            float hour = cog ? cog.CurrentHour : 0f;
            float nightBias = (hour >= 22 || hour <= 6) ? 1f : 0.2f;
            return nightBias * weight;
        }

        public override void Execute(UtilityContext ctx, float dt)
        {
            var target = ctx.self.position + localSleepOffset;
            ctx.self.position = Vector3.MoveTowards(ctx.self.position, target, moveSpeed * dt);
            if ((ctx.self.position - target).sqrMagnitude < 0.25f)
            {
                var cog = ctx.self.GetComponent<NpcCognition>();
                cog?.RecordAction("sleep", 0.4f, "sleep");
            }
        }

        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag)
        {
            valence = 0.40f; tag = "sleep";
        }
    }
}
