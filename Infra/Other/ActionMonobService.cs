using System;
using System.Collections;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.UI;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Infra
{
    [DefaultExecutionOrder(-100)]
    /// <summary>
    /// Abstract base class for all unit actions (move, shoot, pass, etc.).
    /// </summary>
    public class ActionMonobService : SingletonBaseService<ActionMonobService>
    {
        private DurationData durationData => DurationData.Instance;

        private void OnEnable()
        {
            AssemblyPartInfo.OnPartAssembled += AssemblyPartInfo_OnPartAssembled;
        }

        private void OnDisable()
        {
            AssemblyPartInfo.OnPartAssembled -= AssemblyPartInfo_OnPartAssembled;
        }

        // Handles AI action selection and execution with delay
        public void HandleSelectedActionAI(UnitAIData bestAction, UnitSingleController unit)
        {
            if (unit != UnitSelectService.Instance.Data.SelectedUnit)
            {
                SelectModeManager.Instance.UnitSelected(unit);
                ActionCoordinatorService.Instance.SelectPassAction();
                ActionPassService.Instance.TakeAction(bestAction.GridPosition);
            }

            StartCoroutine(TakeActionWithDelay(bestAction, unit));
        }

        private void AssemblyPartInfo_OnPartAssembled(object sender, EventArgs e)
        {
            ActionCoordinatorService.Instance.SelectMoveAction();
        }

        // Coroutine for AI action delay
        private IEnumerator TakeActionWithDelay(UnitAIData bestEnemyAIAction, UnitSingleController unit)
        {
            yield return new WaitForSeconds(durationData.ActionDuration);
        }
    }
}
