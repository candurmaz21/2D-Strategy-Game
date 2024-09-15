using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Build;

namespace Grids
{
    public class GridController : MonoBehaviour
    {

        //Essentials.
        public static event Action<BuildingType.BuildingTypes> OnBuildingTypeChanged;
        private IGrid grid;
        private PathfindingGrid pathfindingGrid;
        //Const
        private const int GRID_WIDTH = 24;
        private const int GRID_HEIGHT = 25;
        private const float CELL_SIZE = 0.32f;
        //Serializable Fields.
        [Header("Square Infos")]
        [Tooltip("32x32 square icon for place.")]
        [SerializeField] GameObject squareIcon;
        [Tooltip("Square default position.")]
        [SerializeField] Vector3 squareStartPos;
        [Tooltip("Building Infos.")]
        private BuildingData[] buildings;
        //Helpers
        private Vector3[] currentMidPoint = new Vector3[20];
        private Vector3 activeMidPoint;
        private GameObject squareIconSpawned;
        float buildingValue;
        int redCount;
        Color buildingColor;

        private BuildingBehavior currentBuildingBehavior;
        private List<GameObject> squaresSpawned = new List<GameObject>();
        private Vector3[,] gridWorldPositions;

        //Small optimization.
        private Vector3 cachedWorldPosOffset = Vector3.zero;
        private Vector3 cachedNewVector = Vector3.zero;
        private Vector3 cachedWorldPos = Vector3.zero;
        Vector2Int[] neighborOffsets = new Vector2Int[]
        {
            new Vector2Int(1, 0),   // Right
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1)   // Down
        };

        //Start
        private void Start()
        {
            grid = new Grid(GRID_WIDTH, GRID_HEIGHT, CELL_SIZE, this.transform.position);
            for (int i = 0; i < 16; i++)
            {
                squareIconSpawned = Instantiate(squareIcon, squareStartPos, Quaternion.identity, transform);
                squareIconSpawned.GetComponent<SpriteRenderer>().sortingOrder = 2;
                squaresSpawned.Add(squareIconSpawned);
            }
            InitializeWorldPositions();
            InitializePathfindingGrid();
            ChangeBuildingToEmpty();
        }
        private void InitializePathfindingGrid()
        {
            int[,] gridArray = GetGridArray();
            pathfindingGrid = new PathfindingGrid(grid.Width, grid.Height, gridArray, gridWorldPositions);
        }
        private void InitializeWorldPositions()
        {
            gridWorldPositions = new Vector3[grid.Width, grid.Height];

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    Vector3 gridPosition = new Vector3(x * CELL_SIZE, y * CELL_SIZE, 0);
                    Vector3 midPosition = grid.GetGridMidPoint(gridPosition);
                    gridWorldPositions[x, y] = midPosition;
                }
            }
            //Debug.Log("14: " + grid.GetGridMidPoint(new Vector3(14 * 32, 14 * 32, 0)));
            //Debug.Log("15: " + grid.GetGridMidPoint(new Vector3(15 * 32, 15 * 32, 0)));
        }
        private int[,] GetGridArray()
        {
            int[,] gridArray = new int[grid.Width, grid.Height];
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    //Debug.Log("grid.GetValue:  x:" + x + "y: " + y + " -> " + grid.GetValue(new Vector3(x, y, 0)));
                    gridArray[x, y] = grid.GetValue(x, y) < 1 ? 0 : 1;  //0 walkable.
                }
            }
            return gridArray;
        }

        //Request a path from the PathfindingGrid.
        public List<Vector3> RequestPath(Vector3 startPosition, Vector3 targetPosition)
        {
            UpdatePathfindingGrid();

            int startX, startY, targetX, targetY;
            grid.GetXYValuesOutside(startPosition, out startX, out startY);
            grid.GetXYValuesOutside(targetPosition, out targetX, out targetY);
            //Debug.Log("startX: " + startX + " startY: " + startY + " targetX :" + targetX + " targetY:" + targetY);
            return pathfindingGrid.FindPath(startX, startY, targetX, targetY);
        }
        private void UpdatePathfindingGrid()
        {
            int[,] gridArray = GetGridArray();  //Get updated grid.
            pathfindingGrid.InitializeNodes(gridArray, gridWorldPositions);  //Update nodes.
        }
        private void Update()
        {
            cachedWorldPosOffset = GetWorldPosOffset();
            //On mouse hover.
            if (grid.GetValueWithPos(cachedWorldPosOffset) > -1)
            {
                redCount = 0;
                int x, y;
                GetXYValuesOfBuildings(out x, out y);

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        cachedNewVector.Set(CELL_SIZE * i, CELL_SIZE * j, 0);
                        //Vector3 newVector = new Vector3(cellSize * i, cellSize * j, 0);
                        cachedWorldPos = cachedWorldPosOffset + cachedNewVector;
                        SelectGround(grid.GetGridMidPoint(cachedWorldPosOffset) + cachedNewVector, j + y * i, cachedWorldPos);
                    }
                }
            }
            else
            {
                ResetSquares();
            }

            //Building built logic.
            //On mouse left click.
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 clickedWorldPos = cachedWorldPosOffset;
                if (grid.GetValueWithPos(clickedWorldPos) == -1)
                {
                    //Reset values if clicked outside.
                    ChangeBuildingToEmpty();
                    //Debug.Log("Clicked outside grid, resetting values.");
                    return;
                }
                if (BuildingType.SelectedBuilding != BuildingType.BuildingTypes.Empty)
                {
                    if (redCount < 1 && grid.GetValueWithPos(cachedWorldPosOffset) > -1)
                    {
                        int x, y;
                        GetXYValuesOfBuildings(out x, out y);
                        Vector3 bottomLeftWorldPos = grid.GetGridMidPoint(cachedWorldPosOffset);
                        Vector3 topRightWorldPos = grid.GetGridMidPoint(cachedWorldPosOffset + new Vector3(CELL_SIZE * (x - 1), CELL_SIZE * (y - 1), 0));
                        Vector3 middleWorldPos = (bottomLeftWorldPos + topRightWorldPos) / 2f;
                        //Debug.Log("initialWorldPos: " + middleWorldPos);
                        currentBuildingBehavior = BuildingFactory.CreateBuilding(buildings[GetScriptableIdx()], middleWorldPos, this.transform);
                        GameObject buildingOnGrid = Instantiate(
                                        buildings[GetScriptableIdx()].buildingVisuals,
                                        middleWorldPos, Quaternion.identity,
                                        currentBuildingBehavior.transform);
                        BuildingOnGrid buildingOnGridComponent = buildingOnGrid.GetComponent<BuildingOnGrid>();
                        currentBuildingBehavior.SetBuildingOnGrid(buildingOnGridComponent);

                        for (int i = 0; i < x; i++)
                        {
                            for (int j = 0; j < y; j++)
                            {
                                Vector3 newVector = new Vector3(CELL_SIZE * i, CELL_SIZE * j, 0);
                                Vector3 worldPos = cachedWorldPosOffset + newVector;
                                SetValueAndSpawnBuilding(grid.GetGridMidPoint(cachedWorldPosOffset) + newVector, worldPos);
                            }
                        }
                        //Reset after placement.
                        ChangeBuildingToEmpty();
                    }
                }
            }
        }
        private Vector3 ClampToGridBounds(Vector3 position)
        {
            float minX = grid.GetGridMidPoint(Vector3.zero).x;
            float minY = grid.GetGridMidPoint(Vector3.zero).y;
            float maxX = minX + grid.GetGridMidPoint(new Vector3(grid.Width - 1, grid.Height - 1)).x;
            float maxY = minY + grid.GetGridMidPoint(new Vector3(grid.Width - 1, grid.Height - 1)).y;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            return position;
        }
        //Set value of the grid and spawn building.
        private void SetValueAndSpawnBuilding(Vector3 checkPos, Vector3 worldPos)
        {
            if (currentBuildingBehavior != null)
            {
                GameObject buildingSquare = Instantiate(squareIcon, checkPos, Quaternion.identity);
                buildingSquare.GetComponent<SpriteRenderer>().color = buildingColor;
                grid.SetValue(worldPos, (int)buildingValue);

                currentBuildingBehavior.OnPlace(worldPos, grid, buildingSquare);
            }
            else
            {
                //Debug.LogWarning("No building behavior created.");
            }
        }
        //Check ground buildable.
        private void SelectGround(Vector3 checkPos, int squareNum, Vector3 worldPos)
        {
            currentMidPoint[squareNum] = checkPos;
            if (currentMidPoint[squareNum] != activeMidPoint)
            {
                squaresSpawned[squareNum].transform.position = currentMidPoint[squareNum];
                activeMidPoint = currentMidPoint[squareNum];
                ChangeSquareColor(grid.GetValueWithPos(worldPos), squareNum);
            }
        }

        //Change color of square to show area availability.
        private void ChangeSquareColor(float value, int squareNum)
        {
            if (value > 0)
            {
                squaresSpawned[squareNum].GetComponent<SpriteRenderer>().color = Color.red;
                redCount++;
            }
            else
            {
                squaresSpawned[squareNum].GetComponent<SpriteRenderer>().color = Color.white;
            }
            if (value == -1)
            {
                squaresSpawned[squareNum].GetComponent<SpriteRenderer>().color = Color.red;
                redCount++;
            }
        }
        //Get size of the building as XY.
        private void GetXYValuesOfBuildings(out int x, out int y)
        {
            x = y = 0;
            int buildingIdx = GetScriptableIdx();
            if (buildingIdx != -1)
            {
                x = buildings[buildingIdx].buildingXValue;
                y = buildings[buildingIdx].buildingYValue;

                buildingColor = buildings[buildingIdx].buildingColor;
                buildingValue = buildings[buildingIdx].buildingNumber;
            }
            else
            {
                //Debug.LogWarning("No building selected or building index is invalid.");
                currentBuildingBehavior = null;
            }
        }
        public Vector3 GetGridMidPoint(Vector3 worldPosition)
        {
            return grid.GetGridMidPoint(worldPosition);
        }
        private int GetScriptableIdx()
        {
            int returnValue = -1;
            for (int i = 0; i < buildings.Length; i++)
            {
                if (buildings[i].types == BuildingType.SelectedBuilding)
                {
                    returnValue = i;
                }
            }
            return returnValue;
        }
        //Change to empty on click.
        private void ChangeBuildingToEmpty()
        {
            ResetSquares();
            OnBuildingTypeChanged.Invoke(BuildingType.BuildingTypes.Empty);
            buildingValue = 0;

            currentBuildingBehavior = null;
        }
        //Reset square positions.
        private void ResetSquares()
        {
            for (int i = 0; i < squaresSpawned.Count; i++)
            {
                squaresSpawned[i].transform.position = squareStartPos;
            }
        }
        public void SelectBuildingToPlace(BuildingType.BuildingTypes type)
        {
            OnBuildingTypeChanged.Invoke(type);
        }
        //Destroy Building
        public void NotifyBuildingDestroyed(Vector3 position)
        {
            //Reset the grid value when a building is destroyed.
            grid.SetValue(position, 0);
            //Debug.Log($"Grid updated: Building destroyed at {position}");
        }
        //Set soldiers.
        public void ClearAndSetNewPositionSoldier(Vector3 oldPos, Vector3 newPos, int soldierVal = 5)
        {
            if (oldPos != Vector3.zero)
            {
                grid.SetValue(oldPos, 0);
            }
            grid.SetValue(newPos, soldierVal);
        }
        public void SoldierDied(Vector3 pos)
        {
            grid.SetValue(pos, 0);
            //Debug.Log($"Grid updated: Soldier killed at {pos}");
        }
        public bool CanSelectUnit()
        {
            if (buildingValue == 0)
            {
                return true;
            }
            else
            { return false; }
        }
        //Get on click world pos.
        #region getWorldPos
        private Vector3 GetWorldPosOffset()
        {
            Vector3 vec = GetMouseWorldPos() - this.transform.position;
            vec.z = 0f;
            return vec;
        }
        private Vector3 GetMouseWorldPos()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }
        private Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
        public void SetGridBuildingsInfo(BuildingData[] allData)
        {
            buildings = allData;
        }
        //For testing.
        public Vector3 GetRandomGridPosition()
        {
            //Debug.Log("grid.Width: " + grid.Width + "grid.Height" + grid.Height);
            Vector3 randomPosition;
            int randomX, randomY;
            do
            {
                randomX = UnityEngine.Random.Range(0, grid.Width);
                randomY = UnityEngine.Random.Range(0, grid.Height);

                Vector3 gridPosition = new Vector3(randomX * CELL_SIZE, randomY * CELL_SIZE, 0);
                randomPosition = grid.GetGridMidPoint(gridPosition);

            } while (grid.GetValueWithPos(randomPosition) > 0);

            return randomPosition;
        }
        public Vector3 GetZeroZero()
        {
            //Debug.Log("grid.Width: " + grid.Width + "grid.Height" + grid.Height);
            Vector3 randomPosition;
            int randomX, randomY;
            do
            {
                randomX = 0;
                randomY = 0;

                Vector3 gridPosition = new Vector3(randomX * CELL_SIZE, randomY * CELL_SIZE, 0);
                randomPosition = grid.GetGridMidPoint(gridPosition);

            } while (grid.GetValueWithPos(randomPosition) > 0);

            return randomPosition;
        }
        #endregion
        //Check for unit spawn.
        #region SpawnCheck
        //Spawn check.
        public Vector3? GetClosestEmptyNeighbor(List<Vector3> occupiedGridPositions)
        {
            foreach (var occupiedPosition in occupiedGridPositions)
            {
                int x, y;
                grid.GetXYValuesBuilding(occupiedPosition, out x, out y);

                Vector2Int? emptyNeighbor = GetEmptyNeighboringGridPosition(x, y);

                if (emptyNeighbor.HasValue)
                {
                    int emptyX = emptyNeighbor.Value.x;
                    int emptyY = emptyNeighbor.Value.y;

                    //Return spawn pos.
                    Vector3 emptyWorldPos = grid.GetGridMidPoint(new Vector3(emptyX * CELL_SIZE, emptyY * CELL_SIZE, 0));
                    return emptyWorldPos;
                }
            }
            Debug.LogWarning("No empty neighbor found.");
            return null;
        }
        private Vector2Int? GetEmptyNeighboringGridPosition(int x, int y)
        {


            foreach (var offset in neighborOffsets)
            {
                int newX = x + offset.x;
                int newY = y + offset.y;
                if (newX >= 0 && newX < grid.Width && newY >= 0 && newY < grid.Height)
                {
                    if (grid.GetValue(newX, newY) == 0)
                    {
                        //Debug.Log($"Empty neighbor found at Grid X: {newX}, Y: {newY}");
                        return new Vector2Int(newX, newY); //Return found coord.
                    }
                }
            }
            return null;
        }
        #endregion
    }
}