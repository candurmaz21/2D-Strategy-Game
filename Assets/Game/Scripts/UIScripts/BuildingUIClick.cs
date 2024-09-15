using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIClick : MonoBehaviour
{
    [SerializeField] Build.BuildingType.BuildingTypes type;
    [SerializeField] Button button;
    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }
    private void OnButtonClick()
    {
        GameManager.Instance.SelectBuilding(type);
    }

}
