using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Combat
{
    public class ProjectilePool : MonoBehaviour
    {
        private static ProjectilePool _instance;
        private readonly Queue<Projectile> _pool = new();

        [SerializeField] private int prewarmCount = 0;

        void Awake()
        {
            if (_instance && _instance != this) { Destroy(gameObject); return; }
            _instance = this; DontDestroyOnLoad(gameObject);
            for (int i=0;i<prewarmCount;i++)
            {
                var p = Create(); Return(p);
            }
        }

        static Projectile Create()
        {
            var go = new GameObject("PooledProjectile");
            var p = go.AddComponent<Projectile>();
            // Add a small visual tip (optional)
            var tip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tip.transform.SetParent(go.transform, false);
            tip.transform.localScale = Vector3.one * 0.15f;
            var col = tip.GetComponent<Collider>(); if (col) Destroy(col);
            p.enabled = false; go.SetActive(false);
            return p;
        }

        public static Projectile Get()
        {
            var inst = _instance ?? new GameObject("ProjectilePool").AddComponent<ProjectilePool>();
            if (inst._pool.Count > 0)
            {
                var p = inst._pool.Dequeue();
                p.gameObject.SetActive(true); p.enabled = true; return p;
            }
            return Create();
        }

        public static void Return(Projectile p)
        {
            if (!p) return;
            p.enabled = false; p.gameObject.SetActive(false);
            // Reset simple fields
            p.transform.rotation = Quaternion.identity;
            _instance._pool.Enqueue(p);
        }
    }
}

