using UnityEngine;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy
{
    public class UnitSelectData
    {
        public Vector2Int? SelectedGridPosition { get; set; }
        public UnitSingleController SelectedUnit { get; set; }
        public UnitSingleController PreviousSelectedUnit { get; set; }
        public UnitSingleController SelectedTarget { get; set; }

        public UnitSelectData(UnitSingleController selectedUnit, UnitSingleController previousSelectedUnit, UnitSingleController selectedTarget)
        {
            SelectedUnit = selectedUnit;
            PreviousSelectedUnit = previousSelectedUnit;
            SelectedTarget = selectedTarget;
        }
    }
}