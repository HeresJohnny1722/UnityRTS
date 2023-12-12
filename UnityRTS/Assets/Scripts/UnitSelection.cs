using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using TMPro;
using System;

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

    private NavMeshAgent myAgent;

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

    public void moveUnits(Vector3 moveToPosition)
    {
        if (unitsSelected.Count > 0 && Formation != null)
        {
            setGroundMarker(groundMarker, moveToPosition);

            _points = Formation.EvaluatePoints().ToList();

            for (int i = 0; i < unitsSelected.Count; i++)
            {
                myAgent = unitsSelected[i].GetComponent<NavMeshAgent>();
                myAgent.SetDestination(_points[i] + moveToPosition);
                
                Unit unit = unitsSelected[i].GetComponent<Unit>();
                unit.moveToPosition = _points[i] + moveToPosition;
                //unit.currentState = Unit.UnitState.Moving;
                //Debug.Log(unit.currentState);
                //unit.muzzleFlash.SetActive(false);
            }
        }
    }


    

    private void Update()
    {
        if (unitsSelected.Count == 0)
        {
            unitInfoPanel.SetActive(false);
        }
    }


    public void takeDamageUnitTest(float damage)
    {


        for (int i = 0; i < unitsSelected.Count; i++)
        {
            unitsSelected[i].GetComponent<Unit>().takeDamage(damage);
        }
    }

    public void ClickSelectUnit(GameObject unitToAdd)
    {
        DeselectAll();
        SelectUnit(unitToAdd);

    }

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

            updateInfoPanelForUnits();
            if (unitsSelected.Count <= 0)
            {
                unitInfoPanel.SetActive(false);
            }
        }
    }

    public void DragSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
        {
            SelectUnit(unitToAdd);
        }
    }

    public void DeselectAll()
    {

        foreach (var unit in unitsSelected)
        {

            unitScript = unit.GetComponent<Unit>();
            unitScript.deselectUnit();
            updateInfoPanelForUnits();
            unitInfoPanel.SetActive(false);
        }
        unitsSelected.Clear();


    }

    public void SelectUnit(GameObject unitToSelect)
    {
        if (unitToSelect.activeSelf == true)
        {
            unitsSelected.Add(unitToSelect);
            unitScript = unitToSelect.GetComponent<Unit>();
            unitScript.selectUnit();
            BuildingSelection.Instance.DeselectBuilding();
            updateInfoPanelForUnits();
            unitInfoPanel.SetActive(true);
        }


    }


    public void setGroundMarker(GameObject groundMarkerObject, Vector3 groundMarkerPosition)
    {
        groundMarkerObject.transform.position = groundMarkerPosition;
        groundMarkerObject.SetActive(false);
        groundMarkerObject.SetActive(true);
    }

    public void updateInfoPanelForUnits()
    {
        selectedUnitsTitle.text = "Unit(s) Selected";

        Dictionary<UnitSO.UnitType, int> unitTypeCounts = new Dictionary<UnitSO.UnitType, int>();

        foreach (var unit in unitsSelected)
        {
            UnitSO.UnitType unitType = unit.GetComponent<Unit>().unitSO.unitType;

            if (unitTypeCounts.ContainsKey(unitType))
            {
                unitTypeCounts[unitType]++;
            }
            else
            {
                unitTypeCounts[unitType] = 1;
            }
        }

        selectedUnitsListText.text = "";

        if (unitTypeCounts.Count > 1)
        {
            selectedUnitsListText.text += "Total #Units: " + unitsSelected.Count + "\n";
        }

        foreach (var kvp in unitTypeCounts)
        {
            selectedUnitsListText.text += $"{kvp.Key}s: {kvp.Value}\n";
        }
    }
}
