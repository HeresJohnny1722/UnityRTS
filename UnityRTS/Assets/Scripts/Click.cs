using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour
{
    Camera myCam;

    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask friendlyUnits;
    [SerializeField] private LayerMask building;

    
    void Awake()
    {
        myCam = Camera.main;
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {

            RaycastHit hit;
            Ray ray = myCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, friendlyUnits))
            {


                if (Input.GetKey(KeyCode.LeftShift))
                {

                    Selections.Instance.ShiftClickSelect(hit.collider.gameObject);

                }
                else
                {

                    Selections.Instance.ClickSelectUnit(hit.collider.gameObject);

                }

            } else if(Physics.Raycast(ray, out hit, Mathf.Infinity, building))
            {
                Debug.Log("Selecting Building");
                Selections.Instance.SelectBuilding(hit.transform);
            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    Selections.Instance.DeselectAll();
                }

            }

        }

        if (Input.GetMouseButtonDown(1) && !IsPointerOverUI())
        {

            RaycastHit hit;
            Ray ray = myCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                if (Selections.Instance.selectedBuilding != null)
                {
                    //Checks if it is a barracks to move the flag
                    if (Selections.Instance.selectedBuilding.parent.GetComponent<Building>().buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
                    {
                        Selections.Instance.selectedBuilding.parent.GetChild(1).gameObject.transform.position = hit.point;
                        Selections.Instance.selectedBuilding.parent.GetChild(1).gameObject.SetActive(true);
                    }
                    
                } else
                {
                    Selections.Instance.moveUnits(hit.point);
                }
                

            }

        }
    }
}
