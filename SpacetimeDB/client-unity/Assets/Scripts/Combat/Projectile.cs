using UnityEngine;

namespace MMORPG.Client.Combat
{
    [RequireComponent(typeof(SphereCollider))]
    public class Projectile : MonoBehaviour
    {
        public float speed = 12f;
        public float damage = 12f;
        public float lifeSeconds = 4f;
        public Transform owner;
        public bool usePooling = false;

        private float _dieAt;

        void Awake()
        {
            var col = GetComponent<SphereCollider>();
            col.isTrigger = true; col.radius = 0.2f;
            _dieAt = Time.time + lifeSeconds;
        }

        void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            if (Time.time >= _dieAt) Despawn();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other) return;
            if (owner && other.transform.IsChildOf(owner)) return;
            var hp = other.GetComponentInParent<Health>();
            if (hp)
            {
                if (hp.TryDamage(damage))
                {
                    // Threat to target table if present
                    var tt = hp.GetComponentInParent<MMORPG.Client.Enemies.ThreatTable>();
                    if (tt && owner) tt.AddThreat(owner, damage);
                    // Impact VFX/SFX
                    MMORPG.Client.VFX.ImpactVFX.Spawn(transform.position);
                    MMORPG.Client.Audio.SfxPlayer.PlayImpact(transform.position);
                    MMORPG.Client.Combat.HitStop.Impact(0.03f, 0.15f);
                }
                Despawn();
            }
        }

        void Despawn()
        {
            if (usePooling) ProjectilePool.Return(this);
            else Destroy(gameObject);
        }
    }
}
