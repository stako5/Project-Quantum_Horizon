using UnityEngine;

namespace MMORPG.Client.Net
{
    public class PlayerNetSync : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float sendInterval = 0.1f; // 10 Hz
        [SerializeField] private float minDistDelta = 0.05f;
        [SerializeField] private float minYawDelta = 1.0f;
        [SerializeField] private byte regionId = 0;

        private float _t;
        private Vector3 _lastSentPos;
        private float _lastSentYaw;

        void Awake()
        {
            if (!target) target = transform;
        }

        async void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = sendInterval;
#if SPACETIMEDB_SDK
            await SpacetimeDbClient.Instance.ConnectAsync();
            var pos = target.position; var yaw = target.eulerAngles.y;
            if ((pos - _lastSentPos).sqrMagnitude < (minDistDelta * minDistDelta) && Mathf.Abs(Mathf.DeltaAngle(_lastSentYaw, yaw)) < minYawDelta) return;
            _lastSentPos = pos; _lastSentYaw = yaw;
            await SpacetimeDbClient.Instance.CallReducerAsync("update_player_state", pos.x, pos.y, pos.z, yaw);
#endif
        }
    }
}
