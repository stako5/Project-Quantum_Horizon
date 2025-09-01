using UnityEngine;

namespace MMORPG.Client.Audio
{
    public class SfxPlayer : MonoBehaviour
    {
        private static SfxPlayer _inst;
        [SerializeField] private AudioClip defaultImpact;
        [SerializeField] private float defaultVolume = 0.5f;

        void Awake()
        {
            if (_inst && _inst != this) { Destroy(gameObject); return; }
            _inst = this; DontDestroyOnLoad(gameObject);
        }

        public static void PlayImpact(Vector3 pos, AudioClip clip = null, float? volume = null)
        {
            if (!_inst)
            {
                var go = new GameObject("SfxPlayer");
                _inst = go.AddComponent<SfxPlayer>();
            }
            var useClip = clip ? clip : _inst.defaultImpact;
            if (!useClip) return; // no clip assigned, silently skip
            AudioSource.PlayClipAtPoint(useClip, pos, Mathf.Clamp01(volume ?? _inst.defaultVolume));
        }
    }
}

