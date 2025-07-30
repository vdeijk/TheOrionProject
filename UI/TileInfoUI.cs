using System;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class TileInfoUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI tileTypeField;
        [SerializeField] TextMeshProUGUI tileEffectField;
        [SerializeField] GridData gridData;

        private void OnDisable()
        {
            UnitSelectService.OnUnitSelected -= UnitActionSystem_OnUnitSelected;
            UnitActionSystem.Instance.OnActionCompleted -= TurnSystem_OnActionCompleted;
            GridSquareInfoController.OnGridSquareClicked -= GridSquareInfoController_OnGridSquareClicked;
        }

        private void Start()
        {
            UnitSelectService.OnUnitSelected += UnitActionSystem_OnUnitSelected;
            UnitActionSystem.Instance.OnActionCompleted += TurnSystem_OnActionCompleted;
            GridSquareInfoController.OnGridSquareClicked += GridSquareInfoController_OnGridSquareClicked;
        }

        private void UnitActionSystem_OnUnitSelected(object sender, EventArgs e)
        {
            UpdateTileInfo();
        }

        private void TurnSystem_OnActionCompleted(object sender, EventArgs e)
        {
            UpdateTileInfo();
        }

        private void GridSquareInfoController_OnGridSquareClicked(object sender, EventArgs e)
        {
            UpdateTileInfo();
        }

        private void UpdateTileInfo()
        {
            if (CameraChangeService.Instance.curCamera != CameraType.Overhead) return;
            if (GridSquareInfoController.Instance.selectedGridPosition == null) return;
            GridPosition gridPosition = (GridPosition)GridSquareInfoController.Instance.selectedGridPosition;

            GridObject gridObject = gridData.gridObjectArray[gridPosition.x, gridPosition.z];
            tileTypeField.text = gridObject.gridSquareType.ToString().ToUpper();
            tileEffectField.text = gridData.squareTypes[gridObject.gridSquareType].description;
        }
    }
}
