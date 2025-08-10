using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Infra
{
    public class VignetteMonobService : SingletonBaseService<VignetteMonobService>
    {
        [Serializable]
        public class VignetteSettings
        {
            public float intensity = 0.25f;
            public float smoothness = 0.25f;
            public Color32 color = new Color32(0, 14, 47, 255);
        }

        [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] Volume postProcessVolume;
        [SerializeField] VignetteSettings actionCamSettings;
        [SerializeField] VignetteSettings salvageCamSettings;
        [SerializeField] VignetteSettings lowHealthSettings;
        [SerializeField] VignetteSettings defaulSettings;

        private Vignette vignette;
        private DurationData durationData => DurationData.Instance;

        protected override void Awake()
        {
            Instance = SetSingleton();
            postProcessVolume.profile.TryGet(out vignette);
        }

        private void OnEnable()
        {
            // Subscribe to relevant events for vignette updates
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
            UnitSelectService.OnUnitSelected += UnitSelectService_OnUnitSelected;
            UnitSelectService.OnUnitDeselected += UnitSelectService_OnUnitDeselected;
            ActionBaseService.OnActionCompleted += UnitActionSystem_OnActionCompleted;
            UnitCategoryService.OnUnitRemoved += UnitCategoryService_OnUnitRemoved;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
            UnitSelectService.OnUnitSelected -= UnitSelectService_OnUnitSelected;
            UnitSelectService.OnUnitDeselected -= UnitSelectService_OnUnitDeselected;
            ActionBaseService.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
            UnitCategoryService.OnUnitRemoved -= UnitCategoryService_OnUnitRemoved;
        }

        // Event handlers trigger vignette update
        private void UnitCategoryService_OnUnitRemoved(object sender, EventArgs e) { UpdateVignette(); }
        private void UnitSelectService_OnUnitDeselected(object sender, EventArgs e) { UpdateVignette(); }
        private void UnitActionSystem_OnActionCompleted(object sender, EventArgs e) { UpdateVignette(); }
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e) { UpdateVignette(); }
        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e) { UpdateVignette(); }
        private void UnitSelectService_OnUnitSelected(object sender, EventArgs e) { UpdateVignette(); }

        // Updates vignette settings based on camera, unit health, and game state
        private void UpdateVignette()
        {
            VignetteSettings target;
            switch (CameraChangeMonobService.Instance.curCamera)
            {
                case Data.CameraType.Action:
                    target = actionCamSettings;
                    break;
                case Data.CameraType.Salvage:
                    target = salvageCamSettings;
                    break;
                default:
                    UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
                    List<UnitSingleController> allies = UnitCategoryService.Instance.Data.Allies;
                    bool isInMission = ControlModeManager.Instance.gameControlType == GameControlType.Mission;
                    if (unit != null)
                    {
                        UnitHealthController healthSystem = unit.Data.UnitMindTransform.GetComponent<UnitHealthController>();
                        float percentageHealth = healthSystem.health / healthSystem.MaxHealth;
                        HeatSystemController heatSystem = unit.Data.UnitMindTransform.GetComponent<HeatSystemController>();
                        float percentageHeat = heatSystem.heat / heatSystem.MaxHeat;
                        if (percentageHealth <= .25f || percentageHeat >= .75f || allies.Count <= 1)
                        {
                            target = lowHealthSettings;
                        }
                        else
                        {
                            target = defaulSettings;
                        }
                    }
                    else if (isInMission && allies.Count <= 1)
                    {
                        target = lowHealthSettings;
                    }
                    else
                    {
                        target = defaulSettings;
                    }
                    break;
            }
            StopAllCoroutines();
            StartCoroutine(LerpVignette(target));
        }

        // Smoothly transitions vignette parameters to target values
        private IEnumerator LerpVignette(VignetteSettings target)
        {
            float startIntensity = vignette.intensity.value;
            float startSmoothness = vignette.smoothness.value;
            Color32 startColor = vignette.color.value;
            float elapsedTime = 0f;
            while (elapsedTime < durationData.CameraBlendDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = elapsedTime / durationData.CameraBlendDuration;
                float curveT = transitionCurve.Evaluate(t);
                vignette.intensity.value = Mathf.Lerp(startIntensity, target.intensity, curveT);
                vignette.smoothness.value = Mathf.Lerp(startSmoothness, target.smoothness, curveT);
                vignette.color.value = Color32.Lerp(startColor, target.color, curveT);
                yield return null;
            }
            vignette.intensity.value = target.intensity;
            vignette.smoothness.value = target.smoothness;
            vignette.color.value = target.color;
        }
    }
}
