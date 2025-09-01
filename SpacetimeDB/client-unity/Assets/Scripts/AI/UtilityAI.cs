using System;
using System.Collections.Generic;
using UnityEngine;
using MMORPG.Client.NPC.Cognition;
using MMORPG.Client.AI.Actions;

namespace MMORPG.Client.AI
{
    public abstract class UtilityAction : ScriptableObject
    {
        [Range(0f, 10f)] public float weight = 1f;
        [SerializeField] private string displayName;
        [Header("Timing")]
        [Tooltip("Cooldown after finishing this action.")]
        public float cooldownSeconds = 0.5f;
        [Tooltip("Minimum time to stay on this action once selected.")]
        public float minActiveSeconds = 0.2f;
        [Tooltip("Optional timed phases; if any > 0, the agent will keep this action until recovery ends.")]
        public float windupSeconds = 0f;
        public float activeSeconds = 0f;
        public float recoverySeconds = 0f;
        [Header("Root Windows")]
        public bool rootDuringWindup = false;
        public bool rootDuringActive = false;
        public bool rootDuringRecovery = false;
        public abstract float Score(UtilityContext ctx);
        public abstract void Execute(UtilityContext ctx, float dt);
        public string Name => string.IsNullOrEmpty(displayName) ? name : displayName;

        // Cognitive memory signature for this action. Override in custom actions.
        public virtual void GetMemorySignature(UtilityContext ctx, out float valence, out string tag)
        {
            valence = 0.05f; tag = "act";
        }

        // Lifecycle hooks when the agent switches actions
        public virtual void OnEnter(UtilityContext ctx) {}
        public virtual void OnExit(UtilityContext ctx) {}
    }

    [Serializable]
    public class UtilityContext
    {
        public Transform self;
        public Transform target;
        public float health01;
        public float stamina01;
        public float distToTarget;
        public ActionPhase phase;
        public float phaseElapsed;
        public float phaseDuration;
    }

    public class UtilityAgent : MonoBehaviour
    {
        [SerializeField] private List<UtilityAction> actions = new();
        [SerializeField] private Transform target;
        [SerializeField] private float thinkInterval = 0.2f;

        private UtilityAction _current;
        private float _nextThink;
        private UtilityContext _ctx = new();
        private readonly Dictionary<UtilityAction, ActionState> _states = new();

        public event Action<UtilityAction, UtilityAction> ActionChanged; // (prev, next)

        void Update()
        {
            if (Time.time >= _nextThink)
            {
                _nextThink = Time.time + thinkInterval;
                Think();
            }
            UpdatePhaseAndRoot();
            _current?.Execute(_ctx, Time.deltaTime);
        }

        void Think()
        {
            _ctx.self = transform;
            _ctx.target = target;
            _ctx.distToTarget = target ? Vector3.Distance(transform.position, target.position) : float.PositiveInfinity;
            // Health/Stamina could be read from components; set placeholders 0..1
            _ctx.health01 = 1f;
            _ctx.stamina01 = 1f;

            // Enforce lock-in for current action
            if (_current != null)
            {
                var st = GetState(_current);
                if (UsesPhases(_current))
                {
                    if (!st.phasesComplete) return; // keep action until recovery completes
                }
                else
                {
                    float activeFor = Time.time - st.enteredAt;
                    if (activeFor < Mathf.Max(0f, _current.minActiveSeconds)) return;
                }
            }

            float best = float.NegativeInfinity;
            UtilityAction next = _current; // default to staying
            foreach (var a in actions)
            {
                if (!a) continue;
                var st = GetState(a);
                if (Time.time < st.cooldownUntil) continue; // on cooldown
                var s = a.weight * a.Score(_ctx);
                if (s > best) { best = s; next = a; }
            }
            if (!ReferenceEquals(_current, next))
            {
                var prev = _current;
                // Exit prev: start its cooldown
                if (prev != null)
                {
                    var pst = GetState(prev);
                    pst.cooldownUntil = Time.time + Mathf.Max(0f, prev.cooldownSeconds);
                    prev.OnExit(_ctx);
                }
                _current = next;
                if (_current != null)
                {
                    var nst = GetState(_current);
                    nst.enteredAt = Time.time;
                    // Initialize phases
                    if (UsesPhases(_current)) { nst.phase = ActionPhase.Windup; nst.phaseStart = Time.time; nst.phasesComplete = false; }
                    else { nst.phase = ActionPhase.Active; nst.phaseStart = Time.time; nst.phasesComplete = false; }
                    _current.OnEnter(_ctx);
                }
                ActionChanged?.Invoke(prev, _current);
                LogActionChange(_current);
            }
        }

        void LogActionChange(UtilityAction act)
        {
            if (act == null) return;
            var cog = GetComponent<NpcCognition>();
            if (!cog) return;
            act.GetMemorySignature(_ctx, out var val, out var tag);
            // Include time-of-day tag for narrative richness
            string hourTag = $"h{System.DateTime.UtcNow.Hour:00}";
            cog.RecordAction(act.Name, val, tag, hourTag);
        }

        public void SetTarget(Transform t) { target = t; }

        // Allow runtime population of actions from brain bootstrappers
        public void SetActions(List<UtilityAction> list)
        {
            actions = list != null ? new List<UtilityAction>(list) : new List<UtilityAction>();
        }

        // Expose current action and context for systems like melee hitboxes/telegraphs
        public UtilityAction Current => _current;
        public UtilityContext Context => _ctx;
        public System.Collections.Generic.IReadOnlyList<UtilityAction> Actions => actions;
        public float GetCooldownRemaining(UtilityAction a)
        {
            if (a == null) return 0f;
            var st = GetState(a);
            return Mathf.Max(0f, st.cooldownUntil - Time.time);
        }

        ActionState GetState(UtilityAction a)
        {
            if (!_states.TryGetValue(a, out var st))
            {
                st = new ActionState();
                _states[a] = st;
            }
            return st;
        }

        void UpdatePhaseAndRoot()
        {
            if (_current == null) return;
            var st = GetState(_current);
            // Progress phases
            if (UsesPhases(_current) && !st.phasesComplete)
            {
                float now = Time.time;
                float dur = GetPhaseDuration(_current, st.phase);
                float elapsed = now - st.phaseStart;
                if (elapsed >= dur)
                {
                    // Advance
                    switch (st.phase)
                    {
                        case ActionPhase.Windup:
                            st.phase = ActionPhase.Active; st.phaseStart = now; break;
                        case ActionPhase.Active:
                            st.phase = ActionPhase.Recovery; st.phaseStart = now; break;
                        case ActionPhase.Recovery:
                            st.phasesComplete = true; break;
                    }
                }
            }

            // Populate context
            var st2 = GetState(_current);
            _ctx.phase = st2.phase;
            _ctx.phaseElapsed = Time.time - st2.phaseStart;
            _ctx.phaseDuration = GetPhaseDuration(_current, st2.phase);

            // Root handling
            bool root = false;
            switch (_ctx.phase)
            {
                case ActionPhase.Windup: root = _current.rootDuringWindup; break;
                case ActionPhase.Active: root = _current.rootDuringActive; break;
                case ActionPhase.Recovery: root = _current.rootDuringRecovery; break;
            }
            var mover = GetComponent<MMORPG.Client.Enemies.EnemyMover>();
            if (mover) mover.SetRooted(root);
        }

        static bool UsesPhases(UtilityAction a) => a.windupSeconds > 0f || a.activeSeconds > 0f || a.recoverySeconds > 0f;
        static float GetPhaseDuration(UtilityAction a, ActionPhase p)
        {
            return p switch
            {
                ActionPhase.Windup => Mathf.Max(0f, a.windupSeconds),
                ActionPhase.Active => Mathf.Max(0f, a.activeSeconds),
                ActionPhase.Recovery => Mathf.Max(0f, a.recoverySeconds),
                _ => 0f,
            };
        }

        class ActionState { public float cooldownUntil; public float enteredAt; public ActionPhase phase = ActionPhase.Active; public float phaseStart; public bool phasesComplete; }
    }

    public enum ActionPhase { Windup, Active, Recovery }
}
