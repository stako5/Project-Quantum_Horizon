using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.VFX
{
    public class VFXPool : MonoBehaviour
    {
        private static VFXPool _inst;
        [SerializeField] private int prewarm = 16;
        [SerializeField] private Shader shader;
        private Material _mat;
        private readonly Queue<GameObject> _pool = new();

        void Awake()
        {
            if (_inst && _inst != this) { Destroy(gameObject); return; }
            _inst = this; DontDestroyOnLoad(gameObject);
            if (!shader) shader = Shader.Find("Sprites/Default");
            _mat = new Material(shader);
            for (int i=0;i<prewarm;i++) Return(CreateQuad());
        }

        static VFXPool Ensure()
        {
            if (_inst) return _inst;
            var go = new GameObject("VFXPool");
            return go.AddComponent<VFXPool>();
        }

        GameObject CreateQuad()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "VFXQuad";
            var col = go.GetComponent<Collider>(); if (col) Destroy(col);
            var mr = go.GetComponent<MeshRenderer>();
            mr.sharedMaterial = _mat;
            go.SetActive(false);
            go.AddComponent<VFXReturn>();
            return go;
        }

        void Return(GameObject go)
        {
            if (!go) return;
            go.SetActive(false);
            _pool.Enqueue(go);
        }

        GameObject Get()
        {
            if (_pool.Count > 0) { var go = _pool.Dequeue(); go.SetActive(true); return go; }
            return CreateQuad();
        }

        public static void SpawnQuad(Vector3 pos, Color color, float size, float life, bool billboard)
        {
            var inst = Ensure();
            var go = inst.Get();
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * size;
            var mr = go.GetComponent<MeshRenderer>();
            if (mr && mr.sharedMaterial) mr.material = mr.sharedMaterial; // instance for unique color
            if (mr) mr.material.color = color;
            var ret = go.GetComponent<VFXReturn>();
            ret.owner = inst; ret.life = life; ret.billboard = billboard; ret.ResetTimer();
        }

        // Spawns a quad with explicit rotation; returns the instance for advanced positioning
        public static GameObject SpawnQuadRaw(Vector3 pos, Quaternion rotation, Color color, Vector2 size, float life, bool billboard)
        {
            var inst = Ensure();
            var go = inst.Get();
            go.transform.position = pos;
            go.transform.rotation = rotation;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            var mr = go.GetComponent<MeshRenderer>();
            if (mr && mr.sharedMaterial) mr.material = mr.sharedMaterial;
            if (mr) mr.material.color = color;
            var ret = go.GetComponent<VFXReturn>();
            ret.owner = inst; ret.life = life; ret.billboard = billboard; ret.ResetTimer();
            return go;
        }

        // Helper: oriented ground-aligned rectangle (length along dir, width orthogonal), not billboarded
        public static void SpawnRectOnGround(Vector3 center, Vector3 dir, float length, float width, Color color, float life)
        {
            dir.y = 0f; if (dir.sqrMagnitude < 1e-4f) dir = Vector3.forward;
            dir.Normalize();
            float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            var rot = Quaternion.Euler(-90f, yaw, 0f); // lay flat on ground, align Z with dir
            SpawnQuadRaw(center, rot, color, new Vector2(width, length), life, false);
        }

        class VFXReturn : MonoBehaviour
        {
            public VFXPool owner; public float life = 0.2f; public bool billboard = true;
            float _dieAt; MeshRenderer _mr;
            void Awake(){ _mr = GetComponent<MeshRenderer>(); }
            void OnEnable(){ ResetTimer(); }
            public void ResetTimer(){ _dieAt = Time.time + life; }
            void Update()
            {
                if (billboard && Camera.main)
                    transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
                float t = Mathf.InverseLerp(_dieAt, _dieAt - life, Time.time);
                if (_mr) _mr.material.color = new Color(_mr.material.color.r, _mr.material.color.g, _mr.material.color.b, 1f - t);
                if (Time.time >= _dieAt)
                {
                    owner?.Return(gameObject);
                }
            }
        }
    }
}
