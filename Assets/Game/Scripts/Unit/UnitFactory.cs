using System.Collections;
using System.Collections.Generic;
using CanDurmazLibrary;
using UnityEngine;

namespace Unit
{
    public class UnitFactory
    {
        private static List<UnitData> unitDataList;
        public static void Initialize(List<UnitData> data)
        {
            unitDataList = data;
        }
        public static UnitBehavior CreateUnit(UnitType unitType, Vector3 spawnPosition, Transform parent)
        {
            UnitData selectedUnitData = unitDataList.Find(u => u.unitType == unitType);

            if (selectedUnitData != null && selectedUnitData.unitPrefab != null)
            {
                //GameObject unitInstance = Object.Instantiate(selectedUnitData.unitPrefab, spawnPosition, Quaternion.identity, parent);
                GameObject unitInstance = ObjectPooler.Instance.GetPooledObject(unitType.ToString(), spawnPosition, Quaternion.identity);
                UnitBehavior unitBehavior = unitInstance.GetComponent<UnitBehavior>();

                unitBehavior.Initialize(selectedUnitData);
                return unitBehavior;
            }

            Debug.LogWarning("Unit data or prefab is missing.");
            return null;
        }
    }
}