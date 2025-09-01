using System.Text;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Enemies;

namespace MMORPG.Client.UI
{
    public class BestiaryDetailUI : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text statsText;
        [SerializeField] private Text descText;

        [Header("Preview")]
        [SerializeField] private RawImage previewImage;
        [SerializeField] private BestiaryPreviewRenderer previewRenderer;

        public void Show(EnemyTypeModel e, GameObject prefab)
        {
            if (nameText) nameText.text = e.name;
            if (statsText)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Family: {e.family}  Tier: {e.tier}");
                sb.AppendLine($"Role: {e.role}  Element: {e.element}");
                sb.AppendLine($"HP: {e.hp}  DMG: {e.damage}  SPD: {e.speed:F1}");
                // Optional details: stage, size, and parsed O/C/E
                var stage = TryGetTagValue(e, "stage:");
                if (!string.IsNullOrEmpty(e.size) || !string.IsNullOrEmpty(stage))
                {
                    sb.AppendLine($"Stage: {stage ?? "?"}  Size: {e.size}");
                }
                var o = TryGetTagValue(e, "O:"); var c = TryGetTagValue(e, "C:"); var ex = TryGetTagValue(e, "E:");
                if (!string.IsNullOrEmpty(o) || !string.IsNullOrEmpty(c) || !string.IsNullOrEmpty(ex))
                {
                    sb.AppendLine($"O/C/E: {o ?? "-"} / {c ?? "-"} / {ex ?? "-"}");
                }
                statsText.text = sb.ToString();
            }
            if (descText) descText.text = e.description;
            if (previewRenderer)
            {
                previewRenderer.Render(prefab);
                if (previewImage) previewImage.texture = previewRenderer.Texture;
            }
        }

        static string TryGetTagValue(EnemyTypeModel e, string prefix)
        {
            if (e == null || e.tags == null || string.IsNullOrEmpty(prefix)) return null;
            var t = e.tags.FirstOrDefault(x => x != null && x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(t)) return null;
            var val = t.Substring(prefix.Length);
            return string.IsNullOrEmpty(val) ? null : val;
        }
    }
}
