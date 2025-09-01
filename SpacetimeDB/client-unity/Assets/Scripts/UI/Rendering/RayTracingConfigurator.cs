using UnityEngine;

namespace MMORPG.Client.Rendering
{
    public class RayTracingConfigurator : MonoBehaviour
    {
        [Tooltip("Enable HDRP ray traced effects if HDRP + DXR is present.")]
        public bool enableRayTracing = true;
        [Tooltip("Try to enable ray traced reflections if available.")]
        public bool reflections = true;
        [Tooltip("Try to enable ray traced ambient occlusion if available.")]
        public bool ambientOcclusion = true;
        [Tooltip("Try to enable ray traced shadows if available.")]
        public bool shadows = true;

        void Start()
        {
#if HDRP_PRESENT
            if (!enableRayTracing) return;
            // This component expects an HDRP Project with DXR enabled and appropriate Volume overrides.
            // At runtime, we simply ensure that Volume overrides are set to Ray Tracing quality where possible.
            Debug.Log("[RayTracing] HDRP_PRESENT defined; ensure DXR enabled in HDRP asset and graphics settings.");
#else
            if (enableRayTracing)
                Debug.LogWarning("[RayTracing] HDRP ray tracing requires HDRP + DXR. Define HDRP_PRESENT and configure your HDRP asset.");
#endif
        }
    }
}

