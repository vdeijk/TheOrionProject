using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles selection and execution of unit actions
    public class UnitActionSystem : Singleton<UnitActionSystem>
    {
        public BaseAction selectedAction { get; private set; }
        public List<GridPosition> validGridPositions { get; private set; }
        public bool inProgress { get; private set; } = false;

        [SerializeField] float AI_ACTION_DELAY;

        private MenuType menuType => MenuChangeService.Instance.curMenu;
        private bool isInInvalidMenu => menuType == MenuType.Repair || menuType == MenuType.Assemble;

        public event EventHandler OnSelectedActionChanged;
        public event EventHandler OnActionStarted;
        public event EventHandler OnActionCompleted;

        private void OnDisable()
        {
            // Unsubscribe from events
            UnitSelectService.OnUnitSelected -= UnitSelection_OnUnitSelected;
            BaseAction.OnActionCompleted -= BaseAction_OnActionCompleted;
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        private void Start()
        {
            // Subscribe to events
            UnitSelectService.OnUnitSelected += UnitSelection_OnUnitSelected;
            BaseAction.OnActionCompleted += BaseAction_OnActionCompleted;
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        // Sets the current selected action and updates valid grid positions
        public void SetSelectedAction(BaseAction baseAction)
        {
            selectedAction = baseAction;

            if (baseAction == null || isInInvalidMenu)
            {
                validGridPositions = new List<GridPosition>();
            }
            else
            {
                validGridPositions = UpdateGridPositions(baseAction.unit);
            }

            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }

        // Handles AI action selection and execution with delay
        public void HandleSelectedActionAI(EnemyAIAction bestEnemyAIAction, Unit unit)
        {
            if (unit != UnitSelectService.Instance.selectedUnit) UnitSelectService.Instance.SelectUnit(unit);
            StartCoroutine(TakeActionWithDelay(bestEnemyAIAction, unit));
        }

        // Attempts to execute the selected action for the player
        public bool TryHandleSelectedAction()
        {
            Unit selectedUnit = UnitSelectService.Instance.selectedUnit;
            if (selectedUnit == null || selectedAction == null) return false;
            UnitFaction unitFaction = selectedUnit.unitEntityTransform.GetComponent<UnitFaction>();
            if (unitFaction.IS_ENEMY) return false;
            bool inMission = ControlModeManager.Instance.gameControlType == GameControlType.Mission;
            if (!inMission) return false;
            if (selectedAction.isActive) return true;
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            AmmoSystem ammoSystem = UnitAmmoService.Instance.GetAmmoSystem(GetWeaponPartTypeFromAction());
            ActionSystem actionSystem = selectedUnit.unitMindTransform.GetComponent<ActionSystem>();
            bool isRangeValid = !selectedAction.hasMaxRange || validGridPositions.Contains(mouseGridPosition);
            if (isRangeValid && actionSystem.actionPoints > 0 && ammoSystem.HasAmmo(selectedAction))
            {
                inProgress = true;
                actionSystem.SpendActionPoints(selectedAction);
                selectedAction.TakeAction(mouseGridPosition);
                SFXController.Instance.PlaySFX(SFXType.ExecuteAction);
                OnActionStarted?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        // Returns valid grid positions for the selected action
        public List<GridPosition> UpdateGridPositions(Unit unit)
        {
            GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(unit.unitBodyTransform.position);
            return LevelGrid.Instance.GetValidPositions(unit, selectedAction.Range, selectedAction.GRID_POSITION_TYPE, unitGridPosition);
        }

        // Determines which weapon part type is used for the selected action
        private PartType GetWeaponPartTypeFromAction()
        {
            return selectedAction.actionType switch
            {
                ActionType.ShootPrimary => PartType.WeaponPrimary,
                ActionType.ShootSecondary => PartType.WeaponSecondary,
                _ => PartType.WeaponPrimary
            };
        }

        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            if (isInInvalidMenu)
            {
                SetSelectedAction(null);
            }
            else
            {
                Unit selectedUnit = UnitSelectService.Instance.selectedUnit;
                if (selectedUnit == null) return;
                SetSelectedAction(selectedUnit.unitEntityTransform.GetComponent<MoveAction>());
            }
        }

        // Coroutine for AI action delay
        private IEnumerator TakeActionWithDelay(EnemyAIAction bestEnemyAIAction, Unit unit)
        {
            yield return new WaitForSeconds(AI_ACTION_DELAY);
            SetSelectedAction(bestEnemyAIAction.baseAction);
            yield return new WaitForSeconds(AI_ACTION_DELAY);
            ActionSystem actionSystem = unit.unitMindTransform.GetComponent<ActionSystem>();
            actionSystem.SpendActionPoints(bestEnemyAIAction.baseAction);
            bestEnemyAIAction.baseAction.TakeAction(bestEnemyAIAction.gridPosition);
            SFXController.Instance.PlaySFX(SFXType.ExecuteAction);
            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }

        // Handles unit selection event
        private void UnitSelection_OnUnitSelected(object sender, EventArgs e)
        {
            Unit selectedUnit = UnitSelectService.Instance.selectedUnit;
            ActionSystem actionSystem = selectedUnit.unitMindTransform.GetComponent<ActionSystem>();
            SetSelectedAction(actionSystem.GetAction<MoveAction>());
        }

        // Handles action completion event
        private void BaseAction_OnActionCompleted(object sender, BaseAction.OnActionCompletedEventArgs e)
        {
            if (ControlModeManager.Instance.gameControlType == GameControlType.Outside) return;
            validGridPositions = UpdateGridPositions(e.baseAction.unit);
            SFXController.Instance.PlaySFX(SFXType.CompleteAction);
            inProgress = false;
            OnActionCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}

