using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles projectile-related audio effects
    public class ProjectileAudioService : Singleton<ProjectileAudioService>
    {
        [SerializeField] Transform audioPrefab;

        public void CreateShootClip(ShootAction shootAction)
        {
            // Play shoot sound at weapon slot
            Unit unit = UnitSelectService.Instance.selectedUnit;
            WeaponData weaponData = (WeaponData)unit.partsData[shootAction.weaponPartType];
            Transform slotTransform = unit.transformData[shootAction.weaponPartType];
            AudioClip audioClip = weaponData.shootClip;
            Transform audioInstance = Instantiate(audioPrefab, slotTransform.position, Quaternion.identity);
            audioInstance.GetComponent<AudioSource>().clip = audioClip;
            audioInstance.GetComponent<AudioSource>().Play();
        }

        public void CreateHitClip(Unit targetUnit, Vector3 targetPos)
        {
            // Play hit sound at target position
            WeaponData weaponData = (WeaponData)targetUnit.partsData[PartType.WeaponPrimary];
            AudioClip audioClip = weaponData.hitClip;
            Transform audioInstance = Instantiate(audioPrefab, targetPos, Quaternion.identity);
            audioInstance.GetComponent<AudioSource>().clip = audioClip;
            audioInstance.GetComponent<AudioSource>().Play();
        }
    }
}
