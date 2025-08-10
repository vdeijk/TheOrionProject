using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    // Handles projectile-related audio effects
    public class ProjectileAudioService
    {
        private static ProjectileAudioService _instance;

        public ProjectileAudioData Data { get; set; }

        public static ProjectileAudioService Instance => _instance ??= new ProjectileAudioService();

        public void Init(ProjectileAudioData data)
        {
            Data = data;
        }

        public void CreateShootClip(ShootActionData shootActionData)
        {
            // Play shoot sound at weapon slot
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            WeaponSO weaponData = (WeaponSO)unit.Data.PartsData[shootActionData.WeaponPartType];
            Transform slotTransform = unit.Data.TransformData[shootActionData.WeaponPartType];
            AudioClip audioClip = weaponData.shootClip;
            Transform audioInstance = Data.Controller.InstantiateProjectile(slotTransform.position);
            audioInstance.GetComponent<AudioSource>().clip = audioClip;
            audioInstance.GetComponent<AudioSource>().Play();
        }

        public void CreateHitClip(UnitSingleController targetUnit, Vector3 targetPos)
        {
            // Play hit sound at target position
            WeaponSO weaponData = (WeaponSO)targetUnit.Data.PartsData[PartType.WeaponPrimary];
            AudioClip audioClip = weaponData.hitClip;
            Transform audioInstance = Data.Controller.InstantiateProjectile(targetPos);
            audioInstance.GetComponent<AudioSource>().clip = audioClip;
            audioInstance.GetComponent<AudioSource>().Play();
        }
    }
}
