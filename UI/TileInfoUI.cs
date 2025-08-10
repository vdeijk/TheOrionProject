using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class TileInfoUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI tileTypeField;
        [SerializeField] TextMeshProUGUI tileEffectField;

        private GridData gridData => GridCoordinatorService.Instance.Data;

        private void OnDisable()
        {
            UnitSelectService.OnUnitSelected -= UnitActionSystem_OnUnitSelected;
            ActionBaseService.OnActionCompleted -= TurnSystem_OnActionCompleted;
            UnitSelectService.OnGridSquareSelected -= GridSquareInfoController_OnGridSquareClicked;
        }

        private void Start()
        {
            UnitSelectService.OnUnitSelected += UnitActionSystem_OnUnitSelected;
            ActionBaseService.OnActionCompleted += TurnSystem_OnActionCompleted;
            UnitSelectService.OnGridSquareSelected += GridSquareInfoController_OnGridSquareClicked;
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
            if (CameraChangeMonobService.Instance.curCamera != Data.CameraType.Overhead) return;
            if (UnitSelectService.Instance.Data.SelectedGridPosition == null) return;

            Vector2Int gridPosition = (Vector2Int)UnitSelectService.Instance.Data.SelectedGridPosition;

            GridObject gridObject = gridData.Objects[gridPosition.x, gridPosition.y];
            tileTypeField.text = gridObject.gridSquareType.ToString().ToUpper();
            tileEffectField.text = gridData.GetSquareType(gridObject.gridSquareType).description;
        }
    }
}
