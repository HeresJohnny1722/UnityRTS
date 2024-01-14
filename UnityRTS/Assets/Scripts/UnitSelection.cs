using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using TMPro;

/// <summary>
/// Responsible for the selection of units and saving and loading the units positions and health
/// </summary>
public class UnitSelection : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    private static UnitSelection _instance;
    public static UnitSelection Instance { get { return _instance; } }

    [SerializeField] private float maxGroupSize = 8;

    [SerializeField] private GameObject groundMarker;
    [SerializeField] private GameObject unitInfoPanel;

    [SerializeField] private TextMeshProUGUI selectedUnitsTitle;
    [SerializeField] private TextMeshProUGUI selectedUnitsListText;

    private AstarAI myAstarAI;

    private Unit unitScript;

    private float proximityRadius;

    private FormationBase _formationBase;

    public FormationBase Formation
    {
        get
        {
            if (_formationBase == null) _formationBase = GetComponent<FormationBase>();
            return _formationBase;
        }
        set => _formationBase = value;
    }

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

    private List<Vector3> _points = new List<Vector3>();

    /// <summary>
    /// Moves the selected units to a position that is calculated using the Formation class
    /// </summary>
    /// <param name="moveToPosition"></param>
    public void moveUnits(Vector3 moveToPosition)
    {
        try
        {
            if (unitsSelected.Count > 0 && Formation != null)
            {
                setGroundMarker(groundMarker, moveToPosition);

                _points = Formation.EvaluatePoints().ToList();

                for (int i = 0; i < unitsSelected.Count; i++)
                {
                    myAstarAI = unitsSelected[i].GetComponent<AstarAI>();
                    myAstarAI.ai.destination = _points[i] + moveToPosition;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in moveUnits(): {ex.Message}");
        }
    }

    /// <summary>
    /// Deselects all units and selects the single unit
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void ClickSelectUnit(GameObject unitToAdd)
    {
        try
        {
            DeselectAll();
            SelectUnit(unitToAdd);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in ClickSelectUnit(): {ex.Message}");
        }
    }

    /// <summary>
    /// Adds the unit to the pre-selected unit when shift is held down when selecting units
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void ShiftClickSelect(GameObject unitToAdd)
    {
        try
        {
            if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
            {
                SelectUnit(unitToAdd);
            }
            else
            {
                unitsSelected.Remove(unitToAdd);
                unitScript = unitToAdd.GetComponent<Unit>();
                unitScript.deselectUnit();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in ShiftClickSelect(): {ex.Message}");
        }
    }

    /// <summary>
    /// Drag selection box and selection
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void DragSelect(GameObject unitToAdd)
    {
        try
        {
            if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
            {
                SelectUnit(unitToAdd);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in DragSelect(): {ex.Message}");
        }
    }

    /// <summary>
    /// Removes all units from the unitSelectionList
    /// </summary>
    public void DeselectAll()
    {
        try
        {
            foreach (var unit in unitsSelected)
            {
                unitScript = unit.GetComponent<Unit>();
                unitScript.deselectUnit();
            }
            unitsSelected.Clear();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in DeselectAll(): {ex.Message}");
        }
    }

    /// <summary>
    /// Function to select a unit to reduce duplicated code
    /// </summary>
    /// <param name="unitToSelect"></param>
    public void SelectUnit(GameObject unitToSelect)
    {
        try
        {
            if (unitToSelect.activeSelf == true)
            {
                unitsSelected.Add(unitToSelect);
                unitScript = unitToSelect.GetComponent<Unit>();
                unitScript.selectUnit();
                BuildingSelection.Instance.DeselectBuilding();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in SelectUnit(): {ex.Message}");
        }
    }

    /// <summary>
    /// Sets the ground marker to where the player clicked, also plays the animation
    /// </summary>
    /// <param name="groundMarkerObject"></param>
    /// <param name="groundMarkerPosition"></param>
    public void setGroundMarker(GameObject groundMarkerObject, Vector3 groundMarkerPosition)
    {
        try
        {
            groundMarkerObject.transform.position = groundMarkerPosition;
            groundMarkerObject.SetActive(false);
            groundMarkerObject.SetActive(true);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in setGroundMarker(): {ex.Message}");
        }
    }

    
}
