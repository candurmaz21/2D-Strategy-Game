using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Unit Data", order = 5)]
public class UnitData : ScriptableObject
{
    public Unit.UnitType unitType;
    public GameObject unitPrefab;
    public float health;
    public float moveSpeed;
    public float damage;
    public float range;
}

