using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailsController : CanDurmazLibrary.Singleton<DetailsController>
{
    [Header("Details Information")]
    [SerializeField] private Image buildingImage;
    [SerializeField] private Image productionImage;
    [SerializeField] private Sprite emptyImage;

    //Set images.
    private void Start()
    {
        SetEmptyImage();
    }
    public void SetBuildingImage(Sprite buildingImg, Sprite productionImg)
    {
        buildingImage.sprite = buildingImg;
        productionImage.sprite = productionImg;
    }
    public void SetEmptyImage()
    {
        buildingImage.sprite = emptyImage;
        productionImage.sprite = emptyImage;
    }

}
