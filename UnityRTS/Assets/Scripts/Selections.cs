using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class Selections : MonoBehaviour
{

    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();
    public List<GameObject> buildingsList = new List<GameObject>();
    public Transform selectedBuilding = null;

    [SerializeField] private int maxGroupSize = 8;

    private static Selections _instance;
    public static Selections Instance { get { return _instance; } }

    [SerializeField]
    private BarracksHandler barracksHandler;

    private NavMeshAgent myAgent;
    

    [SerializeField] private GameObject groundMarker;

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
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true);


        //barraksHandler.BarracksMenuClose();

    }

    public void ShiftClickSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
        {
            //barraksHandler.BarracksMenuClose();
            unitsSelected.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            unitToAdd.transform.GetChild(0).gameObject.SetActive(false);
            unitsSelected.Remove(unitToAdd);

        }
    }

    public void DragSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd) && (unitsSelected.Count < maxGroupSize))
        {
            //barraksHandler.BarracksMenuClose();
            unitsSelected.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
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
        //barraksHandler.BarracksMenuClose();
        DeselectAll();
        selectedBuilding = buildingToSelect;
        
        selectedBuilding.parent.GetComponent<Building>().BuildingSelected();

        /*
        Debug.Log(buildingToSelect.name);
        selectedBuilding = buildingToSelect;
        selectedBuilding.GetChild(3).gameObject.SetActive(false);
        selectedBuilding.GetChild(4).gameObject.SetActive(true);
        selectedBuilding.GetChild(2).gameObject.SetActive(true);
        */
        //selectedBuilding.GetChild(0).gameObject.SetActive(true);
        //change material to white outline


        //selectedBuilding.GetChild(2).gameObject.SetActive(true);
        //Open up a training/reserch menu/ just some kind of UI
        //ActionFrame.instance.SetActionButtons();
        //barraksHandler.BarracksMenuOpen();


        //unitToAdd.transform.GetChild(0).gameObject.SetActive(true);


    }
    
}
