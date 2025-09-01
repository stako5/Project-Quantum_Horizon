using UnityEngine;

namespace MMORPG.Client.VFX
{
    public static class GroundTelegraph
    {
        public static void Cone(Vector3 origin, Vector3 forward, float angleDeg, float range, Color color, float life, int radialLines = 7, float lineWidth = 0.25f)
        {
            forward.y = 0f; if (forward.sqrMagnitude < 1e-4f) forward = Vector3.forward; forward.Normalize();
            float start = -angleDeg * 0.5f; float step = radialLines > 1 ? (angleDeg / (radialLines - 1)) : angleDeg;
            for (int i = 0; i < radialLines; i++)
            {
                float a = start + step * i;
                var dir = Quaternion.Euler(0f, a, 0f) * forward;
                var center = origin + dir * (range * 0.5f);
                VFXPool.SpawnRectOnGround(center, dir, range, lineWidth, color, life);
            }
            // Arc edge: place small segments along the arc for better readability
            int arcSegs = Mathf.Max(6, Mathf.RoundToInt(angleDeg / 10f));
            float arcStep = angleDeg / arcSegs;
            float arcWidth = lineWidth * 0.6f;
            for (int i = 0; i < arcSegs; i++)
            {
                float a = start + arcStep * (i + 0.5f);
                var dir = Quaternion.Euler(0f, a, 0f) * forward;
                var center = origin + dir * range;
                VFXPool.SpawnRectOnGround(center, dir, arcStep * Mathf.Deg2Rad * range, arcWidth, color, life);
            }
        }
    }
}

