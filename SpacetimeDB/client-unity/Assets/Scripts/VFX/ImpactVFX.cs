using UnityEngine;

namespace MMORPG.Client.VFX
{
    public static class ImpactVFX
    {
        public static void Spawn(Vector3 pos, Color? color = null, float size = 0.35f, float life = 0.2f)
        {
            var col = color ?? new Color(1f, 0.6f, 0.1f, 0.9f);
            VFXPool.SpawnQuad(pos + Vector3.up * 0.2f, col.Value, size, life, true);
        }
    }
}
