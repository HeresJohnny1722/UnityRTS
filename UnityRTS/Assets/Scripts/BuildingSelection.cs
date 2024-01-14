using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Handles the selection and deselection of buildings, along with data persistence for building placement.
/// </summary>
public class BuildingSelection : MonoBehaviour
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

    // [SerializeField] private float buildingCount = 0;

    /// <summary>
    /// Initializes the BuildingSelection singleton instance.
    /// </summary>
    void Awake()
    {
        try
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
        catch (Exception ex)
        {
            Debug.LogError($"Error in Awake(): {ex.Message}");
        }
    }

    /// <summary>
    /// Selects a building and triggers necessary actions.
    /// </summary>
    public void SelectBuilding(Transform buildingToSelect)
    {
        try
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
        catch (Exception ex)
        {
            Debug.LogError($"Error in SelectBuilding(): {ex.Message}");
        }
    }

    /// <summary>
    /// Deselects the currently selected building.
    /// </summary>
    public void DeselectBuilding()
    {
        try
        {
            if (building != null)
            {
                building.deselectBuilding();
            }
            selectedBuilding = null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in DeselectBuilding(): {ex.Message}");
        }
    }

    /// <summary>
    /// Moves the flag to the specified position.
    /// </summary>
    public void MoveFlag(Vector3 point)
    {
        try
        {
            building.buildingTraining.moveFlag(point);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in MoveFlag(): {ex.Message}");
        }
    }

    
}
