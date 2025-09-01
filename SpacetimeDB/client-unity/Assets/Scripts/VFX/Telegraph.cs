using UnityEngine;

namespace MMORPG.Client.VFX
{
    public static class Telegraph
    {
        public static void FlashLine(Vector3 from, Vector3 to, Color color, float width = 0.05f, float duration = 0.2f)
        {
            var go = new GameObject("TelegraphLine");
            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.startWidth = width; lr.endWidth = width;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = color; lr.endColor = color;
            lr.SetPosition(0, from + Vector3.up * 1.5f);
            lr.SetPosition(1, to + Vector3.up * 1.5f);
            go.AddComponent<AutoDestroy>().life = duration;
        }

        public class AutoDestroy : MonoBehaviour
        {
            public float life = 0.25f;
            void Start(){ Destroy(gameObject, life); }
        }
    }
}

