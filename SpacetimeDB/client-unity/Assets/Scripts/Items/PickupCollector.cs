using UnityEngine;

namespace MMORPG.Client.Items
{
    public class PickupCollector : MonoBehaviour
    {
        public float radius = 1.5f;
        public LayerMask pickupMask = ~0;

        void Update()
        {
            var hits = Physics.OverlapSphere(transform.position, radius, pickupMask, QueryTriggerInteraction.Collide);
            var inv = GetComponentInParent<Inventory>();
            var prog = GetComponentInParent<MMORPG.Client.Player.PlayerProgress>();
            foreach (var h in hits)
            {
                var p = h.GetComponent<Pickup>();
                if (p) p.Collect(inv, prog);
            }
        }
    }
}
