using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class ObjectiveUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI APRemainingText;
        [SerializeField] TextMeshProUGUI levelAndTurnText;

        private void OnEnable()
        {
            ActionBaseService.OnActionCompleted += UnitActionSystem_OnActionCompleted;
            PhaseManager.OnPhaseChanged += PhaseManager_OnPhaseChanged;
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            ActionBaseService.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
            PhaseManager.OnPhaseChanged -= PhaseManager_OnPhaseChanged;
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        private void Start()
        {
            UpdateUI();
        }

        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UnitActionSystem_OnActionCompleted(object sender, EventArgs e)
        {
            APRemainingText.text = CalculateRemainingAP().ToString() + "/" + CalculateBaseAP().ToString() + " AP";
        }

        private void PhaseManager_OnPhaseChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (!PhaseManager.Instance.isPlayerPhase) return;

            APRemainingText.text = CalculateRemainingAP().ToString() + "/" + CalculateBaseAP().ToString() + " AP";

            if (ControlModeManager.Instance.gameControlType == GameControlType.Mission)
            {
                int level = LevelManager.Instance.level;
                int turnNumber = PhaseManager.Instance.turnNumber;
                levelAndTurnText.text = "Battle " + level + ", Turn " + turnNumber;
            }
            else
            {
                levelAndTurnText.text = "Preview";
            }
        }

        private int CalculateRemainingAP()
        {
            int i = 0;
            foreach (UnitSingleController unit in UnitCategoryService.Instance.Data.Allies)
            {
                i += unit.Data.UnitMindTransform.GetComponent<UnitActionController>().actionPoints;
            }

            return i;
        }

        private int CalculateBaseAP()
        {
            int i = 0;

            foreach (UnitSingleController unit in UnitCategoryService.Instance.Data.Allies)
            {
                i += unit.Data.UnitMindTransform.GetComponent<UnitActionController>().MAX_ACTION_POINTS;
            }

            return i;
        }
    }
}

