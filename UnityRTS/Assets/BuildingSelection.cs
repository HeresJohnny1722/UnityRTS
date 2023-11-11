using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BuildingSelection : MonoBehaviour
{
    public List<GameObject> buildingsList = new List<GameObject>();
    public Transform selectedBuilding = null;

    private static BuildingSelection _instance;
    public static BuildingSelection Instance { get { return _instance; } }

    private Building building;


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void takeDamageBuildingTest(float damage)
    {
        if (selectedBuilding != null && selectedBuilding.parent != null)
        {
            selectedBuilding.parent.GetComponent<Building>().takeDamage(damage);
        }
    }

    public void SelectBuilding(Transform buildingToSelect)
    {
        DeselectBuilding();
        selectedBuilding = buildingToSelect;
        building = selectedBuilding.parent.GetComponent<Building>();
        building.BuildingSelected();
    }


    public void DeselectBuilding()
    {
        if (building != null)
        {
            building.deselectBuilding();
        }
        selectedBuilding = null;
        
    }

    
    public void MoveFlag(Vector3 point)
    {
        building.moveFlag(point);
    }
}
