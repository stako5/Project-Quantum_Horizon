using System.Linq;
using UnityEngine;
using MMORPG.Client.Net;
using MMORPG.Client.NPC.Cognition;

namespace MMORPG.Client.NPC
{
    public class NpcMemorySync : MonoBehaviour
    {
        [SerializeField] private ulong npcId;
        [SerializeField] private NpcCognitiveMemory memory;
        [SerializeField] private float refreshInterval = 2.0f;

        private float _t;

        void Awake()
        {
            if (!memory) memory = GetComponent<NpcCognitiveMemory>();
        }

        void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
            var rows = BindingsBridge.GetNpcMemories().Where(m => m.NpcId == npcId).OrderBy(m => m.At).ToList();
            // naive: replay into memory; duplicates ignored by time check
            if (memory != null)
            {
                foreach (var r in rows)
                {
                    float valence = 0f; // could be parsed from description/tags later
                    memory.Add(r.Kind, r.Description, valence, 0.5f, Mathf.Clamp01(r.Importance/10f), r.Kind);
                }
            }
        }
    }
}
