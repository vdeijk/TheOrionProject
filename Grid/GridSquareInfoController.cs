using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class GridSquareInfoController : Singleton<GridSquareInfoController>
    {
        public GridPosition? selectedGridPosition;

        public static event EventHandler OnGridSquareClicked;
        public static event EventHandler OnClickedOutsideGrid;

        // Selects a grid square based on world position and notifies listeners
        public void SelectGridSquare(Vector3 pos)
        {
            selectedGridPosition = LevelGrid.Instance.GetGridPosition(pos);
            OnGridSquareClicked?.Invoke(this, EventArgs.Empty);
        }

        // Handles right mouse input for deselecting grid squares
        public void HandleInputMouseRight(bool rightMouseButtonInput)
        {
            bool inProgress = UnitActionSystem.Instance.inProgress;
            if (!rightMouseButtonInput || selectedGridPosition == null || inProgress) return;

            SFXController.Instance.PlaySFX(SFXType.DeselectUnit);
            DeselectGridSquare();
        }

        // Attempts to select a grid square based on mouse position
        public bool TryHandleGridSquareSelection()
        {
            Vector3 pos = MouseWorld.GetPosition();

            if (pos == Vector3.zero)
            {
                SFXController.Instance.PlaySFX(SFXType.DeselectUnit);
                DeselectGridSquare();
                return false;
            }
            else
            {
                GridPosition? gridPosition = LevelGrid.Instance.GetGridPosition(pos);

                // If already selected or invalid, treat as handled
                if (gridPosition == null || gridPosition == selectedGridPosition) return true;

                SFXController.Instance.PlaySFX(SFXType.SelectTile);
                selectedGridPosition = LevelGrid.Instance.GetGridPosition(pos);
                OnGridSquareClicked?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }

        // Deselects the current grid square and notifies listeners
        public void DeselectGridSquare()
        {
            selectedGridPosition = null;
            OnClickedOutsideGrid?.Invoke(this, EventArgs.Empty);
        }
    }
}