using UnityEngine;

namespace Build
{
    public class BuildingOnGrid : MonoBehaviour
    {
        public HealthBar healthBar;
        public BuildingBehavior BuildingBehavior
        {
            get { return _buildingBehavior; }
            set
            {
                _buildingBehavior = value;
                //Debug.Log("Building behavior set for: " + _buildingBehavior?.name);
            }
        }
        private BuildingBehavior _buildingBehavior;
        public void UpdateHealth(float health)
        {
            healthBar?.UpdateHealth(health);
        }
        public void InitializeHealth(float health)
        {
            healthBar.Initialize(health);
        }
    }
}
