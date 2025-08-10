using UnityEngine;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    // Handles projectile hit visual effects
    public class ProjectileHitService
    {
        private static ProjectileHitService _instance;

        public ProjectileHitData Data { get; set; }

        public static ProjectileHitService Instance => _instance ??= new ProjectileHitService();


        public void Init(ProjectileHitData data)
        {
            Data = data;
        }

        public void CreateBeamhit(Vector3 targetPosition)
        {
            Data.Controller.CreateBeamhit(targetPosition);
        }

        public void CreateMissileHit(Vector3 targetPosition)
        {
            Data.Controller.CreateMissileHit(targetPosition);
        }

        public void CreateBulletHit(Vector3 targetPosition)
        {
            Data.Controller.CreateBulletHit(targetPosition);
        }
    }
}