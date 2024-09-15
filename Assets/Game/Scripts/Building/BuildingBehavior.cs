using System.Collections.Generic;
using Build;
using UnityEngine;

public abstract class BuildingBehavior : MonoBehaviour, Grids.IBuilding
{
    protected Color buildingColor;
    protected float buildingValue;
    protected BuildingData buildingData;
    protected float health;
    protected List<Vector3> occupiedGridPositions = new List<Vector3>();
    private List<GameObject> buildingSquares = new List<GameObject>();
    private BuildingOnGrid buildingOnGrid;

    //Initialize.
    public virtual void Initialize(BuildingData data)
    {
        buildingData = data;
        buildingColor = data.buildingColor;
        buildingValue = data.buildingNumber;
        health = data.health;
    }
    public void SetBuildingOnGrid(BuildingOnGrid buildingOnGrid)
    {
        buildingOnGrid.BuildingBehavior = this;
        this.buildingOnGrid = buildingOnGrid;
        this.buildingOnGrid.gameObject.transform.SetParent(this.transform);
        buildingOnGrid?.InitializeHealth(health);
    }
    public virtual void OnPlace(Vector3 position, Grids.IGrid grid, GameObject square)
    {
        grid.SetValue(position, (int)buildingValue);
        occupiedGridPositions.Add(position);
        buildingSquares.Add(square);
        //Debug.Log($"{buildingData.buildingName} placed at {position}");
    }
    public virtual void OnHover(Vector3 position, Grids.IGrid grid)
    {
        //Debug.Log($"Hovering over {buildingData.buildingName} at {position}");
    }
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        //Debug.Log($"{buildingData.buildingName} took {damage} damage. Health now: {health}");
        buildingOnGrid?.UpdateHealth(health);
        if (health <= 0)
        {
            DestroyBuilding();
        }
    }
    public virtual void SetDetailsImages()
    {
        DetailsController.Instance.SetBuildingImage(buildingData.buildingImage, buildingData.productionImage);
    }
    protected virtual void DestroyBuilding()
    {
        foreach (var gridPosition in occupiedGridPositions)
        {
            //Notify the GameManager, not the GridController directly.
            GameManager.Instance.HandleBuildingDestroyed(gridPosition);
        }
        //Destroy all associated squares.
        foreach (var square in buildingSquares)
        {
            Destroy(square);
        }
        //Debug.Log($"{buildingData.buildingName} destroyed!");
        Destroy(gameObject);
    }

    public virtual void ExecuteObjective()
    {
        //Debug.Log($"Executing objective: {buildingData.objective} for {buildingData.buildingName}");
        //Add if button or sth.
    }
}
