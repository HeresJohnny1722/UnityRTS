using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWall : MonoBehaviour
{

    public Animator animator;

    private Building building;

    public Transform nearestTroop;

    [SerializeField] private float gateOpenRadius = 3f;

    [SerializeField] private LayerMask friendlyUnitLayerMask;

    private bool isOpen = false;

    private void Awake()
    {
        building = this.GetComponent<Building>();
    }


    // animator.Play("GateOpen");

    private void Update()
    {
        if (!building.buildingConstruction.isUnderConstruction)
        {
            bool shouldOpen = CheckForEnemies();

            if (shouldOpen && !isOpen)
            {
                //Open the gate
                animator.Play("GateOpen");
                isOpen = true;
                //Updates nav mesh in the animation

            }
            else if (shouldOpen && isOpen)
            {
                Debug.Log("Already Open");
            } else if (!shouldOpen && isOpen){ 
                //Close the gate
                animator.Play("GateClose");
                isOpen = false;
            }
        }

    }

    private bool CheckForEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.GetChild(1).position, gateOpenRadius, friendlyUnitLayerMask);

        if (colliders.Length > 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    
}
