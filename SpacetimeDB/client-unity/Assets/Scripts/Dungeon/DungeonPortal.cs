using UnityEngine;

namespace MMORPG.Client.Dungeon
{
    [RequireComponent(typeof(Collider))]
    public class DungeonPortal : MonoBehaviour
    {
        public string TargetBossId;
        public string TargetBossName;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Seamless: ask the instance manager to spawn an instance and teleport the player
            if (string.IsNullOrEmpty(TargetBossId)) return;
            var mgr = DungeonInstanceManager.Instance;
            if (!mgr)
            {
                Debug.LogWarning("DungeonPortal: No DungeonInstanceManager in scene");
                return;
            }
            mgr.SpawnInstance(TargetBossId);
        }
    }
}
