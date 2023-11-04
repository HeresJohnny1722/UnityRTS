using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClick : MonoBehaviour
{
    Camera myCam;

    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask friendlyUnits;

    
    void Awake()
    {
        myCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    Selections.Instance.DeselectAll();
                }

            }

        }

        if (Input.GetMouseButtonDown(1))
        {

            RaycastHit hit;
            Ray ray = myCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {

                Selections.Instance.moveUnits(hit.point);

            }

        }
    }
}
