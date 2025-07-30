using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class GridData : ScriptableObject
    {
        public float cellSize = 10;
        public int gridWidth;
        public int gridHeight;
        public GridObject[,] gridObjectArray;

        [SerializeField] GridSquareData grassSquareType;
        [SerializeField] GridSquareData forestSquareType;
        [SerializeField] GridSquareData roughSquareType;
        [SerializeField] GridSquareData highSquareType;
        [SerializeField] GridSquareData steepSquareType;
        [SerializeField] GridSquareData inaccessibleSquareType;

        public Dictionary<GridSquareType, GridSquareData> squareTypes = new Dictionary<GridSquareType, GridSquareData>();

        public void Initialize()
        {
            if (squareTypes.Count <= 0)
            {
                squareTypes.Add(GridSquareType.Grass, grassSquareType);
                squareTypes.Add(GridSquareType.Forest, forestSquareType);
                squareTypes.Add(GridSquareType.Rough, roughSquareType);
                squareTypes.Add(GridSquareType.High, highSquareType);
                squareTypes.Add(GridSquareType.Steep, steepSquareType);
                squareTypes.Add(GridSquareType.Inaccessible, inaccessibleSquareType);
            }
        }
    }
}
