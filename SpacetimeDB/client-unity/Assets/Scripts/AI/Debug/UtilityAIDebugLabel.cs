using System.Text;
using UnityEngine;

namespace MMORPG.Client.AI.Debugging
{
    public class UtilityAIDebugLabel : MonoBehaviour
    {
        [SerializeField] private MMORPG.Client.AI.UtilityAgent agent;
        [SerializeField] private Color color = new Color(1f, 1f, 0.6f, 1f);
        [SerializeField] private float refreshInterval = 0.25f;
        [SerializeField] private KeyCode toggleKey = KeyCode.F2;
        [SerializeField] private bool startVisible = false;

        private float _t;
        private TextMesh _tm;
        private bool _visible;

        void Awake()
        {
            if (!agent) agent = GetComponent<MMORPG.Client.AI.UtilityAgent>();
            var go = new GameObject("AI_DebugLabel");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0f, 2.2f, 0f);
            _tm = go.AddComponent<TextMesh>();
            _tm.anchor = TextAnchor.LowerCenter; _tm.alignment = TextAlignment.Center;
            _tm.characterSize = 0.08f; _tm.fontSize = 52; _tm.color = color;
            _visible = startVisible;
            _tm.gameObject.SetActive(_visible);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey)) { _visible = !_visible; _tm.gameObject.SetActive(_visible); }
            if (!_visible) return;
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
            if (!agent || _tm == null) return;
            _tm.text = BuildText();
            // Always face camera
            if (Camera.main) _tm.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        }

        string BuildText()
        {
            var sb = new StringBuilder();
            var cur = agent.Current;
            var ctx = agent.Context;
            var id = GetComponent<MMORPG.Client.Enemies.EnemyIdentity>();
            if (id) sb.AppendLine($"{id.DisplayName} (T{id.Tier})");
            sb.Append("Cur: ").Append(cur != null ? cur.Name : "-");
            if (cur != null)
            {
                float rem = Mathf.Max(0f, ctx.phaseDuration - ctx.phaseElapsed);
                sb.Append($"  [{ctx.phase}] {rem:0.00}s");
            }
            sb.AppendLine();
            var acts = agent.Actions;
            if (acts != null)
            {
                for (int i = 0; i < acts.Count; i++)
                {
                    var a = acts[i]; if (a == null) continue;
                    float cd = agent.GetCooldownRemaining(a);
                    bool isCur = ReferenceEquals(cur, a);
                    sb.Append(isCur ? "*" : " ");
                    sb.Append(a.Name).Append(": ");
                    sb.Append(cd > 0.001f ? $"CD {cd:0.00}s" : "ready");
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
    }
}

