using UnityEngine;

namespace MMORPG.Client.Boot
{
    [CreateAssetMenu(menuName = "MMORPG/Boot/Platform Graphics Presets", fileName = "PlatformGraphicsPresets")]
    public class PlatformGraphicsPresets : ScriptableObject
    {
        [System.Serializable]
        public class Preset
        {
            public int targetFrameRate = 120;
            public bool disableVSync = true;
            public AnisotropicFiltering anisotropic = AnisotropicFiltering.Enable;
            [Range(0,8)] public int antiAliasing = 2;
            public ShadowQuality shadows = ShadowQuality.All;
            public ShadowResolution shadowResolution = ShadowResolution.Medium;
        }

        public Preset windows = new Preset();
        public Preset macos = new Preset { targetFrameRate = 90 };
        public Preset linux = new Preset { antiAliasing = 2 };
    }
}

