using UnityEngine;

public static class BuildingFactory
{
    public static BuildingBehavior CreateBuilding(Build.BuildingData buildingData, Vector3 position, Transform parent)
    {
        GameObject buildingObject = new GameObject(buildingData.buildingName);
        buildingObject.transform.position = position;
        buildingObject.transform.SetParent(parent);

        BuildingBehavior buildingBehavior;

        switch (buildingData.types)
        {
            case Build.BuildingType.BuildingTypes.PowerPlant:
                buildingBehavior = buildingObject.AddComponent<PowerPlantBehavior>();
                break;
            case Build.BuildingType.BuildingTypes.Barracks:
                buildingBehavior = buildingObject.AddComponent<BarracksBehavior>();
                break;
            default:
                buildingBehavior = buildingObject.AddComponent<GenericBuildingBehavior>();
                break;
        }

        buildingBehavior.Initialize(buildingData);
        return buildingBehavior;
    }
}
