using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] Transform actionButtonContainerTransform;
        [SerializeField] ActionButtonUI[] actionButtonUIList;

        private Dictionary<ActionType, ActionButtonUI> actionButtonDict = new Dictionary<ActionType, ActionButtonUI>();

        private void OnEnable()
        {
            BaseAction.OnActionStarted += BaseAction_OnActionStarted;
            BaseAction.OnActionCompleted += BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected += UnitSelectionSystem_OnUnitSelected;
            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            ControlModeManager.OnGameModeChanged += ControlModeManager_OnControlModeChanged;
        }

        private void OnDestroy()
        {
            BaseAction.OnActionStarted -= BaseAction_OnActionStarted;
            BaseAction.OnActionCompleted -= BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected -= UnitSelectionSystem_OnUnitSelected;
            UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
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
            Unit selectedUnit = UnitSelectService.Instance.selectedUnit;
            bool isOverhead = CameraChangeService.Instance.curCamera == CameraType.Overhead;

            if (selectedUnit == null || !isOverhead) return;

            ActionSystem actionSystem = selectedUnit.unitMindTransform.GetComponent<ActionSystem>();

            HashSet<ActionType> availableActionTypes = new HashSet<ActionType>();

            foreach (BaseAction baseAction in actionSystem.baseActions)
            {
                ActionType actionType = baseAction.actionType;
                availableActionTypes.Add(actionType);

                if (actionButtonDict.TryGetValue(actionType, out ActionButtonUI actionButtonUI))
                {
                    actionButtonUI.SetBaseAction(baseAction);
                    actionButtonUI.UpdateSelectedVisual();
                }
            }

            foreach (var kvp in actionButtonDict)
            {
                ActionType actionType = kvp.Key;
                ActionButtonUI actionButtonUI = kvp.Value;

                actionButtonUI.gameObject.SetActive(availableActionTypes.Contains(actionType));
            }
        }
    }
}

