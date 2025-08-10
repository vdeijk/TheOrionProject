using UnityEngine;
using System.Collections.Generic;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Domain
{
    public class GridCoordinatorService
    {
        private static GridCoordinatorService _instance;

        public GridData Data { get; private set; }

        public static GridCoordinatorService Instance => _instance ??= new GridCoordinatorService();

        public void Init(GridData data)
        {
            Data = data;

            // Initialize grid visuals array based on grid dimensions
            Data.GridSystemArray = new GridSingleSquareController[
                 Data.Width,
                 Data.Height
             ];

            // Instantiate grid visuals for each cell
            for (int x = 0; x < Data.Width; x++)
            {
                for (int z = 0; z < Data.Height; z++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, z);

                    Vector3 worldPos = GridUtilityService.Instance.GetWorldPosition(gridPosition);
                    Vector3 newWorldPos = new Vector3(worldPos.x, GridUtilityService.Instance.GetTerrainY(worldPos.x, worldPos.z), worldPos.z);
                    Transform gridSystemVisualSingleTransform = Data.Controller.InstantiateGridSquare(newWorldPos);

                    Data.GridSystemArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSingleSquareController>();
                }
            }

            GridUXService.Instance.UpdateGrid();
        }
    }
}
