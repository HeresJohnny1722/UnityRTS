using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public UnitSO unitSO;

    [SerializeField] private GameObject unitFloorHighlight;
    [SerializeField] private Healthbar unitHealthbar;

    public float unitHealth;
    private float unitWalkingSpeed;

    private NavMeshAgent myAgent;

    void Start()
    {
        unitHealth = unitSO.startingHealth;
        unitHealthbar.UpdateHealthBar(unitSO.startingHealth, unitHealth);
        unitHealthbar.gameObject.SetActive(false);

        UnitSelection.Instance.unitList.Add(this.gameObject);


    }


    public void takeDamage(float damageAmount)
    {
        unitHealth -= damageAmount;

        if (unitHealth <= 0)
        {
            //deselectUnit();
            InventoryManager.instance.changeCurrentPopulation(-(int)unitSO.populationCost);

            if (UnitSelection.Instance.unitsSelected.Contains(this.gameObject))
            {
                UnitSelection.Instance.unitsSelected.Remove(this.gameObject);
            }
            
            UnitSelection.Instance.unitList.Remove(this.gameObject);
            Destroy(this.gameObject);

        }
        unitHealthbar.gameObject.SetActive(true);
        unitHealthbar.UpdateHealthBar(unitSO.startingHealth, unitHealth);
    }

    public void selectUnit()
    {
        unitFloorHighlight.SetActive(true);
        //only if health stuff

        if (unitSO.startingHealth == unitHealth)
        {
            unitHealthbar.gameObject.SetActive(false);

        } else
        {
            unitHealthbar.gameObject.SetActive(true);
        }
        
    }

    public void deselectUnit()
    {
        unitFloorHighlight.SetActive(false);
        unitHealthbar.gameObject.SetActive(false);
    }

    public bool isEnteringProduction = false;
    private Transform buildingTransform = null;

    public void productionBuildingStuff(Transform buildingToEnter)
    {
        myAgent = GetComponent<NavMeshAgent>();
        float radius = 1f;

        NavMesh.SamplePosition(buildingTransform.position, out NavMeshHit hit, radius, NavMesh.AllAreas);

        // Set destination for the agent to the closest point on the nav mesh
        myAgent.SetDestination(hit.position);

        //myAgent.SetDestination(buildingToEnter.position);
        buildingTransform = buildingToEnter;
    }


    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == buildingTransform)
        {
            Debug.Log("Entering Building");
            buildingTransform = null;
        }
    }*/


}
