using UnityEngine;
using TMPro;


namespace Build
{
    public class BuildingUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text textSelectedBuildingText;

        private void OnEnable()
        {
            BuildingType.OnBuildingTypeChanged += UpdateSelectedBuildingText;
        }

        private void OnDisable()
        {
            BuildingType.OnBuildingTypeChanged -= UpdateSelectedBuildingText;
        }
        private void UpdateSelectedBuildingText(BuildingType.BuildingTypes selectedType)
        {
            BuildingData data = GameManager.Instance.GetBuildingData(selectedType);
            GameManager.Instance.DeselectAllUnits();
            DetailsController.Instance.SetEmptyImage();
            if (data == null)
            {
                textSelectedBuildingText.text = "-";
            }
            else
            {
                textSelectedBuildingText.text = data.types.ToString();
            }
        }
    }
}
