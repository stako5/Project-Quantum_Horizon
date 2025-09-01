using UnityEngine;

namespace MMORPG.Client.World
{
    [RequireComponent(typeof(Collider))]
    public class SpawnShrine : MonoBehaviour
    {
        [SerializeField] private bool autoAttuneOnEnter = true;
        [SerializeField] private string playerTag = "Player";

        void Awake()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!autoAttuneOnEnter) return;
            TryAttune(other);
        }

        public void TryAttune(Component other)
        {
            if (!other) return;
            var root = other.GetComponentInParent<Transform>();
            if (!root) return;
            var respawn = root.GetComponentInParent<MMORPG.Client.Player.PlayerDeathRespawn>();
            if (!respawn)
            {
                var go = root.gameObject;
                if (!go.CompareTag(playerTag)) return;
            }
            else
            {
                respawn.SetSpawnPoint(transform.position, transform.rotation);
            }
            // Simple VFX/SFX feedback
            MMORPG.Client.VFX.ImpactVFX.Spawn(transform.position + Vector3.up * 1f, new Color(0.2f, 1f, 0.7f, 0.9f), 0.6f, 0.4f);
            MMORPG.Client.Audio.SfxPlayer.PlayImpact(transform.position);
        }
    }
}

