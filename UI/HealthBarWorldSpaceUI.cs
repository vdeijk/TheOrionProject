using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class HealthBarWorldSpaceUI : MonoBehaviour
    {
        [SerializeField] UnitSingleController unit;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] UnitHealthController healthSystem;
        [SerializeField] Transform worldUITransform;
        [SerializeField] public Image healthBar;
        [SerializeField] public Image armorBar;
        [SerializeField] public Image shieldBar;

        private DurationData durationData => DurationData.Instance;

        private void OnEnable()
        {
            UnitHealthController.OnDamaged += HealthSystem_OnDamaged;
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
            //ShootAction.OnStartedShooting += ShootAction_OnStartedShooting;
            //ShootAction.OnStoppedShooting += ShootAction_OnStoppedShooting;
        }

        private void OnDisable()
        {
            UnitHealthController.OnDamaged -= HealthSystem_OnDamaged;
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
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

            StartCoroutine(BarAnimationMonobService.Instance.AnimateBarRoutine(healthBar,
                healthSystem.GetHealthNormalized(), healthSystem.MaxHealth));
            StartCoroutine(BarAnimationMonobService.Instance.AnimateBarRoutine(armorBar,
                healthSystem.GetArmorNormalized(), healthSystem.MaxArmor));
            StartCoroutine(BarAnimationMonobService.Instance.AnimateBarRoutine(shieldBar,
                healthSystem.GetShieldNormalized(), healthSystem.MaxShield));
        }

        private void ShootAction_OnStartedShooting(object sender, EventArgs e)
        {
            StopAllCoroutines();

            if (unit == UnitSelectService.Instance.Data.SelectedTarget)
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
            yield return new WaitForSecondsRealtime(durationData.CameraBlendDuration);

            canvasGroup.alpha = 1;
        }

    }
}
