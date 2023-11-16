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
    [SerializeField] private LayerMask resourceNode;


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

                    UnitSelection.Instance.ShiftClickSelect(hit.collider.gameObject);

                }
                else
                {

                    UnitSelection.Instance.ClickSelectUnit(hit.collider.gameObject);

                }

            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, building))
            {
                BuildingSelection.Instance.SelectBuilding(hit.transform);

            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    BuildingSelection.Instance.DeselectBuilding();
                    UnitSelection.Instance.DeselectAll();
                }

            }

        }

        if (Input.GetMouseButtonDown(1) && !IsPointerOverUI())
        {

            RaycastHit hit;
            Ray ray = myCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, building))
            {
                Debug.Log("Trying to move to a building");
                UnitSelection.Instance.moveWorkersIntoProductionBuilding(hit.transform);

            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                if (BuildingSelection.Instance.selectedBuilding != null)
                {
                    BuildingSelection.Instance.MoveFlag(hit.point);

                }
                else
                {

                    UnitSelection.Instance.moveUnits(hit.point);
                }




                /*if (hit.transform.GetComponent < buildingSO.buildingType == BuildingSO.BuildingType.Production)
                {

                }*/


            }
        }
    }
}
