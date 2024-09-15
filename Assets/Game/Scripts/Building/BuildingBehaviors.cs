using System.Collections;
using UnityEngine;

public class PowerPlantBehavior : BuildingBehavior
{
    public int energyProduction;

    public override void Initialize(Build.BuildingData data)
    {
        base.Initialize(data);
        energyProduction = data.energyProduction;
    }

    public override void ExecuteObjective()
    {
        base.ExecuteObjective();
        //Debug.Log("Generating energy...");
    }
}

public class BarracksBehavior : BuildingBehavior
{
    public float troopCooldown;
    private Coroutine spawnCoroutine;


    public override void Initialize(Build.BuildingData data)
    {
        base.Initialize(data);
        troopCooldown = data.troopCooldown;

        spawnCoroutine = StartCoroutine(SpawnUnitRoutine());
    }

    public override void ExecuteObjective()
    {
        base.ExecuteObjective();
        //Debug.Log("Train troops with click or something.");
    }
    private IEnumerator SpawnUnitRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(troopCooldown); //wait troop cooldown.
            TrySpawnRandomUnit();
        }
    }

    private void TrySpawnRandomUnit()
    {
        Vector3? spawnPosition = GameManager.Instance.RequestClosestEmptyNeighbor(occupiedGridPositions);

        if (spawnPosition.HasValue)
        {
            //If a valid spawn position is found, spawn random unit.
            GameManager.Instance.SpawnUnit(GameManager.Instance.GetRandomUnitType(), spawnPosition.Value);
            //Debug.Log($"Spawning a soldier at {spawnPosition.Value}");
        }
        else
        {
            //Debug.Log("No empty neighbor grid found for barracks.");
        }
    }
    protected override void DestroyBuilding()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        base.DestroyBuilding();
    }
}
public class GenericBuildingBehavior : BuildingBehavior
{
    public override void Initialize(Build.BuildingData data)
    {
        base.Initialize(data);
    }

    public override void ExecuteObjective()
    {
        base.ExecuteObjective();
        //Debug.Log($"Executing generic objective for {buildingData.buildingName}");
        //Add if button.
    }
}

