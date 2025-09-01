using UnityEngine;

namespace MMORPG.Client.Dungeon
{
    [RequireComponent(typeof(Collider))]
    public class DungeonExitPortal : MonoBehaviour
    {
        private void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var mgr = DungeonInstanceManager.Instance;
            if (mgr) mgr.ExitInstance();
        }
    }
}

