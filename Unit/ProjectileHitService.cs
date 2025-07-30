using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles projectile hit visual effects
    public class ProjectileHitService : Singleton<ProjectileHitService>
    {
        [SerializeField] Transform bulletPrefab;
        [SerializeField] Transform missiletPrefab;
        [SerializeField] Transform beamPrefab;

        public void CreateBeamhit(Vector3 targetPosition)
        {
            Instantiate(beamPrefab, targetPosition, Quaternion.identity);
        }

        public void CreateMissileHit(Vector3 targetPosition)
        {
            Instantiate(missiletPrefab, targetPosition, Quaternion.identity);
        }

        public void CreateBulletHit(Vector3 targetPosition)
        {
            Instantiate(bulletPrefab, targetPosition, Quaternion.identity);
        }
    }
}