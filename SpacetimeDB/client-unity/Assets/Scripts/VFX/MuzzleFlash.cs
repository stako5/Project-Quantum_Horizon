using UnityEngine;

namespace MMORPG.Client.VFX
{
    public static class MuzzleFlash
    {
        public static void Spawn(Transform origin, Color? color = null, float size = 0.22f, float life = 0.06f, float forwardOffset = 0.35f, float upOffset = 1.4f)
        {
            if (!origin) return;
            var pos = origin.position + origin.forward * forwardOffset + Vector3.up * upOffset;
            var col = color ?? new Color(1f, 0.8f, 0.2f, 0.9f);
            VFXPool.SpawnQuad(pos, col.Value, size, life, true);
        }
    }
}

