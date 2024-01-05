using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BuildingSelection : MonoBehaviour, IDataPersistence
{
    public List<GameObject> buildingsList = new List<GameObject>();
    public Transform selectedBuilding = null;

    private static BuildingSelection _instance;
    public static BuildingSelection Instance { get { return _instance; } }

    private Building building;

    [SerializeField] BuildingGridPlacer buildingGridPlacer;

    [SerializeField] private float buildingCount = 0;


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

    public void SelectBuilding(Transform buildingToSelect)
    {

        DeselectBuilding();
        selectedBuilding = null;

        if (buildingGridPlacer._buildingPrefab == null)
        {
            building = buildingToSelect.GetComponent<Building>();

            
            UnitSelection.Instance.DeselectAll();
            DeselectBuilding();
            selectedBuilding = buildingToSelect;
            
            building.BuildingSelected();
        }
            
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
        building.buildingTraining.moveFlag(point);
    }

    public void LoadData(GameData data)
    {
        //this.buildingCount = data.buildingCount;
        //        this.buildingsList = data.listBuilding;

        foreach (var building in data.buildingsList)
        {
            Debug.Log(building);


        }

        foreach (var buildingPos in data.buildingsListPositions)
        {
            Debug.Log(buildingPos);
        }

    }

    public void SaveData(GameData data)
    {
        //data.buildingCount = this.buildingsList.Count;

        for (int i = 0; i < this.buildingsList.Count; i++)
        {
            data.buildingsList.Add(this.buildingsList[i].name);
            data.buildingsListPositions.Add(this.buildingsList[i].transform.position);
        }

        
      //  data.listBuilding = this.buildingsList;
    }
}
