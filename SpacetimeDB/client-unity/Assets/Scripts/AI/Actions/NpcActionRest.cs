using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.NPC.Cognition;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/NPC/Rest")]
    public class NpcActionRest : UtilityAction
    {
        public float moveSpeed = 1.2f;
        public Vector3 localRestOffset = new Vector3(2,0,-2);

        public override float Score(UtilityContext ctx)
        {
            var cog = ctx.self ? ctx.self.GetComponent<NpcCognition>() : null;
            float need = 1f - (ctx.stamina01);
            float neuro = cog && cog.personality ? cog.personality.neuroticism : 0.5f;
            return Mathf.Lerp(0.1f, 1.0f, (need + neuro)/2f) * weight;
        }

        public override void Execute(UtilityContext ctx, float dt)
        {
            var target = ctx.self.position + localRestOffset;
            ctx.self.position = Vector3.MoveTowards(ctx.self.position, target, moveSpeed * dt);
            if ((ctx.self.position - target).sqrMagnitude < 0.25f)
            {
                var cog = ctx.self.GetComponent<NpcCognition>();
                cog?.RecordAction("rest", 0.15f, "rest");
            }
        }

        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag)
        {
            valence = 0.15f; tag = "rest";
        }
    }
}
