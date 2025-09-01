using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Net;

namespace MMORPG.Client.UI
{
    public class ActiveEventsUI : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private Text itemTemplate;
        [SerializeField] private float refreshInterval = 1.0f;

        private float _timer;

        void Update()
        {
            _timer -= Time.deltaTime; if (_timer > 0f) return; _timer = refreshInterval; Rebuild();
        }

        void Rebuild()
        {
            foreach (Transform child in content) Destroy(child.gameObject);
#if SPACETIMEDB_SDK
            var events = BindingsBridge.GetActiveBiomeEvents()
                .OrderBy(e => e.ExpiresAt)
                .ToList();
            foreach (var e in events)
            {
                var row = new GameObject("Event"); row.transform.SetParent(content, false);
                var rt = row.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 28f);
                var bg = row.AddComponent<Image>(); bg.color = new Color(0,0,0,0.25f);
                var label = Instantiate(itemTemplate, row.transform); label.gameObject.SetActive(true);
                label.alignment = TextAnchor.MiddleLeft; label.text = $"{e.Name} — {FormatRemaining(e.ExpiresAt)}";
            }
#endif
        }

        static string FormatRemaining(long expiresAtMicros)
        {
            // SpacetimeDB commonly uses microseconds since Unix epoch.
            long nowMicros = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000L;
            long remainingMicros = expiresAtMicros - nowMicros;
            if (remainingMicros <= 0) return "Expired";

            // Convert microseconds to a TimeSpan with millisecond precision
            var ts = TimeSpan.FromMilliseconds(remainingMicros / 1000.0);
            if (ts.TotalHours >= 1)
                return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
            return string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
        }
    }
}

