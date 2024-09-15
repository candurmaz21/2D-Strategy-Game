using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionController : MonoBehaviour
{
    public GameObject scrollContentUI;
    public ScrollContent scrollContent;
    void Start()
    {
        var buildings = GameManager.Instance.GetBuildingData();
        foreach (var building in buildings)
        {
            GameObject buildingButton = Instantiate(building.buildingUIButton, scrollContentUI.transform);
        }
        scrollContent.Initialize();
        scrollContent.UpdateAfterAddingChildren();
    }
}
