using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class LevelGenerator : Singleton<LevelGenerator>
    {
        [SerializeField] bool createLevelOnPlay;
        [SerializeField] LevelGrid levelGrid;
        [SerializeField] GridData gridData;

        private RaycastHit[] raycastHits = new RaycastHit[5];

        protected override void Awake()
        {
            Instance = SetSingleton();

            // Optionally generate level at play start
            if (createLevelOnPlay)
            {
                DeleteObjects();
                gridData.Initialize();
                CreateGrid();
                PlaceObjects();
            }
        }

        // Generates grid objects and assigns terrain/feature types
        public void CreateGrid()
        {
            gridData.gridWidth = Mathf.RoundToInt(levelGrid.levelMeshRenderer.bounds.size.x) / Mathf.RoundToInt(gridData.cellSize);
            gridData.gridHeight = Mathf.RoundToInt(levelGrid.levelMeshRenderer.bounds.size.z) / Mathf.RoundToInt(gridData.cellSize);
            GridObject[,] gridObjectArray = new GridObject[gridData.gridWidth, gridData.gridHeight];

            gridData.squareTypes[GridSquareType.Forest].noiseMapTexture = new Texture2D(gridData.gridWidth, gridData.gridHeight);
            gridData.squareTypes[GridSquareType.Rough].noiseMapTexture = new Texture2D(gridData.gridWidth, gridData.gridHeight);

            for (int x = 0; x < gridData.gridWidth; x++)
            {
                for (int z = 0; z < gridData.gridHeight; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    GridSquareInfo gridSquareInfo = GetSlopeHeightInfo(gridPosition, levelGrid.levelLayers);
                    gridSquareInfo.forestNoise = GetNoiseInfo(gridPosition, gridData.squareTypes[GridSquareType.Forest]);
                    gridSquareInfo.roughNoise = GetNoiseInfo(gridPosition, gridData.squareTypes[GridSquareType.Rough]);

                    gridSquareInfo.gridSquareType = SetSquareType(gridSquareInfo);

                    gridObjectArray[x, z] = new GridObject(gridSquareInfo);
                }
            }

            gridData.squareTypes[GridSquareType.Forest].noiseMapTexture.Apply();
            gridData.squareTypes[GridSquareType.Rough].noiseMapTexture.Apply();

            gridData.gridObjectArray = gridObjectArray;
        }

        // Removes all child objects from the grid parent
        public void DeleteObjects()
        {
            for (int i = levelGrid.parent.childCount; i > 0; --i)
            {
                DestroyImmediate(levelGrid.parent.GetChild(0).gameObject);
            }
        }

        // Places objects (e.g. trees, rocks) on grid based on cell type
        public void PlaceObjects()
        {
            for (int x = 0; x < gridData.gridWidth; x++)
            {
                for (int z = 0; z < gridData.gridHeight; z++)
                {
                    GridObject gridObject = gridData.gridObjectArray[x, z];

                    if (gridObject.gridSquareType == GridSquareType.Forest)
                    {
                        PlaceObjectOfType(new GridPosition(x, z), gridData.squareTypes[GridSquareType.Forest]);
                    }
                    else if (gridObject.gridSquareType == GridSquareType.Rough)
                    {
                        PlaceObjectOfType(new GridPosition(x, z), gridData.squareTypes[GridSquareType.Rough]);
                    }
                }
            }
        }

        // Places multiple objects of a given type at valid positions in a grid cell
        private void PlaceObjectOfType(GridPosition gridPosition, GridSquareData gridDataSquareType)
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
                Transform go = Instantiate(gridDataSquareType.prefabTransforms[randomPrefab], spawnPosition, Quaternion.Euler(new Vector3(0, randomRotation, 0)));

                float randomScale = Random.Range(gridDataSquareType.minScale, gridDataSquareType.maxScale);
                go.localScale = new Vector3(randomScale, randomScale, randomScale);

                go.SetParent(levelGrid.parent);
            }
        }

        // Ensures minimum distance between spawned objects
        private bool CheckDistanceBetweenSpawns(List<Vector3> spawnPositions, Vector3 newPos, GridSquareData gridSquareData)
        {
            foreach (Vector3 spawnPosition in spawnPositions)
            {
                if (Vector3.Distance(newPos, spawnPosition) <= gridSquareData.minDistBetweenSpawns) return false;
            }
            return true;
        }

        // Gets a randomized position within a grid cell
        private Vector3 GetPosition(GridPosition gridPosition, GridSquareData gridSquareData)
        {
            float posX = gridPosition.x * gridData.cellSize - Random.Range(-gridSquareData.XZRandomness, gridSquareData.XZRandomness);
            float posY = gridData.gridObjectArray[gridPosition.x, gridPosition.z].height + Random.Range(gridSquareData.YRandomnessMin, gridSquareData.YRandomnessMax);
            float posZ = gridPosition.z * gridData.cellSize - Random.Range(-gridSquareData.XZRandomness, gridSquareData.XZRandomness);
            return new Vector3(posX, posY, posZ);
        }

        // Uses raycast to determine slope and height for a grid cell
        private GridSquareInfo GetSlopeHeightInfo(GridPosition gridPosition, LayerMask levelLayers)
        {
            GridSquareInfo gridSquareInfo = new GridSquareInfo()
            {
                slope = 0,
                height = 0,
                inaccessible = false,
                gridPosition = gridPosition
            };

            Vector3 startPos = new Vector3(gridPosition.x * gridData.cellSize, 500, gridPosition.z * gridData.cellSize);
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
        private float GetNoiseInfo(GridPosition gridPosition, GridSquareData gridSquareData)
        {
            float noiseX = (float)gridPosition.x / gridData.gridWidth * gridSquareData.noiseScale;
            float noiseZ = (float)gridPosition.z / gridData.gridHeight * gridSquareData.noiseScale;
            float noise = Mathf.PerlinNoise(noiseX, noiseZ);
            gridSquareData.noiseMapTexture.SetPixel(gridPosition.x, gridPosition.z, new Color(0, noise, 0));
            return noise;
        }

        // Determines the type of grid square based on terrain and noise data
        private GridSquareType SetSquareType(GridSquareInfo gridSquareInfo)
        {
            GridSquareData forestSquareType = gridData.squareTypes[GridSquareType.Forest];
            GridSquareData roughSquareType = gridData.squareTypes[GridSquareType.Rough];

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


