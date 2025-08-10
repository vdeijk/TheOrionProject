using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class UnitController : MonoBehaviour
    {
        [field: SerializeField] public UnitMovementData MovementData { get; private set; }
        [field: SerializeField] public SpawnEnemyData SpawnEnemyData { get; private set; }
        [field: SerializeField] public SpawnAllyData SpawnAllyData { get; private set; }
        [field: SerializeField] public UnitMaterialData MaterialData { get; private set; }

        [SerializeField] DurationData durationData;

        private void OnEnable()
        {
            UnitHealthController.OnunitDead += HealthSystem_OnUnitDead;
            HeatSystemController.OnunitDead += HeatSystem_OnUnitDead;
            ActionBaseService.OnActionCompleted += UnitActionSystem_OnActionCompleted;
            PhaseManager.OnPhaseChanged += ControlModeManager_OnPhaseChanged;
            UnitCategoryService.OnUnitAdded += UnitManager_OnUnitAdded;
        }

        private void OnDestroy()
        {
            UnitHealthController.OnunitDead -= HealthSystem_OnUnitDead;
            HeatSystemController.OnunitDead -= HeatSystem_OnUnitDead;
            ActionBaseService.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
            PhaseManager.OnPhaseChanged -= ControlModeManager_OnPhaseChanged;
            UnitCategoryService.OnUnitAdded -= UnitManager_OnUnitAdded;
        }

        private void Start()
        {
            UnitCategoryService.Instance.Init(new UnitCategoryData());
            UnitMovementService.Instance.Init(MovementData);
            SpawnEnemyService.Instance.Init(SpawnEnemyData);
            SpawnAllyService.Instance.Init(SpawnAllyData);
            UnitMaterialService.Instance.Init(MaterialData);
        }

        private void HealthSystem_OnUnitDead(object sender, UnitHealthController.OnUnitDeadEventArgs e)
        {
            UnitCategoryService.Instance.RemoveUnit(e.unit);
            UnitSelectService.Instance.TrySelectFirstAlly();
        }

        private void HeatSystem_OnUnitDead(object sender, HeatSystemController.OnUnitDeadEventArgs e)
        {
            UnitCategoryService.Instance.RemoveUnit(e.unit);
            UnitSelectService.Instance.TrySelectFirstAlly();
        }

        // Coroutine for unit spawn material fade-in
        private void UnitManager_OnUnitAdded(object sender, UnitCategoryService.OnUnitAddedArgs e)
        {
            bool IS_ENEMY = e.unit.Data.UnitEntityTransform.GetComponent<UnitFactionController>().IS_ENEMY;
            SpawnBaseData data = IS_ENEMY ? SpawnEnemyData : SpawnAllyData;
            UnitMonobService.Instance.StartCoroutine(UnitMonobService.Instance.SpawnUnit(e.unit, data));
        }

        // Removes highlight when action points are depleted
        private void UnitActionSystem_OnActionCompleted(object sender, EventArgs e)
        {
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            if (unit != null && unit.Data.UnitMindTransform.GetComponent<UnitActionController>().actionPoints <= 0)
            {
                UnitMaterialService.Instance.SetMesh(unit, MaterialData.LightLayerUintWithoutHighlight);
            }
        }

        // Adds highlight mesh to all units on phase change
        private void ControlModeManager_OnPhaseChanged(object sender, EventArgs e)
        {
            UnitSingleController[] allUnits = UnitCategoryService.Instance.Data.All.ToArray();
            foreach (UnitSingleController unit in allUnits)
            {
                UnitMaterialService.Instance.SetMesh(unit, MaterialData.LightLayerUintWithHighlight);
            }
        }
    }
}