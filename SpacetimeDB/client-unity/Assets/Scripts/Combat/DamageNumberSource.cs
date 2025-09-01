using UnityEngine;

namespace MMORPG.Client.Combat
{
    [RequireComponent(typeof(Health))]
    public class DamageNumberSource : MonoBehaviour
    {
        private Health _hp;
        void Awake()
        {
            _hp = GetComponent<Health>();
            _hp.OnDamaged += OnDamaged;
        }
        void OnDestroy()
        {
            if (_hp != null) _hp.OnDamaged -= OnDamaged;
        }
        void OnDamaged(float amt)
        {
            MMORPG.Client.UI.DamageNumbers.Spawn(transform.position, amt);
        }
    }
}

