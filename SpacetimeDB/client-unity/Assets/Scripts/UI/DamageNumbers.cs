using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.UI
{
    public class DamageNumbers : MonoBehaviour
    {
        private static DamageNumbers _instance;
        private readonly Queue<TextMesh> _pool = new();

        [SerializeField] private int prewarm = 12;
        [SerializeField] private float defaultLife = 1.2f;
        [SerializeField] private float defaultRise = 1.5f;
        [SerializeField] private Font font;

        void Awake()
        {
            if (_instance && _instance != this) { Destroy(gameObject); return; }
            _instance = this; DontDestroyOnLoad(gameObject);
            for (int i=0;i<prewarm;i++) Return(Create());
        }

        static DamageNumbers Ensure()
        {
            if (_instance) return _instance;
            var go = new GameObject("DamageNumbersPool");
            return go.AddComponent<DamageNumbers>();
        }

        TextMesh Create()
        {
            var go = new GameObject("DmgText");
            go.transform.localScale = Vector3.one;
            var tm = go.AddComponent<TextMesh>();
            tm.anchor = TextAnchor.MiddleCenter; tm.alignment = TextAlignment.Center;
            tm.characterSize = 0.1f; tm.fontSize = 48;
            if (font) tm.font = font;
            var beh = go.AddComponent<FadeAndReturn>();
            beh.owner = this; beh.life = defaultLife; beh.riseSpeed = defaultRise;
            go.SetActive(false);
            return tm;
        }

        void Return(TextMesh tm)
        {
            if (!tm) return;
            tm.gameObject.SetActive(false);
            _pool.Enqueue(tm);
        }

        TextMesh Get()
        {
            if (_pool.Count > 0) { var tm = _pool.Dequeue(); tm.gameObject.SetActive(true); return tm; }
            return Create();
        }

        public static void Spawn(Vector3 pos, float amount, Color? color = null)
        {
            var inst = Ensure();
            var tm = inst.Get();
            tm.text = Mathf.RoundToInt(amount).ToString();
            var c = color ?? Color.yellow; tm.color = c;
            tm.transform.position = pos + Vector3.up * 1.8f;
            var beh = tm.GetComponent<FadeAndReturn>();
            beh.owner = inst; beh.life = inst.defaultLife; beh.riseSpeed = inst.defaultRise; beh.ResetTimer();
        }

        class FadeAndReturn : MonoBehaviour
        {
            public DamageNumbers owner;
            public float life = 1.2f;
            public float riseSpeed = 1.5f;
            float _dieAt;
            TextMesh _tm;
            void Awake(){ _tm = GetComponent<TextMesh>(); }
            void OnEnable(){ ResetTimer(); }
            public void ResetTimer(){ _dieAt = Time.time + life; }
            void Update()
            {
                transform.position += Vector3.up * riseSpeed * Time.deltaTime;
                float t = Mathf.InverseLerp(_dieAt, _dieAt - life, Time.time);
                if (_tm) _tm.color = new Color(_tm.color.r, _tm.color.g, _tm.color.b, 1f - t);
                if (Time.time >= _dieAt)
                {
                    owner?.Return(_tm);
                }
            }
        }
    }
}
