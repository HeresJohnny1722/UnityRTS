using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Handles the selection and deselection of buildings, along with data persistence for building placement.
/// </summary>
public class BuildingSelection : MonoBehaviour, IDataPersistence
{
    /// <summary>
    /// List of buildings placed in the game.
    /// </summary>
    public List<GameObject> buildingsList = new List<GameObject>();

    public Transform selectedBuilding = null;

    public static BuildingSelection _instance;
    public static BuildingSelection Instance { get { return _instance; } }

    private Building building;

    [SerializeField] BuildingGridPlacer buildingGridPlacer;

//    [SerializeField] private float buildingCount = 0;

    /// <summary>
    /// Initializes the BuildingSelection singleton instance.
    /// </summary>
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

    /// <summary>
    /// Selects a building and triggers necessary actions.
    /// </summary>
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

    /// <summary>
    /// Deselects the currently selected building.
    /// </summary>
    public void DeselectBuilding()
    {
        if (building != null)
        {
            building.deselectBuilding();
        }
        selectedBuilding = null;
    }

    /// <summary>
    /// Moves the flag to the specified position.
    /// </summary>
    public void MoveFlag(Vector3 point)
    {
        building.buildingTraining.moveFlag(point);
    }

    /// <summary>
    /// Loads building placement data from the provided GameData object.
    /// </summary>
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

            GameObject prefab = FindBuildingPrefab(buildingName);

            if (prefab != null)
            {
                GameObject buildingObject = Instantiate(prefab, buildingPosition, Quaternion.identity);
                Building building = buildingObject.GetComponent<Building>();
                GameManager.instance.increaseBuildingCount(building.buildingSO);

                building.buildingHealth = data.buildingListHealth[i];

                if (building.buildingHealth != building.buildingSO.startingHealth)
                {
                    building.buildingHealthbar.gameObject.SetActive(true);
                    building.buildingHealthbar.UpdateHealthBar(building.buildingSO.startingHealth, building.buildingHealth);
                }
                else
                {
                    building.buildingHealthbar.gameObject.SetActive(false);
                    building.buildingHealthbar.UpdateHealthBar(building.buildingSO.startingHealth, building.buildingHealth);
                }
            }
            else
            {
                Debug.LogWarning("Prefab not found for building: " + buildingName);
            }
        }
    }

    /// <summary>
    /// Finds the prefab associated with the provided building name.
    /// </summary>
    private GameObject FindBuildingPrefab(string buildingName)
    {
        foreach (var prefab in GameManager.instance.buildingPrefabs)
        {
            if (prefab.name.Length >= 5 && prefab.name.Substring(0, 5) == buildingName.Substring(0, 5))
            {
                return prefab;
            }
        }

        return null;
    }

    /// <summary>
    /// Saves building placement data to the provided GameData object.
    /// </summary>
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
    }
}
