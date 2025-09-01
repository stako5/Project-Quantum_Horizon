using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Combat;

namespace MMORPG.Client.UI
{
    public class ActiveBuffsDebugUI : MonoBehaviour
    {
        [SerializeField] private BuffManager manager;
        [SerializeField] private Text text;
        [SerializeField] private float refreshInterval = 0.5f;
        private float _t;

        void Awake()
        {
            if (!manager) manager = FindObjectOfType<BuffManager>();
        }

        void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
            if (!manager || !text) return;
            var sb = new StringBuilder();
            var mods = manager.GetModifiers();
            sb.AppendLine($"DMG x{mods.damageMult:F2}, DEF x{mods.defenseMult:F2}, SPD x{mods.moveSpeedMult:F2}");
            sb.AppendLine($"CRIT+{mods.critChanceAdd:P0}, CDmg x{mods.critDamageMult:F2}, CDR {1f-mods.cooldownReductionMult:P0}");
            sb.AppendLine($"STA x{mods.staminaRegenMult:F2}, HP+{mods.healthRegenFlat:F1}/s, LS {mods.lifeStealPct:P0}");
            sb.AppendLine($"Res F/I/S/V {mods.resistFire:P0}/{mods.resistIce:P0}/{mods.resistShock:P0}/{mods.resistVoid:P0}");
            sb.AppendLine($"Drop x{mods.dropRateMult:F2}, XP x{mods.xpGainMult:F2}, Gold x{mods.goldFindMult:F2}");
            var buffs = manager.ActiveBuffs().OrderBy(b => b.remainingS).ToList();
            foreach (var b in buffs)
            {
                sb.AppendLine($"- {b.def.name} (T{b.def.tier}) {b.remainingS:F0}s");
            }
            text.text = sb.ToString();
        }
    }
}

