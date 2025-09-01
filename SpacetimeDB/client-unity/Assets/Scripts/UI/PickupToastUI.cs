using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class PickupToastUI : MonoBehaviour
    {
        [SerializeField] private Items.Inventory inventory;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Text text;
        [SerializeField] private float showTime = 1.2f;
        [SerializeField] private float fadeSpeed = 6f;

        private readonly Queue<string> _queue = new();
        private float _timer;

        void Awake()
        {
            if (!inventory) inventory = FindObjectOfType<Items.Inventory>();
            if (!group) group = gameObject.AddComponent<CanvasGroup>();
            if (!text)
            {
                var go = new GameObject("ToastText");
                go.transform.SetParent(transform, false);
                text = go.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.alignment = TextAnchor.LowerCenter; text.color = Color.yellow;
                text.raycastTarget = false;
            }
            if (inventory)
            {
                inventory.OnLootGold += amt => Enqueue($"+{amt} Gold");
                inventory.OnLootItem += (id, amt) => Enqueue($"+{amt} {id}");
            }
            group.alpha = 0f; text.text = string.Empty;
        }

        void Enqueue(string msg)
        {
            _queue.Enqueue(msg);
            if (_timer <= 0f) ShowNext();
        }

        void ShowNext()
        {
            if (_queue.Count == 0) return;
            var msg = _queue.Dequeue();
            text.text = msg; _timer = showTime; group.alpha = 1f;
        }

        void Update()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f) { text.text = string.Empty; }
            }
            float target = _timer > 0f ? 1f : 0f;
            group.alpha = Mathf.MoveTowards(group.alpha, target, Time.deltaTime * fadeSpeed);
            if (_timer <= 0f && _queue.Count > 0 && Mathf.Approximately(group.alpha, 0f))
            {
                ShowNext();
            }
        }
    }
}

