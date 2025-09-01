using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Items
{
    public class Inventory : MonoBehaviour
    {
        public long gold;
        private readonly Dictionary<string, int> _items = new();

        public System.Action<long> OnGoldChanged;
        public System.Action<string, int> OnItemChanged;
        public System.Action<int> OnLootGold; // delta > 0
        public System.Action<string, int> OnLootItem; // (id, delta > 0)

        public void AddGold(long amount)
        {
            int delta = Mathf.RoundToInt(amount);
            gold = Mathf.Max(0, gold + delta);
            OnGoldChanged?.Invoke(gold);
            if (delta > 0) OnLootGold?.Invoke(delta);
        }

        public void AddItem(string id, int count)
        {
            if (string.IsNullOrEmpty(id) || count == 0) return;
            _items.TryGetValue(id, out var cur);
            cur += count; if (cur < 0) cur = 0;
            _items[id] = cur;
            OnItemChanged?.Invoke(id, cur);
            if (count > 0) OnLootItem?.Invoke(id, count);
        }

        public int GetCount(string id)
        {
            return _items.TryGetValue(id, out var c) ? c : 0;
        }

        public IEnumerable<KeyValuePair<string,int>> AllItems()
        {
            return _items;
        }
    }
}
