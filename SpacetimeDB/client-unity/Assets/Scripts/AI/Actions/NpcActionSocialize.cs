using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.NPC.Cognition;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/NPC/Socialize")]
    public class NpcActionSocialize : UtilityAction
    {
        public float moveSpeed = 1.8f;
        public Vector3 localSocialOffset = new Vector3(-3,0,5);

        public override float Score(UtilityContext ctx)
        {
            var cog = ctx.self ? ctx.self.GetComponent<NpcCognition>() : null;
            if (!cog || !cog.personality) return 0.3f * weight;
            float hour = cog.CurrentHour;
            float scheduleBias = (hour >= 18 && hour <= 22) ? 1f : 0.4f;
            float extraversion = cog.personality.extraversion;
            float prefers = Mathf.Max(0f, 0.5f + 0.5f * cog.personality.prefersSocial);
            // Positive mood increases urge to socialize
            float mood = Mathf.Max(0f, cog.MoodValence);
            return scheduleBias * Mathf.Lerp(0.2f, 1.0f, (extraversion + prefers + mood)/3f) * weight;
        }

        public override void Execute(UtilityContext ctx, float dt)
        {
            var target = ctx.self.position + localSocialOffset;
            ctx.self.position = Vector3.MoveTowards(ctx.self.position, target, moveSpeed * dt);
            if ((ctx.self.position - target).sqrMagnitude < 0.25f)
            {
                var cog = ctx.self.GetComponent<NpcCognition>();
                cog?.RecordAction("socialize", 0.3f, "social");
            }
        }

        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag)
        {
            valence = 0.30f; tag = "social";
        }
    }
}
