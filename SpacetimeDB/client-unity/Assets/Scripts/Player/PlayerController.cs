using UnityEngine;
using MMORPG.Client.Combat;

namespace MMORPG.Client.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4.0f;
        [SerializeField] private float rotateSpeed = 720f;
        [SerializeField] private float gravity = -15f;

        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaRegen = 15f;
        [SerializeField] private float rollCost = 25f;
        [SerializeField] private float attackCost = 20f;

        [Header("Roll / Dodge")]
        [SerializeField] private float rollSpeed = 10f;
        [SerializeField] private float rollTime = 0.4f;

        [Header("Combat")]
        [SerializeField] private Health health;
        [SerializeField] private LockOnSystem lockOn;
        [SerializeField] private Collider weaponHitbox;
        [SerializeField] private bool lockRootDuringAttack = true;

        private CharacterController _cc;
        private BuffManager _buffs;
        private float _verticalVel;
        private float _stamina;
        private float _rollUntil;
        private bool _attacking;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _buffs = GetComponent<BuffManager>();
            _stamina = maxStamina;
            if (weaponHitbox && weaponHitbox.GetComponent<MMORPG.Client.Combat.MeleeHitboxDamage>() == null)
            {
                var mhd = weaponHitbox.gameObject.AddComponent<MMORPG.Client.Combat.MeleeHitboxDamage>();
                mhd.SetOwner(transform, _buffs, health);
            }
        }

        void Update()
        {
            HandleStamina();
            if (Time.time < _rollUntil) HandleRollMovement();
            else HandleNormalMovement();

            // Input routing: prefer InputManager if present, else fallback to legacy
            var im = MMORPG.Client.InputSystem.InputManager.Instance;
            bool attackDown = im ? im.AttackDown() : Input.GetMouseButtonDown(0);
            bool rollDown = im ? im.RollDown() : Input.GetKeyDown(KeyCode.Space);
            if (!_attacking && attackDown) TryAttack();
            if (Time.time >= _rollUntil && rollDown) TryRoll();
        }

        void HandleStamina()
        {
            float regen = staminaRegen;
            if (_buffs != null)
            {
                var mods = _buffs.GetModifiers();
                regen *= Mathf.Max(0.1f, mods.staminaRegenMult);
            }
            if (Time.time >= _rollUntil && !_attacking)
                _stamina = Mathf.Min(maxStamina, _stamina + regen * Time.deltaTime);
        }

        void HandleNormalMovement()
        {
            var im = MMORPG.Client.InputSystem.InputManager.Instance;
            Vector2 mv = im ? im.MoveAxis() : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            var h = mv.x;
            var v = mv.y;
            var input = new Vector3(h, 0f, v).normalized;

            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;

            if (Camera.main)
            {
                var camF = Camera.main.transform.forward; camF.y = 0f; camF.Normalize();
                var camR = Camera.main.transform.right;   camR.y = 0f; camR.Normalize();
                forward = camF; right = camR;
            }

            var move = (forward * input.z + right * input.x).normalized;
            if (move.sqrMagnitude > 0.001f)
            {
                var targetRot = Quaternion.LookRotation(lockOn != null && lockOn.CurrentTarget != null ?
                    (lockOn.CurrentTarget.position - transform.position).normalized : move);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }

            if (_cc.isGrounded) _verticalVel = -1f; else _verticalVel += gravity * Time.deltaTime;
            move.y = 0f;
            float speed = moveSpeed;
            if (lockRootDuringAttack && _attacking) speed = 0f;
            if (_buffs != null)
            {
                var mods = _buffs.GetModifiers();
                speed *= Mathf.Clamp(mods.moveSpeedMult, 0.25f, 2.0f);
            }
            move *= speed;
            move.y = _verticalVel;
            _cc.Move(move * Time.deltaTime);
        }

        void HandleRollMovement()
        {
            float rSpeed = rollSpeed;
            if (_buffs != null)
            {
                var mods = _buffs.GetModifiers();
                rSpeed *= Mathf.Clamp(mods.moveSpeedMult, 0.25f, 2.0f);
            }
            var dir = transform.forward * rSpeed;
            if (_cc.isGrounded) _verticalVel = -1f; else _verticalVel += gravity * Time.deltaTime;
            dir.y = _verticalVel;
            _cc.Move(dir * Time.deltaTime);
        }

        void TryRoll()
        {
            if (_stamina < rollCost) return;
            _stamina -= rollCost;
            _rollUntil = Time.time + rollTime;
            if (health) health.GrantIFrames();
        }

        void TryAttack()
        {
            if (_stamina < attackCost) return;
            _stamina -= attackCost;
            _attacking = true;
            ActivateHitbox(true);
            Invoke(nameof(EndAttack), 0.35f); // startup+active+recovery window simplified
        }

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
