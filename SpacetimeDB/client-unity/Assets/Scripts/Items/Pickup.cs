using UnityEngine;

namespace MMORPG.Client.Items
{
    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        public enum Kind { Gold, Item, XP }
        public Kind kind = Kind.Gold;
        public string itemId;
        public int amount = 1;
        public float rotateSpeed = 90f;

        void Awake()
        {
            var col = GetComponent<Collider>(); col.isTrigger = true;
        }

        void Update()
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other) return;
            Collect(other.GetComponentInParent<Inventory>(), other.GetComponentInParent<MMORPG.Client.Player.PlayerProgress>());
        }

        public void Collect(Inventory inv, MMORPG.Client.Player.PlayerProgress prog = null)
        {
            if (!inv && kind != Kind.XP) return;
            switch (kind)
            {
                case Kind.Gold: inv.AddGold(amount); break;
                case Kind.Item: inv.AddItem(itemId, amount); break;
                case Kind.XP:
                    if (prog == null)
                        prog = inv ? inv.GetComponentInParent<MMORPG.Client.Player.PlayerProgress>() : null;
                    if (prog) prog.AddXP(amount);
                    break;
            }
            Destroy(gameObject);
        }
    }
}
