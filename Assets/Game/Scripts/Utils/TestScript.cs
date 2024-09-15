using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Update()
    {
        //Give 5 damage to all buildings.
        if (Input.GetKeyDown(KeyCode.D))
        {
            ApplyDamageToAllBuildings(5);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnRandomUnit();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnUnitZeroZero();
        }
    }

    void ApplyDamageToAllBuildings(int damage)
    {
        BuildingBehavior[] buildings = FindObjectsOfType<BuildingBehavior>();

        foreach (var building in buildings)
        {
            building.TakeDamage(damage);
            //Debug.Log($"{building.name} took {damage} damage.");
        }
    }
    void SpawnUnit()
    {
        GameManager.Instance.SpawnUnit(UnitType.Infantry1, GameManager.Instance.ReturnRandomGridPosition());
    }
    void SpawnUnitZeroZero()
    {
        GameManager.Instance.SpawnUnit(UnitType.Infantry1, GameManager.Instance.ReturnZeroZero());
    }
    void SpawnRandomUnit()
    {
        UnitType randomType = UnitManager.GetRandomUnitType();
        //Debug.Log("randomType: " + randomType);
        GameManager.Instance.SpawnUnit(randomType, GameManager.Instance.ReturnRandomGridPosition());
    }
}
