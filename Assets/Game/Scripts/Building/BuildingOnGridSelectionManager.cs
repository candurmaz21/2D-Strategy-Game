using UnityEngine;

namespace Build
{
    public class BuildingOnGridSelectionManager : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!GameManager.Instance.CanSelectUnit())
                    return;
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
                if (hit.collider != null)
                {
                    BuildingOnGrid buildingOnGrid = hit.collider.GetComponent<BuildingOnGrid>();
                    if (buildingOnGrid != null)
                    {
                        GameManager.Instance.DeselectAllUnits();
                        BuildingBehavior selectedBehavior = buildingOnGrid.BuildingBehavior;
                        selectedBehavior.SetDetailsImages();
                    }
                    else
                    {
                        DetailsController.Instance.SetEmptyImage();
                        //Debug.Log("Raycast did not hit building.");
                    }
                }
            }
        }
    }
}
