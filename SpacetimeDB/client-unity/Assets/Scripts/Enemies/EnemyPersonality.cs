using UnityEngine;

namespace MMORPG.Client.Enemies
{
    public class EnemyPersonality : MonoBehaviour
    {
        [Range(0f,1f)] public float openness = 0.5f;
        [Range(0f,1f)] public float conscientiousness = 0.5f;
        [Range(0f,1f)] public float extraversion = 0.5f;

        public void ApplyTag(string tag)
        {
            if (string.IsNullOrEmpty(tag) || tag.Length < 3) return;
            if (tag[0] == 'O' && tag.StartsWith("O:")) float.TryParse(tag.Substring(2), out openness);
            else if (tag[0] == 'C' && tag.StartsWith("C:")) float.TryParse(tag.Substring(2), out conscientiousness);
            else if (tag[0] == 'E' && tag.StartsWith("E:")) float.TryParse(tag.Substring(2), out extraversion);
            openness = Mathf.Clamp01(openness); conscientiousness = Mathf.Clamp01(conscientiousness); extraversion = Mathf.Clamp01(extraversion);
        }
    }
}
