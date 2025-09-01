using UnityEngine;
using MMORPG.Client.AI;
using MMORPG.Client.Enemies;

namespace MMORPG.Client.AI.Actions
{
    [CreateAssetMenu(menuName = "MMORPG/AI/Enemy/Roar")]
    public class EnemyActionRoar : UtilityAction
    {
        public float cooldownBias = 0.5f;
        public EnemyActionRoar(){ cooldownSeconds = 3f; minActiveSeconds = 0.1f; }
        public override float Score(UtilityContext ctx)
        {
            var ep = ctx.self.GetComponent<EnemyPersonality>();
            float o = ep ? ep.openness : 0.5f;
            float baseScore = 0.3f + o * 0.4f; // more open creatures try varied moves like roar
            float noise = Random.Range(0f, 0.4f) * o;
            return (baseScore + noise) * weight * cooldownBias;
        }
        public override void Execute(UtilityContext ctx, float dt)
        {
            // Roar is a flavor action; we could apply a local debuff VFX here
        }
        public override void GetMemorySignature(UtilityContext ctx, out float valence, out string tag) { valence = 0.05f; tag = "roar"; }
    }
}
