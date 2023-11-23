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
            }
        }
    }

    public void moveWorkersIntoBuilding(Transform buildingToEnter)
    {
        Building building = buildingToEnter.GetComponent<Building>();

        if (unitsSelected.Count > 0 && building.stage == 2)
        {
            // Highlight the resource node

            // If the building is a production building
            //Check if the building is being constructed
            

            if (building.isUnderConstruction)
            {

                foreach (var unitSelected in unitsSelected)
                {
                    Unit unit = unitSelected.GetComponent<Unit>();

                    if (unit.unitSO.unitType == UnitSO.UnitType.Worker)
                    {

                        //Start coroutine
                        if (building.buildingSO.workerCapacity > building.workersCurrentlyInTheBuilding)
                        {
                            building.workersCurrentlyInTheBuilding++;
                            StartCoroutine(MoveWorkerToBuilding(unitSelected, buildingToEnter));
                        }

                    }
                }

            } else if (building.buildingSO.buildingType == BuildingSO.BuildingType.Production)
            {
                
                foreach (var unitSelected in unitsSelected)
                {
                    Unit unit = unitSelected.GetComponent<Unit>();

                    if (unit.unitSO.unitType == UnitSO.UnitType.Worker)
                    {

                        // Start coroutine to check for collision with the buildingworkersCurrentlyInTheBuildingc
                        if (buildingToEnter.GetComponent<Building>().buildingSO.workerCapacity > buildingToEnter.GetComponent<Building>().workersCurrentlyInTheBuilding) {
                            buildingToEnter.GetComponent<Building>().workersCurrentlyInTheBuilding++;
                            StartCoroutine(MoveWorkerToBuilding(unitSelected, buildingToEnter));
                        }
                        
                            
                            
                        
                        
                    }
                }
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

    private IEnumerator MoveWorkerToBuilding(GameObject worker, Transform buildingToEnter)
    {
        myAgent = worker.GetComponent<NavMeshAgent>();
        float radius = 1f;

        NavMesh.SamplePosition(buildingToEnter.position, out NavMeshHit hit, radius, NavMesh.AllAreas);

        // Set destination for the agent to the closest point on the nav mesh
        myAgent.SetDestination(hit.position);

        // Wait for the worker to collide with the building
        yield return StartCoroutine(WaitForProximityToBuilding(worker, buildingToEnter));

        if (buildingToEnter.GetComponent<Building>().workersCurrentlyWorking.Count < buildingToEnter.GetComponent<Building>().buildingSO.workerCapacity)
        {
            // Additional logic after reaching the destination (if needed)
            // ...

            // Worker has collided with the building, do something
            Debug.Log("Worker has collided with the production building!");
            //Do all the add the unit to the building list stuff
            //play a glowing unit animation
            //update the progress bar on the building
            if (worker.GetComponent<Unit>().unitSO.unitType == UnitSO.UnitType.Worker)
            {
                // Start coroutine to check for collision with the building
                
                    
                    buildingToEnter.GetComponent<Building>().addWorker(worker);
                    worker.SetActive(false);
                    unitScript = worker.GetComponent<Unit>();
                    unitScript.deselectUnit();
                    UnitSelection.Instance.unitsSelected.Remove(worker);
                    UnitSelection.Instance.unitList.Remove(worker);
                    updateInfoPanelForUnits();
                    //Destroy(worker);
                        
                
                

                
            }
        } else
        {
            float spawnRadius = 1.5f; // Set your desired radius here

            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * spawnRadius;
            Vector3 newPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

            myAgent.SetDestination(newPosition);

            // Additional logic or messages if needed
            Debug.Log("Worker capacity reached. Moving to a random spot around the building.");
        }
        
    }

    private IEnumerator WaitForProximityToBuilding(GameObject worker, Transform buildingToEnter)
    {
        while (true)
        {
            float distance = Vector3.Distance(worker.transform.position, buildingToEnter.position);
            proximityRadius = buildingToEnter.GetComponent<Building>().buildingSO.workerEnterRadius;
            if (distance <= proximityRadius)
            {
                // Worker is within the specified radius of the building
                yield break;
            }
            yield return null;
        }
    }


    public void UnitEnterBuilding(GameObject unitAddingIntoBuilding)
    {
        unitAddingIntoBuilding.SetActive(false);
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
