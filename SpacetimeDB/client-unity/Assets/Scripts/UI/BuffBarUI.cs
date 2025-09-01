
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Combat;

namespace MMORPG.Client.UI
{
    public class BuffBarUI : MonoBehaviour
    {
        [SerializeField] private BuffManager buffManager;
        [SerializeField] private RectTransform content;
        [SerializeField] private Text itemTemplate;
        [SerializeField] private float refreshInterval = 0.5f;

        private float _t;

        void Awake()
        {
            if (!buffManager) buffManager = FindObjectOfType<BuffManager>();
        }

        void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
            Rebuild();
        }

        void Rebuild()
        {
            foreach (Transform child in content) Destroy(child.gameObject);
            var list = buffManager?.ActiveBuffs()?.OrderByDescending(b => b.remainingS).ToList();
            if (list == null) return;
            foreach (var b in list)
            {
                var row = new GameObject("Buff"); row.transform.SetParent(content, false);
                var rt = row.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 20);
                var bg = row.AddComponent<Image>(); bg.color = new Color(0,0,0,0.2f);
                var txt = Instantiate(itemTemplate, row.transform); txt.gameObject.SetActive(true);
                var stacks = b.stacks > 1 ? $" x{b.stacks}" : string.Empty;
                txt.text = $"{b.def.name}{stacks}  {Format(b.remainingS)}";
                txt.alignment = TextAnchor.MiddleLeft;
            }
        }

        static string Format(float s)
        {
            if (s < 0f) s = 0f; int m = Mathf.FloorToInt(s / 60f); int ss = Mathf.FloorToInt(s % 60f);
            if (m > 0) return string.Format("{0:D2}:{1:D2}", m, ss);
            return ss + "s";
        }
    }
}
