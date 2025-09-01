using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class EnemyStatsDebugUI : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private float refreshInterval = 0.5f;
        [SerializeField] private KeyCode toggleKey = KeyCode.F3;
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private float scanRadius = 40f;

        private bool _visible = true;
        private float _t;

        void Awake()
        {
            if (!text)
            {
                var go = new GameObject("EnemyStatsText");
                go.transform.SetParent(transform, false);
                text = go.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.color = new Color(1f, 1f, 1f, 0.85f);
                text.alignment = TextAnchor.UpperLeft;
                text.raycastTarget = false;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey)) _visible = !_visible;
            if (text) text.enabled = _visible;
            if (!_visible) return;
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
            if (!text) return;
            var player = GameObject.FindGameObjectWithTag(playerTag);
            Vector3 origin = player ? player.transform.position : Vector3.zero;
            var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            var target = enemies
                .Where(go => go)
                .OrderBy(go => (go.transform.position - origin).sqrMagnitude)
                .FirstOrDefault(go => (go.transform.position - origin).sqrMagnitude <= scanRadius * scanRadius) ?? enemies.FirstOrDefault();
            if (!target) { text.text = "No enemies in scene."; return; }

            var id = target.GetComponent<MMORPG.Client.Enemies.EnemyIdentity>();
            var hp = target.GetComponent<MMORPG.Client.Combat.Health>();
            var mover = target.GetComponent<MMORPG.Client.Enemies.EnemyMover>();
            var melee = target.GetComponent<MMORPG.Client.Enemies.EnemyMeleeAttack>();
            var pers = target.GetComponent<MMORPG.Client.Enemies.EnemyPersonality>();

            var sb = new StringBuilder();
            sb.AppendLine($"Enemy: {(id?.DisplayName ?? target.name)}  Family: {id?.Family}  Tier: {id?.Tier}");
            sb.AppendLine($"Role: {id?.Role}  Stage: {id?.Stage}  Size: {id?.Size}");
            if (hp) sb.AppendLine($"HP: {hp.currentHealth:F0}/{hp.maxHealth:F0}");
            if (melee) sb.AppendLine($"Damage: {melee.damage:F1}");
            if (mover) sb.AppendLine($"Speed: {mover.baseSpeed:F2}");
            if (id) sb.AppendLine($"Base HP/DMG/SPD: {id.BaseHP}/{id.BaseDamage}/{id.BaseSpeed:F2}");
            if (pers) sb.AppendLine($"O/C/E: {pers.openness:F2}/{pers.conscientiousness:F2}/{pers.extraversion:F2}");
            text.text = sb.ToString();
        }
    }
}

