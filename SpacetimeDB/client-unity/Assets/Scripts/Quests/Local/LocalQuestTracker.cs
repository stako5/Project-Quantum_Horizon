using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Quests.Local
{
    public class LocalQuestTracker : MonoBehaviour
    {
        [SerializeField] private LocalQuestList quests;
        [SerializeField] private Items.Inventory inventory;
        [SerializeField] private Player.PlayerProgress progress;

        private readonly HashSet<string> _claimed = new();
        private readonly Dictionary<string, int> _kills = new();
        private readonly Dictionary<string, int> _collects = new();

        void Awake()
        {
            if (!inventory) inventory = FindObjectOfType<Items.Inventory>();
            if (!progress) progress = FindObjectOfType<Player.PlayerProgress>();
            if (!quests)
            {
                quests = Resources.Load<LocalQuestList>("Quests/LocalQuests");
            }
            MMORPG.Client.Enemies.EnemyDeathWatcher.OnEnemyKilledFamily += OnKilled;
            if (inventory) inventory.OnLootItem += OnLootItem;
        }

        void OnDestroy()
        {
            MMORPG.Client.Enemies.EnemyDeathWatcher.OnEnemyKilledFamily -= OnKilled;
            if (inventory) inventory.OnLootItem -= OnLootItem;
        }

        void OnKilled(string family)
        {
            if (string.IsNullOrEmpty(family)) return;
            _kills.TryGetValue(family, out var c); _kills[family] = c + 1;
        }

        void OnLootItem(string id, int delta)
        {
            if (string.IsNullOrEmpty(id) || delta <= 0) return;
            _collects.TryGetValue(id, out var c); _collects[id] = c + delta;
        }

        public IEnumerable<QuestDefinition> AllQuests()
        {
            if (!quests || quests.quests == null) yield break;
            foreach (var q in quests.quests) if (q) yield return q;
        }

        public (int cur, int req) GetKillProgress(QuestDefinition q)
        {
            int cur = 0; _kills.TryGetValue(q.killFamily ?? string.Empty, out cur);
            return (cur, Mathf.Max(0, q.killCount));
        }

        public IEnumerable<(string id, int cur, int req)> GetCollectProgress(QuestDefinition q)
        {
            if (q.collect == null) yield break;
            foreach (var r in q.collect)
            {
                int cur = 0; _collects.TryGetValue(r.itemId ?? string.Empty, out cur);
                int req = Mathf.Max(0, r.count);
                yield return (r.itemId, cur, req);
            }
        }

        public bool IsCompleted(QuestDefinition q)
        {
            if (!q) return false;
            if (q.killCount > 0)
            {
                var kp = GetKillProgress(q); if (kp.cur < kp.req) return false;
            }
            if (q.collect != null)
            {
                foreach (var tup in GetCollectProgress(q)) if (tup.cur < tup.req) return false;
            }
            return true;
        }

        public bool IsClaimed(QuestDefinition q) => q && _claimed.Contains(q.questId ?? q.name);

        public bool TryClaim(QuestDefinition q)
        {
            if (!q || IsClaimed(q) || !IsCompleted(q)) return false;
            _claimed.Add(q.questId ?? q.name);
            if (inventory && q.rewardGold > 0) inventory.AddGold(q.rewardGold);
            if (progress && q.rewardXP > 0) progress.AddXP(q.rewardXP);
            return true;
        }
    }
}
