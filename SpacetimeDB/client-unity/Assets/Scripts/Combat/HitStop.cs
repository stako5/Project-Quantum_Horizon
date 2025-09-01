using System.Collections;
using UnityEngine;

namespace MMORPG.Client.Combat
{
    public class HitStop : MonoBehaviour
    {
        private static HitStop _inst;
        private bool _running;
        private float _restoreScale = 1f;

        void Awake()
        {
            if (_inst && _inst != this) { Destroy(gameObject); return; }
            _inst = this; DontDestroyOnLoad(gameObject);
        }

        public static void Impact(float duration = 0.05f, float slowScale = 0.05f)
        {
            if (!_inst)
            {
                var go = new GameObject("HitStop");
                _inst = go.AddComponent<HitStop>();
            }
            _inst.StopAllCoroutines();
            _inst.StartCoroutine(_inst.CoHitStop(duration, slowScale));
        }

        IEnumerator CoHitStop(float duration, float slowScale)
        {
            if (_running)
            {
                // Already running: override scale and extend duration
                Time.timeScale = Mathf.Min(Time.timeScale, slowScale);
                yield return new WaitForSecondsRealtime(duration);
                Time.timeScale = _restoreScale;
                _running = false; yield break;
            }
            _running = true;
            _restoreScale = Time.timeScale;
            Time.timeScale = Mathf.Clamp(slowScale, 0.01f, 1f);
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = _restoreScale;
            _running = false;
        }
    }
}

