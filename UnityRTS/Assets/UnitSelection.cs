using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using TMPro;

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
            }
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
        unitsSelected.Add(unitToAdd);
        BuildingSelection.Instance.DeselectBuilding();
        //updateInfoPanelForUnits();
        //infoPanel.SetActive(true);
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
        unitToAdd.transform.GetChild(1).gameObject.SetActive(true);

    }

    public void ShiftClickSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
        {
            BuildingSelection.Instance.DeselectBuilding();
            unitsSelected.Add(unitToAdd);
            //updateInfoPanelForUnits();
            //infoPanel.SetActive(true);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            unitToAdd.transform.GetChild(0).gameObject.SetActive(false);
            unitToAdd.transform.GetChild(1).gameObject.SetActive(false);
            //updateInfoPanelForUnits();
            unitsSelected.Remove(unitToAdd);
            if (unitsSelected.Count <= 0)
            {
                //infoPanel.SetActive(false);
            }
        }
    }

    public void DragSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
        {
            unitsSelected.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.transform.GetChild(1).gameObject.SetActive(true);
            BuildingSelection.Instance.DeselectBuilding();
            //updateInfoPanelForUnits();
            //infoPanel.SetActive(true);
        }
    }

    public void DeselectAll()
    {

        foreach (var unit in unitsSelected)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
            unit.transform.GetChild(1).gameObject.SetActive(false);
        }
        unitsSelected.Clear();
        //unitsSelected.Clear();

    }

    public void SelectUnit(GameObject unitToSelect)
    {
        //updateInfoPanelForUnits();
        //unitInfoPanel.SetActive(true);
        BuildingSelection.Instance.DeselectBuilding();
        unitScript = unitToSelect.GetComponent<Unit>();
        unitScript.selectUnit();
        
        
    }

    public void DeselectUnit(GameObject unitToDeselect)
    {
        //updateInfoPanelForUnits();
        unitInfoPanel.SetActive(false);
        unitScript.deselectUnit();
        
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
