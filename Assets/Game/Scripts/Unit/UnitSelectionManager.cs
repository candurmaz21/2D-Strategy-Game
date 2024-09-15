using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
    public class UnitSelectionManager : MonoBehaviour
    {
        private UnitBehavior selectedUnit;

        public void DeselectCurrentUnit()
        {
            if (selectedUnit != null)
            {
                selectedUnit.DeselectUnit();
                selectedUnit = null;
            }
        }

        public void SelectUnit(UnitBehavior unit)
        {
            //Deselect first.
            DeselectCurrentUnit();

            selectedUnit = unit;
            selectedUnit.SelectUnit();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!GameManager.Instance.CanSelectUnit())
                    return;


                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Debug.DrawRay(mousePosition, Vector2.zero, Color.red, 1f);

                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null)
                {

                    UnitBehavior clickedUnit = hit.collider.GetComponent<UnitBehavior>();
                    if (clickedUnit != null)
                    {
                        SelectUnit(clickedUnit);
                    }
                }
                else
                {
                    //Debug.Log("Raycast did not hit anything.");
                }
            }
        }
    }
}
