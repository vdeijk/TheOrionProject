using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TurnBasedStrategy
{
    public class HealthBarWorldSpace : MonoBehaviour
    {
        [SerializeField] Unit unit;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] HealthSystem healthSystem;
        [SerializeField] Transform worldUITransform;
        [SerializeField] public Image healthBar;
        [SerializeField] public Image armorBar;
        [SerializeField] public Image shieldBar;
        [SerializeField] GameDurations gameDurations;

        private void OnEnable()
        {
            HealthSystem.OnDamaged += HealthSystem_OnDamaged;
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
            //ShootAction.OnStartedShooting += ShootAction_OnStartedShooting;
            //ShootAction.OnStoppedShooting += ShootAction_OnStoppedShooting;
        }

        private void OnDisable()
        {
            HealthSystem.OnDamaged -= HealthSystem_OnDamaged;
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
            //ShootAction.OnStartedShooting -= ShootAction_OnStartedShooting;
            //ShootAction.OnStoppedShooting -= ShootAction_OnStoppedShooting;
        }

        private void Start()
        {
            InitBar();
        }

        private void LateUpdate()
        {
            worldUITransform.LookAt(worldUITransform.position + Camera.main.transform.forward);
        }

        public void UpdateBar()
        {
            StopAllCoroutines();

            StartCoroutine(BarAnimationService.Instance.AnimateBarRoutine(healthBar,
                healthSystem.GetHealthNormalized(), healthSystem.MaxHealth));
            StartCoroutine(BarAnimationService.Instance.AnimateBarRoutine(armorBar,
                healthSystem.GetArmorNormalized(), healthSystem.MaxArmor));
            StartCoroutine(BarAnimationService.Instance.AnimateBarRoutine(shieldBar,
                healthSystem.GetShieldNormalized(), healthSystem.MaxShield));
        }

        private void ShootAction_OnStartedShooting(object sender, EventArgs e)
        {
            StopAllCoroutines();

            if (unit == UnitSelectService.Instance.selectedTarget)
            {
                canvasGroup.alpha = 1;
            }
            else
            {
                canvasGroup.alpha = 0;
            }
        }

        private void ShootAction_OnStoppedShooting(object sender, EventArgs e)
        {
            StopAllCoroutines();

            StartCoroutine(SetWithDelay());
        }

        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            UpdateBar();
        }

        private void HealthSystem_OnDamaged(object sender, EventArgs e)
        {
            UpdateBar();
        }

        private void InitBar()
        {

        }

        private IEnumerator SetWithDelay()
        {
            yield return new WaitForSecondsRealtime(gameDurations.cameraBlendDuration);

            canvasGroup.alpha = 1;
        }

    }
}
