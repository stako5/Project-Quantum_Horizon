using UnityEngine;

namespace MMORPG.Client.Combat
{
    public static class ProjectileShooter
    {
        public static void FireSimple(Transform origin, Vector3 dir, float speed, float damage)
        {
            var proj = ProjectilePool.Get();
            proj.transform.position = origin.position;
            proj.transform.rotation = Quaternion.LookRotation(dir.normalized == Vector3.zero ? origin.forward : dir.normalized, Vector3.up);
            proj.speed = speed; proj.damage = damage; proj.owner = origin;
            proj.usePooling = true;
        }
    }
}
