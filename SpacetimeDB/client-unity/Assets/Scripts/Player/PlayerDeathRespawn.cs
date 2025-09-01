using System.Collections;
using UnityEngine;

namespace MMORPG.Client.Player
{
    public class PlayerDeathRespawn : MonoBehaviour
    {
        [SerializeField] private Combat.Health health;
        [SerializeField] private Transform spawnPoint; // optional; if null, uses initial position
        [SerializeField] private float respawnDelay = 2.0f;
        [SerializeField] private float iframeAfterRespawn = 1.5f;
        [SerializeField] private CanvasGroup fadeOverlay;
        [SerializeField] private float fadeSpeed = 2.5f;

        private Vector3 _initialPos;
        private Quaternion _initialRot;
        private PlayerController _controller;
        private bool _respawning;

        void Awake()
        {
            if (!health) health = GetComponentInChildren<Combat.Health>();
            _controller = GetComponentInChildren<PlayerController>();
            _initialPos = transform.position; _initialRot = transform.rotation;
            if (!spawnPoint)
            {
                var sp = new GameObject("SpawnPoint"); sp.transform.position = _initialPos; sp.transform.rotation = _initialRot; spawnPoint = sp.transform;
            }
            if (health) health.OnDied += OnDied;
            if (fadeOverlay) fadeOverlay.alpha = 0f;
        }

        void OnDestroy()
        {
            if (health) health.OnDied -= OnDied;
        }

        void OnDied()
        {
            if (_respawning) return;
            StartCoroutine(CoRespawn());
        }

        IEnumerator CoRespawn()
        {
            _respawning = true;
            if (_controller) _controller.enabled = false;
            float t = 0f;
            while (t < respawnDelay)
            {
                t += Time.deltaTime;
                if (fadeOverlay) fadeOverlay.alpha = Mathf.Min(1f, fadeOverlay.alpha + Time.deltaTime * fadeSpeed);
                yield return null;
            }

            // Teleport and revive
            transform.position = spawnPoint.position; transform.rotation = spawnPoint.rotation;
            if (health) { health.ResetHealth(health.maxHealth); health.GrantIFrames(); }

            // Fade out and re-enable
            if (_controller) _controller.enabled = true;
            t = 0f;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                if (fadeOverlay) fadeOverlay.alpha = Mathf.Max(0f, fadeOverlay.alpha - Time.deltaTime * fadeSpeed);
                yield return null;
            }
            if (health && iframeAfterRespawn > 0f) health.GrantIFrames();
            _respawning = false;
        }

        // Allows shrines to change spawn
        public void SetSpawnPoint(Vector3 pos, Quaternion rot)
        {
            if (!spawnPoint)
            {
                var sp = new GameObject("SpawnPoint"); spawnPoint = sp.transform;
            }
            spawnPoint.position = pos; spawnPoint.rotation = rot;
        }
    }
}

