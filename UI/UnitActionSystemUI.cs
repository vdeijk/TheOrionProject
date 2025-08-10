using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] Transform actionButtonContainerTransform;
        [SerializeField] BaseButtonUI[] actionButtonUIList;

        private Dictionary<ActionType, ActionButtonUI> actionButtonDict = new Dictionary<ActionType, ActionButtonUI>();

        private void OnEnable()
        {
            ActionBaseService.OnActionStarted += BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted += BaseAction_OnActionCompleted;
            ActionBaseService.OnActionSelected += UnitActionSystem_OnSelectedActionChanged;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected += UnitSelectionSystem_OnUnitSelected;
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            ControlModeManager.OnGameModeChanged += ControlModeManager_OnControlModeChanged;
        }

        private void OnDestroy()
        {
            ActionBaseService.OnActionStarted -= BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted -= BaseAction_OnActionCompleted;
            ActionBaseService.OnActionSelected -= UnitActionSystem_OnSelectedActionChanged;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected -= UnitSelectionSystem_OnUnitSelected;
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            ControlModeManager.OnGameModeChanged -= ControlModeManager_OnControlModeChanged;
        }

        private void Start()
        {
            foreach (ActionButtonUI actionButton in actionButtonUIList)
            {
                actionButton.GetComponent<Button>().interactable = false;

                actionButtonDict[actionButton.actionType] = actionButton;
            }
        }

        private void ControlModeManager_OnControlModeChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UnitSelectionSystem_OnUnitSelected(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void BaseAction_OnActionStarted(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void BaseAction_OnActionCompleted(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UnitSelectionSystem_OnUnitDeselected(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
            bool isOverhead = CameraChangeMonobService.Instance.curCamera == Data.CameraType.Overhead;

            if (selectedUnit == null || !isOverhead) return;

            UnitActionController actionSystem = selectedUnit.Data.UnitMindTransform.GetComponent<UnitActionController>();

            HashSet<ActionType> availableActionTypes = new HashSet<ActionType>();

            foreach (ActionBaseController baseAction in actionSystem.baseActions)
            {
                availableActionTypes.Add(baseAction.Data.ActionType);

                if (actionButtonDict.TryGetValue(baseAction.Data.ActionType, out ActionButtonUI actionButtonUI))
                {
                    actionButtonUI.UpdateButton();
                }
            }

            foreach (var kvp in actionButtonDict)
            {
                ActionType actionType = kvp.Key;
                BaseButtonUI actionButtonUI = kvp.Value;

                actionButtonUI.gameObject.SetActive(availableActionTypes.Contains(actionType));
            }
        }
    }
}

