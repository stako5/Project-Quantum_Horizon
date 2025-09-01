using UnityEngine;

namespace MMORPG.Client.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        [SerializeField] private InputBindings bindings;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
            bindings = InputBindings.Load();
        }

        public InputBindings Bindings => bindings;
        public void Save() => bindings.Save();

        // Axes rely on Unity defaults (Horizontal/Vertical) for keyboard+gamepad
        public Vector2 MoveAxis()
        {
            float h = UnityEngine.Input.GetAxisRaw("Horizontal");
            float v = UnityEngine.Input.GetAxisRaw("Vertical");
            return new Vector2(h, v);
        }

        public bool AttackDown() => KeyDown("Attack");
        public bool RollDown() => KeyDown("Roll");
        public bool LockOnDown() => KeyDown("LockOn");
        public bool QuickDown(int i) => KeyDown(i switch { 1 => "Quick1", 2 => "Quick2", 3 => "Quick3", 4 => "Quick4", _ => "" });

        bool KeyDown(string action)
        {
            if (string.IsNullOrEmpty(action) || !bindings.Keys.TryGetValue(action, out var kc)) return false;
            return UnityEngine.Input.GetKeyDown(kc);
        }
    }
}
