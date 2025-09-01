using UnityEngine;

namespace MMORPG.Client.NPC
{
    public class NpcInteractable : MonoBehaviour
    {
        public enum Mode { Talk, Deliver }

        [SerializeField] private Mode mode = Mode.Talk;
        [SerializeField] private string role = "Archivist"; // used as event_param
        [SerializeField] private string deliverItemName = ""; // optional narrative only
        [SerializeField] private uint deliverQty = 1;
        [SerializeField] private Transform player;
        [SerializeField] private float interactRange = 3.0f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        void Awake()
        {
            if (!player)
            {
                var p = GameObject.FindWithTag("Player");
                if (p) player = p.transform;
            }
        }

        async void Update()
        {
            if (!player) return;
            float d = Vector3.Distance(transform.position, player.position);
            if (d <= interactRange && Input.GetKeyDown(interactKey))
            {
                if (mode == Mode.Talk)
                {
                    await MMORPG.Client.Quests.QuestEventReporter.ReportAsync("dialogue_node", role ?? string.Empty, 1);
                }
                else // Deliver
                {
                    await MMORPG.Client.Quests.QuestEventReporter.ReportAsync("deliver_item", role ?? string.Empty, 1);
                }
            }
        }
    }
}
