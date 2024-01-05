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

        for (int i = 1; i < buildingsList.Count; i++)
        {
            buildingsList.RemoveAt(i);
        }

        for (int i = 0; i < data.buildingsListName.Count; i++)
        {
            string buildingName = data.buildingsListName[i];
            Vector3 buildingPosition = data.buildingsListPositions[i];

            // Find the corresponding prefab in GameManager.instance.buildingPrefabs
            GameObject prefab = FindBuildingPrefab(buildingName);

            if (prefab != null)
            {
                // Instantiate the prefab and add it to the buildingsList and set its health to the saved health
                GameObject buildingObject = Instantiate(prefab, buildingPosition, Quaternion.identity);
                Building building = buildingObject.GetComponent<Building>();

                building.buildingHealth = data.buildingListHealth[i];
                Debug.Log(data.buildingListHealth[i]);

                //If the building has taken damage, we should see the healthbar
                if (building.buildingHealth != building.buildingSO.startingHealth)
                {
                    building.buildingHealthbar.gameObject.SetActive(true);
                    building.buildingHealthbar.UpdateHealthBar(building.buildingSO.startingHealth, building.buildingHealth);
                } else
                {
                    building.buildingHealthbar.gameObject.SetActive(false);
                    building.buildingHealthbar.UpdateHealthBar(building.buildingSO.startingHealth, building.buildingHealth);
                }

                //buildingsList.Add(buildingObject);
            }
            else
            {
                Debug.LogWarning("Prefab not found for building: " + buildingName);
            }
        }
    }

    private GameObject FindBuildingPrefab(string buildingName)
    {
        // Find the prefab in GameManager.instance.buildingPrefabs
        foreach (var prefab in GameManager.instance.buildingPrefabs)
        {
            // Compare the first 5 characters of the prefab name with the provided buildingName
            if (prefab.name.Length >= 5 && prefab.name.Substring(0, 5) == buildingName.Substring(0, 5))
            {
                return prefab;
            }
        }

        return null; // If no matching prefab is found
    }


    public void SaveData(GameData data)
    {
        data.buildingsListName.Clear();
        data.buildingsListPositions.Clear();
        data.buildingListHealth.Clear();

        for (int i = 1; i < this.buildingsList.Count; i++)
        {
            data.buildingsListName.Add(this.buildingsList[i].name);
            data.buildingsListPositions.Add(this.buildingsList[i].transform.position);
            data.buildingListHealth.Add((int)this.buildingsList[i].GetComponent<Building>().buildingHealth);
        }

      //  Debug.Log(data.buildingsListName.Count);

        
      //  data.listBuilding = this.buildingsList;
    }
}
