using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class GameManager : CanDurmazLibrary.Singleton<GameManager>
{
    [SerializeField] private Grids.GridController gridController;
    [SerializeField] private UnitManager unitManager;
    [Space]
    [Tooltip("Unit Infos.")]
    [SerializeField] private List<UnitData> unitDataList;
    [Space]
    [Tooltip("Building Infos.")]
    [SerializeField] Build.BuildingData[] buildings;
    private void Start()
    {
        UnitFactory.Initialize(unitDataList);
        gridController.SetGridBuildingsInfo(buildings);
    }
    public void HandleBuildingDestroyed(Vector3 position)
    {
        //Notify GridController to update the grid.
        gridController.NotifyBuildingDestroyed(position);
    }
    public Build.BuildingData GetBuildingData(Build.BuildingType.BuildingTypes buildingType)
    {
        foreach (Build.BuildingData building in buildings)
        {
            if (building.types == buildingType)
            {
                return building;
            }
        }

        //Debug.LogWarning("No building data found for type: " + buildingType);
        return null;
    }
    public UnitData GetUnitData(UnitType unitType)
    {
        foreach (UnitData data in unitDataList)
        {
            if (data.unitType == unitType)
            {
                return data;
            }
        }

        //Debug.LogWarning("No unit data found for type: " + unitType);
        return null;
    }
    public List<Vector3> RequestPath(Vector3 startPosition, Vector3 clickedWorldPos)
    {
        Vector3 adjustedWorldPos = AdjustToGridOffset(clickedWorldPos);
        Vector3 gridMidPoint = gridController.GetGridMidPoint(adjustedWorldPos);

        //Debug.Log("Adjusted Grid MidPoint: " + gridMidPoint);

        List<Vector3> path = gridController.RequestPath(startPosition, gridMidPoint);

        return path;
    }
    public Build.BuildingData[] GetBuildingData()
    {
        return buildings;
    }
    private Vector3 AdjustToGridOffset(Vector3 worldPosition)
    {
        Vector3 gridPosition = worldPosition - gridController.transform.position;
        gridPosition.z = 0f;
        return gridPosition;
    }
    public bool CanSelectUnit()
    {
        return gridController.CanSelectUnit();
    }
    public void SpawnUnit(UnitType unitType, Vector3 spawnPosition)
    {
        unitManager.SpawnUnit(unitType, spawnPosition);
    }
    public void ClearOldAndSetNewPositionSoldier(Vector3 oldPos, Vector3 newPos)
    {
        gridController.ClearAndSetNewPositionSoldier(oldPos, newPos);
    }
    public void SoldierDied(Vector3 pos)
    {
        gridController.SoldierDied(pos);
    }
    public void DeselectAllUnits()
    {
        var units = unitManager.GetAllUnits();
        foreach (UnitBehavior unit in units)
        {
            unit.DeselectUnit();
        }
    }
    public void RemoveUnit(UnitBehavior unit)
    {
        unitManager.RemoveUnit(unit);
    }
    //For testing.
    public Vector3 ReturnRandomGridPosition()
    {
        return gridController.GetRandomGridPosition();
    }
    public Vector3 ReturnZeroZero()
    {
        return gridController.GetZeroZero();
    }
    public void SelectBuilding(Build.BuildingType.BuildingTypes type)
    {
        gridController.SelectBuildingToPlace(type);
    }
    public UnitType GetRandomUnitType() => UnitManager.GetRandomUnitType();
    public Vector3? RequestClosestEmptyNeighbor(List<Vector3> occupiedGridPositions)
    {
        return gridController.GetClosestEmptyNeighbor(occupiedGridPositions);
    }
}
