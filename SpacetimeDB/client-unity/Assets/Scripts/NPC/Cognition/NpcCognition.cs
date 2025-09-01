using UnityEngine;

namespace MMORPG.Client.NPC.Cognition
{
    public class NpcCognition : MonoBehaviour
    {
        [SerializeField] public NpcPersonality personality;
        [SerializeField] public NpcCognitiveMemory memory;

        public float MoodValence { get; private set; } // -1..1
        public float MoodArousal { get; private set; } // 0..1
        public int CurrentHour => System.DateTime.UtcNow.Hour;

        void Awake()
        {
            if (!memory) memory = GetComponent<NpcCognitiveMemory>();
        }

        void Update()
        {
            var (v,a) = memory ? memory.Affect() : (0f,0f);
            // Personality bias: higher neuroticism lowers baseline, higher extraversion raises arousal
            float baseV = Mathf.Lerp(-0.2f, 0.2f, 1f - (personality ? personality.neuroticism : 0.5f));
            float baseA = Mathf.Lerp(0.3f, 0.7f, (personality ? personality.extraversion : 0.5f));
            MoodValence = Mathf.Clamp(baseV + v, -1f, 1f);
            MoodArousal = Mathf.Clamp01(baseA * 0.5f + a * 0.5f);
        }

        public void RecordAction(string action, float valence, params string[] tags)
        {
            memory?.Add("action", action, valence, Mathf.Clamp01(Mathf.Abs(valence)), 0.5f, tags);
        }
    }
}
