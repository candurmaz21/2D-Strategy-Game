using UnityEngine;


namespace Build
{
    [CreateAssetMenu(fileName = "Buildings", menuName = "ScriptableObjects/Buildings")]
    public class BuildingData : ScriptableObject
    {
        public BuildingType.BuildingTypes types;
        public Sprite buildingImage;
        public Sprite productionImage;
        public GameObject buildingVisuals;
        public GameObject buildingUIButton;
        public int buildingNumber;
        public int buildingXValue;
        public int buildingYValue;
        public Color buildingColor;
        public string buildingName;
        public string productionName;
        public string objective;
        public int health;
        public int energyProduction; // Optional.
        public float troopCooldown;// Optional.
        //...
        //Other values that can be added.
    }
}
