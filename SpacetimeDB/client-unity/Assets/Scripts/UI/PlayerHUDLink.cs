using System.Reflection;
using UnityEngine;

namespace MMORPG.Client.UI
{
    public class PlayerHUDLink : MonoBehaviour
    {
        [SerializeField] private SoulsHUD hud;
        [SerializeField] private Combat.Health health;
        [SerializeField] private Player.PlayerController controller;
        [SerializeField] private Items.Inventory inventory;

        private FieldInfo _staminaField;
        private FieldInfo _maxStaminaField;

        void Awake()
        {
            if (!hud) hud = FindObjectOfType<SoulsHUD>();
            if (!health) health = GetComponentInChildren<Combat.Health>();
            if (!controller) controller = GetComponentInChildren<Player.PlayerController>();
            if (!inventory) inventory = GetComponentInChildren<Items.Inventory>();
            if (inventory) inventory.OnGoldChanged += OnGoldChanged;
            // Reflection fallback for stamina fields
            if (controller)
            {
                var t = controller.GetType();
                _staminaField = t.GetField("_stamina", BindingFlags.NonPublic | BindingFlags.Instance);
                _maxStaminaField = t.GetField("maxStamina", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        void OnDestroy()
        {
            if (inventory) inventory.OnGoldChanged -= OnGoldChanged;
        }

        void Update()
        {
            if (!hud) return;
            if (health)
            {
                hud.SetHP01(health.maxHealth > 0f ? health.currentHealth / health.maxHealth : 1f);
            }
            float sta01 = 1f;
            if (controller && _staminaField != null)
            {
                float sta = (float)_staminaField.GetValue(controller);
                float max = _maxStaminaField != null ? (float)_maxStaminaField.GetValue(controller) : 100f;
                sta01 = max > 0f ? sta / max : 1f;
            }
            hud.SetStamina01(sta01);
        }

        void OnGoldChanged(long g)
        {
            if (hud) hud.SetSouls(g);
        }
    }
}

