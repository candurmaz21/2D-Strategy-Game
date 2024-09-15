using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
    public class UnitManager : MonoBehaviour
    {
        private List<UnitBehavior> allUnits = new List<UnitBehavior>();

        public void SpawnUnit(UnitType unitType, Vector3 spawnPosition)
        {
            UnitBehavior newUnit = UnitFactory.CreateUnit(unitType, spawnPosition, transform);
            if (newUnit != null)
            {
                allUnits.Add(newUnit);
            }
        }

        public void RemoveUnit(UnitBehavior unit)
        {
            if (allUnits.Contains(unit))
            {
                allUnits.Remove(unit);
            }
        }

        public List<UnitBehavior> GetAllUnits()
        {
            return allUnits;
        }
        public static UnitType GetRandomUnitType()
        {
            UnitType[] unitTypes = (UnitType[])System.Enum.GetValues(typeof(UnitType));
            int randomIndex = Random.Range(1, unitTypes.Length);
            return unitTypes[randomIndex];
        }
    }
}
