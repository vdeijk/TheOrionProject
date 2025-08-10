using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    public class LevelGeneratorService
    {
        private RaycastHit[] raycastHits = new RaycastHit[5];
        private static LevelGeneratorService _instance;

        public static LevelGeneratorService Instance => _instance ??= new LevelGeneratorService();
        private GridData Data => GridCoordinatorService.Instance.Data;

        // Generates grid objects and assigns terrain/feature types
        public void CreateGrid()
        {
            Data.Width = Mathf.RoundToInt(Data.MeshRenderer.bounds.size.x) / Mathf.RoundToInt(Data.CellSize);
            Data.Height = Mathf.RoundToInt(Data.MeshRenderer.bounds.size.z) / Mathf.RoundToInt(Data.CellSize);
            GridObject[,] gridObjectArray = new GridObject[Data.Width, Data.Height];

            Data.GetSquareType(GridSquareType.Forest).noiseMapTexture = new Texture2D(Data.Width, Data.Height);
            Data.GetSquareType(GridSquareType.Rough).noiseMapTexture = new Texture2D(Data.Width, Data.Height);

            for (int x = 0; x < Data.Width; x++)
            {
                for (int z = 0; z < Data.Height; z++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, z);
                    GridSquareSettings gridSquareInfo = GetSlopeHeightInfo(gridPosition, Data.LevelLayers);
                    gridSquareInfo.forestNoise = GetNoiseInfo(gridPosition, Data.GetSquareType(GridSquareType.Forest));
                    gridSquareInfo.roughNoise = GetNoiseInfo(gridPosition, Data.GetSquareType(GridSquareType.Rough));

                    gridSquareInfo.gridSquareType = SetSquareType(gridSquareInfo);

                    gridObjectArray[x, z] = new GridObject(gridSquareInfo);
                }
            }

            Data.GetSquareType(GridSquareType.Forest).noiseMapTexture.Apply();
            Data.GetSquareType(GridSquareType.Rough).noiseMapTexture.Apply();

            Data.Objects = gridObjectArray;
        }


        // Places objects (e.g. trees, rocks) on grid based on cell type
        public void PlaceObjects()
        {
            for (int x = 0; x < Data.Width; x++)
            {
                for (int z = 0; z < Data.Height; z++)
                {
                    GridObject gridObject = Data.Objects[x, z];

                    if (gridObject.gridSquareType == GridSquareType.Forest)
                    {
                        PlaceObjectOfType(new Vector2Int(x, z), Data.GetSquareType(GridSquareType.Forest));
                    }
                    else if (gridObject.gridSquareType == GridSquareType.Rough)
                    {
                        PlaceObjectOfType(new Vector2Int(x, z), Data.GetSquareType(GridSquareType.Rough));
                    }
                }
            }
        }

        // Places multiple objects of a given type at valid positions in a grid cell
        private void PlaceObjectOfType(Vector2Int gridPosition, GridSquareSO gridDataSquareType)
        {
            int totalSpawns = Random.Range(gridDataSquareType.minSpawns, gridDataSquareType.maxSpawns);
            List<Vector3> spawnPositions = new List<Vector3>();

            for (int i = 0; i < totalSpawns; i++)
            {
                Vector3 newPos = GetPosition(gridPosition, gridDataSquareType);
                bool enoughDist = CheckDistanceBetweenSpawns(spawnPositions, newPos, gridDataSquareType);

                int j = 0;

                // Try up to 12 times to find a valid spawn position
                while (!enoughDist && j < 12)
                {
                    newPos = GetPosition(gridPosition, gridDataSquareType);
                    enoughDist = CheckDistanceBetweenSpawns(spawnPositions, newPos, gridDataSquareType);
                    j++;
                }

                spawnPositions.Add(newPos);
            }

            foreach (Vector3 spawnPosition in spawnPositions)
            {
                int randomPrefab = Random.Range(0, gridDataSquareType.prefabTransforms.Length);
                int randomRotation = Random.Range(0, 360);
                Transform go = Data.Controller.InstantiateGridSquare(gridDataSquareType, spawnPosition);

                float randomScale = Random.Range(gridDataSquareType.minScale, gridDataSquareType.maxScale);
                go.localScale = new Vector3(randomScale, randomScale, randomScale);

                go.SetParent(Data.GridParent);
            }
        }

        // Ensures minimum distance between spawned objects
        private bool CheckDistanceBetweenSpawns(List<Vector3> spawnPositions, Vector3 newPos, GridSquareSO gridSquareData)
        {
            foreach (Vector3 spawnPosition in spawnPositions)
            {
                if (Vector3.Distance(newPos, spawnPosition) <= gridSquareData.minDistBetweenSpawns) return false;
            }
            return true;
        }

        // Gets a randomized position within a grid cell
        private Vector3 GetPosition(Vector2Int gridPosition, GridSquareSO gridSquareData)
        {
            float posX = gridPosition.x * Data.CellSize - Random.Range(-gridSquareData.XZRandomness, gridSquareData.XZRandomness);
            float posY = Data.Objects[gridPosition.x, gridPosition.y].height + Random.Range(gridSquareData.YRandomnessMin, gridSquareData.YRandomnessMax);
            float posZ = gridPosition.y * Data.CellSize - Random.Range(-gridSquareData.XZRandomness, gridSquareData.XZRandomness);

            return new Vector3(posX, posY, posZ);
        }

        // Uses raycast to determine slope and height for a grid cell
        private GridSquareSettings GetSlopeHeightInfo(Vector2Int gridPosition, LayerMask levelLayers)
        {
            GridSquareSettings gridSquareInfo = new GridSquareSettings()
            {
                slope = 0,
                height = 0,
                inaccessible = false,
                gridPosition = gridPosition
            };

            Vector3 startPos = new Vector3(gridPosition.x * Data.CellSize, 500, gridPosition.y * Data.CellSize);
            int hits = Physics.RaycastNonAlloc(startPos, Vector3.down, raycastHits, float.MaxValue, levelLayers);

            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    if (raycastHits[i].collider.gameObject.layer == LayerMask.NameToLayer("Default"))
                    {
                        gridSquareInfo.inaccessible = true;
                        return gridSquareInfo;
                    }
                    else
                    {
                        gridSquareInfo.slope = Vector3.Angle(raycastHits[i].normal, Vector3.up);
                        gridSquareInfo.height = raycastHits[i].point.y;
                    }
                }
            }
            return gridSquareInfo;
        }

        // Calculates Perlin noise for terrain features and updates noise map texture
        private float GetNoiseInfo(Vector2Int gridPosition, GridSquareSO gridSquareData)
        {
            float noiseX = (float)gridPosition.x / Data.Width * gridSquareData.noiseScale;
            float noiseZ = (float)gridPosition.y / Data.Height * gridSquareData.noiseScale;
            float noise = Mathf.PerlinNoise(noiseX, noiseZ);
            gridSquareData.noiseMapTexture.SetPixel(gridPosition.x, gridPosition.y, new Color(0, noise, 0));

            return noise;
        }

        // Determines the type of grid square based on terrain and noise data
        private GridSquareType SetSquareType(GridSquareSettings gridSquareInfo)
        {
            GridSquareSO forestSquareType = Data.GetSquareType(GridSquareType.Forest);
            GridSquareSO roughSquareType = Data.GetSquareType(GridSquareType.Rough);

            if (gridSquareInfo.inaccessible)
            {
                return GridSquareType.Inaccessible;
            }
            else if (gridSquareInfo.slope > 12)
            {
                return GridSquareType.Steep;
            }
            else if (gridSquareInfo.height > 6)
            {
                return GridSquareType.High;
            }
            else if (gridSquareInfo.forestNoise > gridSquareInfo.roughNoise && gridSquareInfo.forestNoise > forestSquareType.density)
            {
                return GridSquareType.Forest;
            }
            else if (gridSquareInfo.roughNoise > gridSquareInfo.forestNoise && gridSquareInfo.roughNoise > roughSquareType.density)
            {
                return GridSquareType.Rough;
            }
            return GridSquareType.Grass;
        }
    }
}


