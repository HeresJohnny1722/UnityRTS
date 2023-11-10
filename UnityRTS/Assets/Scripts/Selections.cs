using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using TMPro;

public class Selections : MonoBehaviour
{

    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();
    public List<GameObject> buildingsList = new List<GameObject>();
    public Transform selectedBuilding = null;

    [SerializeField] private int maxGroupSize = 8;

    private static Selections _instance;
    public static Selections Instance { get { return _instance; } }

    

    private NavMeshAgent myAgent;
    

    [SerializeField] private GameObject groundMarker;

    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI selectedObjectTitleText;
    [SerializeField] private TextMeshProUGUI selectedObjectText;
    [SerializeField] private TextMeshProUGUI resourceDataText;

    private FormationBase _formation;

    public FormationBase Formation
    {
        get
        {
            if (_formation == null) _formation = GetComponent<FormationBase>();
            return _formation;
        }
        set => _formation = value;
    }

    private List<Vector3> _points = new List<Vector3>();

    public void moveUnits(Vector3 moveToPosition)
    {
        
        if (unitsSelected.Count > 0)
        {
            setGroundMarker(groundMarker, moveToPosition);
            NavMeshAgent leaderAgent = unitsSelected[0].GetComponent<NavMeshAgent>();
            leaderAgent.SetDestination(moveToPosition);

            _points = Formation.EvaluatePoints().ToList();

            for (var i = 1; i < unitsSelected.Count; i++)
            {
                myAgent = unitsSelected[i].GetComponent<NavMeshAgent>();
                myAgent.SetDestination(_points[i] + moveToPosition + new Vector3(-0.5f, 0, -0.5f));

            }

        }
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

    public void ClickSelectUnit(GameObject unitToAdd)
    {
        DeselectAll();
        unitsSelected.Add(unitToAdd);
        updateInfoPanelForUnits();
        infoPanel.SetActive(true);
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true);


        //barraksHandler.BarracksMenuClose();

    }

    public void ShiftClickSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
        {
            //barraksHandler.BarracksMenuClose();
            unitsSelected.Add(unitToAdd);
            updateInfoPanelForUnits();
            infoPanel.SetActive(true);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            unitToAdd.transform.GetChild(0).gameObject.SetActive(false);
            updateInfoPanelForUnits();
            unitsSelected.Remove(unitToAdd);
            if (unitsSelected.Count <= 0)
            {
                infoPanel.SetActive(false);
            }

        }
    }

    public void DragSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
        {
            //barraksHandler.BarracksMenuClose();
            unitsSelected.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            updateInfoPanelForUnits();
            infoPanel.SetActive(true);
        }
    }

    public void DeselectAll()
    {
        //barraksHandler.BarracksMenuClose();


        foreach (var unit in unitsSelected)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }
        unitsSelected.Clear();
        updateInfoPanelForUnits();
        infoPanel.SetActive(false);



        if (selectedBuilding)
        {
            
            selectedBuilding.parent.GetComponent<Building>().BuildingDeSelected();
            selectedBuilding = null;
        }
        

    }

    public void setGroundMarker(GameObject groundMarkerObject, Vector3 groundMarkerPosition)
    {
        groundMarkerObject.transform.position = groundMarkerPosition;
        groundMarkerObject.SetActive(false);
        groundMarkerObject.SetActive(true);
    }


    

    public void SelectBuilding(Transform buildingToSelect)
    {
        
        DeselectAll();
        selectedBuilding = buildingToSelect;
        
        selectedBuilding.parent.GetComponent<Building>().BuildingSelected();

        

    }

    public void updateInfoPanelForUnits()
    {
        selectedObjectTitleText.text = "Unit Group";
        resourceDataText.text = "";

        // Dictionary to store the counts of each unit type
        Dictionary<UnitSO.UnitType, int> unitTypeCounts = new Dictionary<UnitSO.UnitType, int>();

        // Iterate through selected units and count each type
        foreach (var unit in unitsSelected)
        {
            UnitSO.UnitType unitType = unit.GetComponent<Unit>().unitSO.unitType;

            // Update the count in the dictionary
            if (unitTypeCounts.ContainsKey(unitType))
            {
                unitTypeCounts[unitType]++;
            }
            else
            {
                unitTypeCounts[unitType] = 1;
            }
        }

        // Update the existing text in selectedObjectText
        selectedObjectText.text = "";

        // Check if there are two or more different types of troops selected
        if (unitTypeCounts.Count > 1)
        {
            selectedObjectText.text += "Total #Units: " + unitsSelected.Count + "\n";
        }

        // Display the counts for each unit type
        foreach (var kvp in unitTypeCounts)
        {
            selectedObjectText.text += $"{kvp.Key}s: {kvp.Value}\n";
        }
    }





    public void updateInfoPanelResource()
    {
        infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Resource";
    }

}
