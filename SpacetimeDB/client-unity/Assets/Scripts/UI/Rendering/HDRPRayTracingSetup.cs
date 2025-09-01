// This file is guarded by HDRP_PRESENT to avoid compile errors without HDRP.
#if HDRP_PRESENT
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace MMORPG.Client.Rendering
{
    [RequireComponent(typeof(Volume))]
    public class HDRPRayTracingSetup : MonoBehaviour
    {
        [Header("Overrides")]
        public bool reflections = true;
        public bool ambientOcclusion = true;
        public bool shadows = true;

        private Volume _volume;

        void Awake()
        {
            _volume = GetComponent<Volume>();
            if (_volume.profile == null)
                _volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();

            if (reflections)
            {
                if (!_volume.profile.TryGet(out ScreenSpaceReflection ssr))
                    ssr = _volume.profile.Add<ScreenSpaceReflection>(true);
                ssr.tracing.value = ScreenSpaceReflection.TracingParam.RayTracing;
                ssr.rayTracing.value = true;
            }
            if (ambientOcclusion)
            {
                if (!_volume.profile.TryGet(out AmbientOcclusion ao))
                    ao = _volume.profile.Add<AmbientOcclusion>(true);
                ao.quality.value = AmbientOcclusionQuality.RayTraced;
                ao.rayTracing.value = true;
            }
            if (shadows)
            {
                if (!_volume.profile.TryGet(out RayTracingSettings rts))
                    rts = _volume.profile.Add<RayTracingSettings>(true);
                rts.raytracedShadows.value = true;
            }
        }
    }
}
#endif

