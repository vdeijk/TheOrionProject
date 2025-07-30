using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class GridObject
    {
        public List<Unit> unitList { get; private set; }
        public GridPosition gridPosition { get; private set; }
        public float slope { get; private set; }
        public float height { get; private set; }
        public float forestNoise { get; private set; }
        public float roughNoise { get; private set; }
        public GridSquareType gridSquareType { get; private set; }

        // Initializes grid cell data from GridSquareInfo
        public GridObject(GridSquareInfo gridSquareInfo)
        {
            unitList = new List<Unit>();

            gridPosition = gridSquareInfo.gridPosition;
            slope = gridSquareInfo.slope;
            height = gridSquareInfo.height;
            forestNoise = gridSquareInfo.forestNoise;
            roughNoise = gridSquareInfo.roughNoise;
            gridSquareType = gridSquareInfo.gridSquareType;
        }

        // Adds a unit to this grid cell if not already present
        public void AddUnit(Unit unit)
        {
            if (!unitList.Contains(unit)) unitList.Add(unit);
        }

        // Removes a unit from this grid cell
        public void RemoveUnit(Unit unit)
        {
            unitList.Remove(unit);
        }

        // Returns a string representation of the grid cell and its units
        public override string ToString()
        {
            string unitString = "";

            foreach (Unit unit in unitList)
            {
                unitString += unit;
            }

            return gridPosition.ToString() + "\n" + unitString;
        }

        // Returns the first unit in this grid cell, or null if empty
        public Unit GetUnit()
        {
            if (unitList.Count > 0)
            {
                return unitList[0];
            }
            return null;
        }
    }
}