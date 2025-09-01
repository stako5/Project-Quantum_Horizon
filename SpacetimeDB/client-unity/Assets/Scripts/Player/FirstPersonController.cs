using UnityEngine;
using MMORPG.Client.Combat;

namespace MMORPG.Client.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4.2f;
        [SerializeField] private float sprintSpeed = 6.8f;
        [SerializeField] private float rotateSpeed = 720f; // for body snapping to camera yaw
        [SerializeField] private float gravity = -18f;
        [SerializeField] private float jumpSpeed = 5.2f;

        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaRegen = 20f;
        [SerializeField] private float jumpCost = 15f;
        [SerializeField] private float attackCost = 20f;

        [Header("Combat")]
        [SerializeField] private Health health;
        [SerializeField] private Collider weaponHitbox;

        [Header("Config")]
        [SerializeField] private FirstPersonConfig config;
        [SerializeField] private bool useConfig = true;

        private CharacterController _cc;
        private BuffManager _buffs;
        private float _verticalVel;
        private float _stamina;
        private bool _attacking;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _buffs = GetComponent<BuffManager>();
            if (useConfig && !config) config = FirstPersonConfigSettings.Instance;
            _stamina = GetMaxStamina();
            if (weaponHitbox && weaponHitbox.GetComponent<MeleeHitboxDamage>() == null)
            {
                var mhd = weaponHitbox.gameObject.AddComponent<MeleeHitboxDamage>();
                mhd.SetOwner(transform, _buffs, health);
            }
        }

        void Update()
        {
            HandleStamina();
            HandleMovement();

            var im = MMORPG.Client.InputSystem.InputManager.Instance;
            bool attackDown = im ? im.AttackDown() : Input.GetMouseButtonDown(0);
            bool jumpDown = Input.GetKeyDown(KeyCode.Space);
            if (!_attacking && attackDown) TryAttack();
            if (jumpDown) TryJump();
        }

        void HandleStamina()
        {
            float regen = GetStaminaRegen();
            if (_buffs != null)
            {
                var mods = _buffs.GetModifiers();
                regen *= Mathf.Max(0.1f, mods.staminaRegenMult);
            }
            if (!_attacking)
                _stamina = Mathf.Min(GetMaxStamina(), _stamina + regen * Time.deltaTime);
        }

        void HandleMovement()
        {
            var im = MMORPG.Client.InputSystem.InputManager.Instance;
            Vector2 mv = im ? im.MoveAxis() : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            var input = new Vector3(mv.x, 0f, mv.y);

            // Move relative to player yaw (camera rotates player root)
            var move = transform.TransformDirection(input).normalized;
            float speed = (Input.GetKey(KeyCode.LeftShift) ? GetSprintSpeed() : GetWalkSpeed());
            if (_buffs != null)
            {
                var mods = _buffs.GetModifiers();
                speed *= Mathf.Clamp(mods.moveSpeedMult, 0.25f, 2.0f);
            }
            move *= speed;
            if (_cc.isGrounded) _verticalVel = -1f; else _verticalVel += GetGravity() * Time.deltaTime;
            move.y = _verticalVel;
            _cc.Move(move * Time.deltaTime);
        }

        void TryJump()
        {
            if (!_cc.isGrounded) return;
            if (_stamina < GetJumpCost()) return;
            _stamina -= GetJumpCost();
            _verticalVel = GetJumpSpeed();
        }

        void TryAttack()
        {
            if (_stamina < GetAttackCost()) return;
            _stamina -= GetAttackCost();
            _attacking = true;
            ActivateHitbox(true);
            Invoke(nameof(EndAttack), 0.3f);
        }

        float GetWalkSpeed() => (useConfig && config) ? config.walkSpeed : walkSpeed;
        float GetSprintSpeed() => (useConfig && config) ? config.sprintSpeed : sprintSpeed;
        float GetJumpSpeed() => (useConfig && config) ? config.jumpSpeed : jumpSpeed;
        float GetGravity() => (useConfig && config) ? config.gravity : gravity;
        float GetMaxStamina() => (useConfig && config) ? config.maxStamina : maxStamina;
        float GetStaminaRegen() => (useConfig && config) ? config.staminaRegen : staminaRegen;
        float GetJumpCost() => (useConfig && config) ? config.jumpCost : jumpCost;
        float GetAttackCost() => (useConfig && config) ? config.attackCost : attackCost;

        void EndAttack()
        {
            ActivateHitbox(false);
            _attacking = false;
        }

        void ActivateHitbox(bool active)
        {
            if (weaponHitbox) weaponHitbox.enabled = active;
        }
    }
}
