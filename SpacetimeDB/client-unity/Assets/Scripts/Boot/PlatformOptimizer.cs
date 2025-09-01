using UnityEngine;

namespace MMORPG.Client.Boot
{
    // Applies lightweight platform optimizations at startup.
    public class PlatformOptimizer : MonoBehaviour
    {
        [Header("General")]
        public int targetFrameRate = 120; // desktop default
        public bool disableVSync = true;
        public AnisotropicFiltering anisotropic = AnisotropicFiltering.Enable;
        [Range(0,8)] public int antiAliasing = 2; // MSAA
        [Header("Shadows")]
        public ShadowQuality shadowQuality = ShadowQuality.All;
        public ShadowResolution shadowResolution = ShadowResolution.Medium;

        [Header("Presets (optional)")]
        public PlatformGraphicsPresets presets;

        void Awake()
        {
            ApplyCommon();
            ApplyPlatformSpecific();
        }

        void ApplyCommon()
        {
            if (presets)
            {
#if UNITY_STANDALONE_WIN
                ApplyPreset(presets.windows);
#elif UNITY_STANDALONE_OSX
                ApplyPreset(presets.macos);
#elif UNITY_STANDALONE_LINUX
                ApplyPreset(presets.linux);
#else
                ApplyPreset(presets.windows);
#endif
            }
            else
            {
                if (disableVSync) QualitySettings.vSyncCount = 0; // let targetFrameRate govern
                if (targetFrameRate > 0) Application.targetFrameRate = targetFrameRate;
                QualitySettings.anisotropicFiltering = anisotropic;
                QualitySettings.antiAliasing = antiAliasing;
                QualitySettings.shadows = shadowQuality;
                QualitySettings.shadowResolution = shadowResolution;
            }
        }

        void ApplyPlatformSpecific()
        {
#if UNITY_STANDALONE_WIN
            // Windows: keep defaults; can consider DX11/12 via player settings (outside runtime)
#elif UNITY_STANDALONE_OSX
            // macOS: limit target framerate slightly lower to save thermals
            Application.targetFrameRate = Mathf.Min(Application.targetFrameRate, 90);
#elif UNITY_STANDALONE_LINUX
            // Linux: some drivers perform better with lower MSAA
            QualitySettings.antiAliasing = Mathf.Min(QualitySettings.antiAliasing, 2);
#endif
        }

        void ApplyPreset(PlatformGraphicsPresets.Preset p)
        {
            if (p == null) return;
            if (p.disableVSync) QualitySettings.vSyncCount = 0; else QualitySettings.vSyncCount = 1;
            if (p.targetFrameRate > 0) Application.targetFrameRate = p.targetFrameRate;
            QualitySettings.anisotropicFiltering = p.anisotropic;
            QualitySettings.antiAliasing = p.antiAliasing;
            QualitySettings.shadows = p.shadows;
            QualitySettings.shadowResolution = p.shadowResolution;
        }
    }
}
