using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.Dungeon
{
    public class BossSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject bossPrefab; // assign your boss model/AI
        [SerializeField] private float bossHP = 5000f;
        [SerializeField] private BossHealth health;
        public string BossId;

        void Start()
        {
            if (!health)
            {
                var go = bossPrefab ? Instantiate(bossPrefab, transform) : new GameObject("Boss");
                health = go.AddComponent<BossHealth>();
                health.maxHp = bossHP; health.currentHp = bossHP;
                health.onDeath += OnBossDeath;
            }
        }

        private async void OnBossDeath()
        {
            // Award loot via reducer
            await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
            await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("award_boss_loot", BossId ?? "");
            // Quest trigger: kill_boss
            await MMORPG.Client.Quests.QuestEventReporter.ReportAsync("kill_boss", string.Empty, 1);
        }
    }

    public class BossHealth : MonoBehaviour
    {
        public float maxHp = 1000f;
        public float currentHp = 1000f;
        public System.Action onDeath;
        public void Damage(float amount)
        {
            currentHp = Mathf.Max(0f, currentHp - Mathf.Max(0f, amount));
            if (currentHp <= 0f) onDeath?.Invoke();
        }
    }
}

