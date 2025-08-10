using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy.Data
{
    [System.Serializable]
    public class GridData
    {
        public GridController Controller;
        public Transform GridParent;
        public LayerMask TerrainLayer;
        public LayerMask LevelLayers;
        public MeshRenderer MeshRenderer;
        public GridSquareSO GrassSquareType;
        public GridSquareSO ForestSquareType;
        public GridSquareSO RoughSquareType;
        public GridSquareSO HighSquareType;
        public GridSquareSO SteepSquareType;
        public GridSquareSO InaccessibleSquareType;
        public Transform GridSquarePrefab;
        public Transform GridSquareDebugPrefab;
        public List<GridMaterialType> materialTypes = new List<GridMaterialType>();

        public GridSingleSquareController[,] GridSystemArray { get; set; }
        public List<Vector2Int> ValidGridPositions { get; set; }
        public float CellSize { get; set; } = 10;
        public int Width { get; set; }
        public int Height { get; set; }
        public GridObject[,] Objects { get; set; }

        public void InitHeightAndWidth()
        {
            Width = Mathf.RoundToInt(MeshRenderer.bounds.size.x / CellSize);
            Height = Mathf.RoundToInt(MeshRenderer.bounds.size.z / CellSize);
        }

        public GridSquareSO GetSquareType(GridSquareType type)
        {
            return type switch
            {
                GridSquareType.Grass => GrassSquareType,
                GridSquareType.Forest => ForestSquareType,
                GridSquareType.Rough => RoughSquareType,
                GridSquareType.High => HighSquareType,
                GridSquareType.Steep => SteepSquareType,
                GridSquareType.Inaccessible => InaccessibleSquareType,
                _ => null
            };
        }
    }
}
