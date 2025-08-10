using UnityEngine;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    /// <summary>
    /// Service for instantiating and setting up projectiles based on weapon type.
    /// </summary>
    public class ProjectileController : MonoBehaviour
    {
        [field: SerializeField] public ProjectileData ProjectileData { get; private set; }
        [field: SerializeField] public ProjectileAudioData AudioData { get; private set; }
        [field: SerializeField] public ProjectileHitData HitData { get; private set; }

        private void Awake()
        {
            ProjectileService.Instance.Init(ProjectileData);
            ProjectileAudioService.Instance.Init(AudioData);
            ProjectileHitService.Instance.Init(HitData);
        }

        public void CreateBeamhit(Vector3 targetPosition)
        {
            Instantiate(HitData.BeamHitPrefab, targetPosition, Quaternion.identity);
        }

        public void CreateMissileHit(Vector3 targetPosition)
        {
            Instantiate(HitData.MissiletHitPrefab, targetPosition, Quaternion.identity);
        }

        public void CreateBulletHit(Vector3 targetPosition)
        {
            Instantiate(HitData.BulletHitPrefab, targetPosition, Quaternion.identity);
        }

        public Transform InstantiateAudioProjectile(Vector3 pos)
        {
            return Instantiate(AudioData.AudioPrefab, pos, Quaternion.identity);
        }

        public Transform InstantiateProjectile(Vector3 pos)
        {
            return Instantiate(ProjectileData.BulletPrefab, pos, Quaternion.identity);
        }

        public Transform InstantiateMissile( Vector3 pos)
        {
            return Instantiate(ProjectileData.MissileProjectilePrefab, pos, Quaternion.identity);
        }

        public Transform InstantiateBeam(Vector3 pos)
        {
            return Instantiate(ProjectileData.LaserPrefab, pos, Quaternion.identity);
        }
    }
}