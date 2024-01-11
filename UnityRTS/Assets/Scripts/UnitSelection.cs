using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using TMPro;
using System;

/// <summary>
/// Responsible for the selection of units and saving and loading the units positions and health
/// </summary>
public class UnitSelection : MonoBehaviour, IDataPersistence
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
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private List<Vector3> _points = new List<Vector3>();

    /// <summary>
    /// Moves the selected units to a position that is calculated using the Formation class
    /// </summary>
    /// <param name="moveToPosition"></param>
    public void moveUnits(Vector3 moveToPosition)
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

    /// <summary>
    /// Deselects all units and selects the single unit
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void ClickSelectUnit(GameObject unitToAdd)
    {
        DeselectAll();
        SelectUnit(unitToAdd);
    }

    /// <summary>
    /// Adds the unit to the pre selected unit when shift is held down when selecting units
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void ShiftClickSelect(GameObject unitToAdd)
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

    /// <summary>
    /// Drag selection box and selection
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void DragSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
        {
            SelectUnit(unitToAdd);
        }
    }

    /// <summary>
    /// Removes all units from the unitSelectionList
    /// </summary>
    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            unitScript = unit.GetComponent<Unit>();
            unitScript.deselectUnit();
        }
        unitsSelected.Clear();

    }

    /// <summary>
    /// Function to select unit to reduce duplicated code
    /// </summary>
    /// <param name="unitToSelect"></param>
    public void SelectUnit(GameObject unitToSelect)
    {
        if (unitToSelect.activeSelf == true)
        {
            unitsSelected.Add(unitToSelect);
            unitScript = unitToSelect.GetComponent<Unit>();
            unitScript.selectUnit();
            BuildingSelection.Instance.DeselectBuilding();
        }

    }

    /// <summary>
    /// Sets the ground marker to where the player clicked, also plays the animation
    /// </summary>
    /// <param name="groundMarkerObject"></param>
    /// <param name="groundMarkerPosition"></param>
    public void setGroundMarker(GameObject groundMarkerObject, Vector3 groundMarkerPosition)
    {
        groundMarkerObject.transform.position = groundMarkerPosition;
        groundMarkerObject.SetActive(false);
        groundMarkerObject.SetActive(true);
    }

    /// <summary>
    /// Loads the positons and health of the units when the game loads in from a save file
    /// </summary>
    /// <param name="data"></param>
    public void LoadData(GameData data)
    {

        unitList.Clear();

        for (int i = 0; i < data.unitListName.Count; i++)
        {
            string unitName = data.unitListName[i];
            Vector3 unitPosition = data.unitListPositions[i];

            // Find the corresponding prefab in GameManager.instance.buildingPrefabs
            GameObject prefab = FindBuildingPrefab(unitName);

            if (prefab != null)
            {
                // Instantiate the prefab and add it to the buildingsList and set its health to the saved health
                GameObject unitObject = Instantiate(prefab, unitPosition, Quaternion.identity);
                Unit unit = unitObject.GetComponent<Unit>();

                
                    myAstarAI = unitObject.GetComponent<AstarAI>();
                    myAstarAI.ai.destination = unitPosition;


                unit.unitHealth = data.unitListHealth[i];
                Debug.Log(data.unitListHealth[i]);

                //If the building has taken damage, we should see the healthbar
                if (unit.unitHealth != unit.unitSO.startingHealth)
                {
                    unit.unitHealthbar.gameObject.SetActive(true);
                    unit.unitHealthbar.UpdateHealthBar(unit.unitSO.startingHealth, unit.unitHealth);
                } else
                {
                    unit.unitHealthbar.UpdateHealthBar(unit.unitSO.startingHealth, unit.unitHealth);
                    unit.unitHealthbar.gameObject.SetActive(false);
                }

                //buildingsList.Add(buildingObject);
            }
            else
            {
                Debug.LogWarning("Prefab not found for unit: " + unitName);
            }
        }
    }

    private GameObject FindBuildingPrefab(string unitName)
    {
        // Find the prefab in GameManager.instance.buildingPrefabs
        foreach (var prefab in GameManager.instance.unitPrefabs)
        {
            // Compare the first 5 characters of the prefab name with the provided buildingName
            if (prefab.name.Length >= 5 && prefab.name.Substring(0, 5) == unitName.Substring(0, 5))
            {
                return prefab;
            }
        }

        return null; // If no matching prefab is found
    }

    public void SaveData(GameData data)
    {
        data.unitListName.Clear();
        data.unitListPositions.Clear();
        data.unitListHealth.Clear();

        for (int i = 0; i < this.unitList.Count; i++)
        {
            data.unitListName.Add(this.unitList[i].name);
            data.unitListPositions.Add(this.unitList[i].transform.position);
            data.unitListHealth.Add((int)this.unitList[i].GetComponent<Unit>().unitHealth);
        }

    }

}
