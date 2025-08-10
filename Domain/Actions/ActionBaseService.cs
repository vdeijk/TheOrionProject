using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    /// <summary>
    /// Non-generic base class for all action services.
    /// Provides shared events and interface for action selection.
    /// </summary>
    public abstract class ActionBaseService
    {
        public static event EventHandler OnActionSelected;
        public static event EventHandler OnActionDeselected;
        public static event EventHandler OnActionStarted;
        public static event EventHandler OnActionCompleted;

        protected void InvokeActionSelected(object sender)
        {
            OnActionSelected?.Invoke(sender, EventArgs.Empty);
        }

        protected void InvokeActionDeselected(object sender)
        {
            OnActionDeselected?.Invoke(sender, EventArgs.Empty);
        }

        protected void InvokeActionStarted(object sender)
        {
            OnActionStarted?.Invoke(sender, EventArgs.Empty);
        }

        protected void InvokeActionCompleted(object sender)
        {
            OnActionCompleted?.Invoke(sender, EventArgs.Empty);
        }
    }

    [DefaultExecutionOrder(100)]
    /// <summary>
    /// Abstract base class for all unit actions (move, shoot, pass, etc.).
    /// </summary>
    public abstract class BaseActionService<TData> : ActionBaseService where TData : BaseActionData
    {
        public virtual TData Data { get; protected set; }
        protected Vector3 targetDirection { get; set; }
        protected Vector3 curForward { get; set; }

        protected UnitSingleController unit => Data.Unit;
        protected int range => Data.partSO.Range;
        protected DurationData durationData => DurationData.Instance;
        protected Rigidbody rb => Data.Rb;
        protected UnitMovementController unitMovement => Data.UnitMovement;

        public abstract void TakeAction(Vector2Int gridPosition);

        protected abstract UnitAIData GetEnemyAction(Vector2Int gridPosition);

        // Attempts to execute the selected action for the player
        public bool TryStartAction()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
            if (selectedUnit == null) return false;

            UnitFactionController unitFaction = selectedUnit.Data.UnitEntityTransform.GetComponent<UnitFactionController>();
            bool inMission = ControlModeManager.Instance.gameControlType == GameControlType.Mission;
            if (Data != ActionCoordinatorService.Instance.SelectedActionData || unitFaction.IS_ENEMY || !inMission) return false;

            if (Data.IsActive) return true;

            Vector2Int mouseGridPosition = GridUtilityService.Instance.GetGridPosition(MouseWorldService.Instance.GetPosition());
            UnitAmmoController ammoSystem = UnitAmmoService.Instance.GetAmmoSystem(Data.GetWeaponPartType());
            UnitActionController actionSystem = selectedUnit.Data.UnitMindTransform.GetComponent<UnitActionController>();
            GridData gridData = GridCoordinatorService.Instance.Data;
            bool isRangeValid = gridData.ValidGridPositions.Contains(mouseGridPosition);
            if (isRangeValid && actionSystem.actionPoints > 0 && ammoSystem.HasAmmo(Data))
            {
                UnitAIData unitAIData = new UnitAIData
                {
                    BaseActionData = Data,
                    GridPosition = mouseGridPosition,
                    ActionValue = 0
                };

                StartAction(unitAIData);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the best EnemyAIAction based on action value for all valid grid positions.
        /// </summary>
        public virtual UnitAIData GetBestEnemyAction()
        {
            Vector2Int unitGridPosition = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);

            List<UnitAIData> enemyAIActions = new List<UnitAIData>();
            List<Vector2Int> validGridPositions = GridActionService.Instance.GetPositions(Data);

            foreach (Vector2Int gridPosition in validGridPositions)
                enemyAIActions.Add(GetEnemyAction(gridPosition));

            if (enemyAIActions.Count > 0)
            {
                enemyAIActions.Sort((UnitAIData a, UnitAIData b) => b.ActionValue - a.ActionValue);
                return enemyAIActions[0];
            }

            return null;
        }

        /// <summary>
        /// Marks the action as started and invokes the OnActionStarted event.
        /// </summary>
        public void StartAction(UnitAIData unitAIData)
        {
            Data.IsActive = true;

            UnitActionController actionSystem = unit.Data.UnitMindTransform.GetComponent<UnitActionController>();
            actionSystem.SpendActionPoints(Data);

            TakeAction(unitAIData.GridPosition);

            SFXMonobService.Instance.PlaySFX(SFXType.ExecuteAction);

            InvokeActionStarted(this);
        }

        /// <summary>
        /// Marks the action as completed and invokes the OnActionCompleted event.
        /// </summary>
        public void CompleteAction()
        {
            if (ControlModeManager.Instance.gameControlType == GameControlType.Outside) return;

            SFXMonobService.Instance.PlaySFX(SFXType.CompleteAction);
            Data.IsActive = false;

            InvokeActionCompleted(this);
        }
    }
}

