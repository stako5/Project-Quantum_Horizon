using UnityEngine;

namespace MMORPG.Client.NPC.Cognition
{
    [CreateAssetMenu(menuName = "MMORPG/NPC/Personality", fileName = "NpcPersonality")]
    public class NpcPersonality : ScriptableObject
    {
        [Header("Traits (0..1)")]
        [Range(0f,1f)] public float openness = 0.5f;
        [Range(0f,1f)] public float conscientiousness = 0.5f;
        [Range(0f,1f)] public float extraversion = 0.5f;
        [Range(0f,1f)] public float agreeableness = 0.5f;
        [Range(0f,1f)] public float neuroticism = 0.5f;

        [Header("Preferences (-1..1)")]
        [Range(-1f,1f)] public float prefersWork = 0.0f;
        [Range(-1f,1f)] public float prefersSocial = 0.0f;
        [Range(-1f,1f)] public float prefersExploration = 0.0f;
    }
}
