using UnityEngine;
using System.Collections;

namespace TurnBasedStrategy
{
    // Handles weapon rotation towards target and back
    public class WeaponMovement : MonoBehaviour
    {
        [SerializeField] Unit unit;
        private Unit selectedTarget => UnitSelectService.Instance.selectedTarget;
        private Quaternion originalRotation;

        // Rotates weapon part towards the selected target
        public void RotateWeaponTowardsTarget(PartType weaponPartType, float duration = 0.3f)
        {
            Transform weaponTransform = unit.transformData[weaponPartType];
            if (weaponTransform == null || selectedTarget == null) return;

            originalRotation = weaponTransform.rotation;
            Vector3 targetPos = selectedTarget.transform.position;
            Vector3 direction = (targetPos - weaponTransform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            StopAllCoroutines();
            StartCoroutine(LerpRotation(weaponTransform, targetRotation, duration));
        }

        // Rotates weapon part back to its original rotation
        public void RotateWeaponBack(PartType weaponPartType, float duration = 0.3f)
        {
            Transform weaponTransform = unit.transformData[weaponPartType];
            if (weaponTransform == null) return;

            StopAllCoroutines();
            StartCoroutine(LerpRotation(weaponTransform, originalRotation, duration));
        }

        // Smoothly interpolates weapon rotation
        private IEnumerator LerpRotation(Transform weaponTransform, Quaternion targetRotation, float duration)
        {
            Vector3 startEuler = weaponTransform.eulerAngles;
            Vector3 targetEuler = targetRotation.eulerAngles;
            float elapsed = 0f;

            float fixedY = startEuler.y;
            float fixedZ = startEuler.z;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float lerpedX = Mathf.LerpAngle(startEuler.x, targetEuler.x, t);
                weaponTransform.eulerAngles = new Vector3(lerpedX, fixedY, fixedZ);

                yield return null;
            }

            weaponTransform.eulerAngles = new Vector3(targetEuler.x, fixedY, fixedZ);
        }
    }
}