using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.World
{
    [RequireComponent(typeof(Collider))]
    public class ActivityInteractable : MonoBehaviour
    {
        public string activityKey;
        public string biome;
        public uint cooldownSeconds = 300;
        public uint rewardCredits = 50;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private bool destroyOnComplete = true;

        private bool _near;

        void Reset()
        {
            var col = GetComponent<Collider>(); col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other) { _near = true; }
        void OnTriggerExit(Collider other) { _near = false; }

        async void Update()
        {
            if (_near && Input.GetKeyDown(interactKey)) await Complete();
        }

        public async Task Complete()
        {
            await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
            await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("complete_activity", activityKey, biome, (uint)cooldownSeconds, (uint)rewardCredits);
            // Quest trigger: world/poi activity often ties to active events
            await MMORPG.Client.Quests.QuestEventReporter.ReportAsync("active_event", biome ?? string.Empty, 1);
            if (destroyOnComplete) Destroy(gameObject);
        }
    }
}
