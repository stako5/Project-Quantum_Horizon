using UnityEngine;

namespace MMORPG.Client.Enemies
{
    [RequireComponent(typeof(EnemyIdentity))]
    public class EnemyDeathWatcher : MonoBehaviour
    {
        [SerializeField] private Combat.Health health;
        [SerializeField] private EnemyIdentity identity;
        private bool _reported;

        public static System.Action<string> OnEnemyKilledFamily; // family

        void Awake()
        {
            if (!health) health = GetComponent<Combat.Health>();
            if (!identity) identity = GetComponent<EnemyIdentity>();
        }

        async void Update()
        {
            if (_reported) return;
            if (health && !health.IsAlive)
            {
                _reported = true;
                string family = identity ? identity.Family : string.Empty;
                await MMORPG.Client.Quests.QuestEventReporter.ReportAsync("kill_enemy", family ?? string.Empty, 1);
                try { OnEnemyKilledFamily?.Invoke(family ?? string.Empty); } catch {}
            }
        }
    }
}
