using UnityEngine;
using System;


namespace Build
{
    public class BuildingType : MonoBehaviour
    {
        public enum BuildingTypes
        {
            PowerPlant,
            Barracks,
            Empty
        }

        public static BuildingTypes SelectedBuilding { get; private set; }
        public static event Action<BuildingTypes> OnBuildingTypeChanged;

        private void OnEnable()
        {
            Grids.GridController.OnBuildingTypeChanged += SetSelectedBuilding;
        }

        private void OnDisable()
        {
            Grids.GridController.OnBuildingTypeChanged -= SetSelectedBuilding;
        }

        private void SetSelectedBuilding(BuildingTypes type)
        {
            if (SelectedBuilding != type)
            {
                SelectedBuilding = type;
                OnBuildingTypeChanged?.Invoke(SelectedBuilding);
            }
        }
    }
}