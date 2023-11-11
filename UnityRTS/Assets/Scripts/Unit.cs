using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitSO unitSO;

    [SerializeField] private GameObject unitFloorHighlight;
    [SerializeField] private Healthbar unitHealthbar;

    private float unitHealth;
    private float unitWalkingSpeed;

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
            deselectUnit();

            UnitSelection.Instance.unitsSelected.Remove(this.gameObject);
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

        }
        
    }

    public void deselectUnit()
    {
        unitFloorHighlight.SetActive(false);
        unitHealthbar.gameObject.SetActive(false);
    }
}
