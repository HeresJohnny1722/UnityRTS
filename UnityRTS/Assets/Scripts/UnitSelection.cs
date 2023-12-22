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

    private static UnitSelection instance;
    public static UnitSelection Instance { get { return instance; } }

    [SerializeField] private float maxGroupSize = 8;

    [SerializeField] private GameObject groundMarker;
    [SerializeField] private GameObject unitInfoPanel;

    [SerializeField] private TextMeshProUGUI selectedUnitsTitle;
    [SerializeField] private TextMeshProUGUI selectedUnitsListText;

    private AstarAI myAstarAI;

    private Unit unitScript;

    private FormationBase _formationBase;

    public FormationBase Formation
    {
        get
        {
            if (_formationBase == null) _formationBase = GetComponent<FormationBase>();
            return _formationBase;
        }
        private set => _formationBase = value;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private List<Vector3> _points = new List<Vector3>();

    public void MoveUnits(Vector3 moveToPosition)
    {
        if (unitsSelected.Count > 0 && Formation != null)
        {
            SetGroundMarker(groundMarker, moveToPosition);

            _points = Formation.EvaluatePoints().ToList();

            for (int i = 0; i < unitsSelected.Count; i++)
            {
                myAstarAI = unitsSelected[i]?.GetComponent<AstarAI>();
                if (myAstarAI != null)
                {
                    myAstarAI.ai.destination = _points[i] + moveToPosition;
                }
            }
        }
    }

    public void TakeDamageUnitTest(float damage)
    {
        for (int i = 0; i < unitsSelected.Count; i++)
        {
            var unit = unitsSelected[i]?.GetComponent<Unit>();
            if (unit != null)
            {
                unit.takeDamage(damage);
            }
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
            unitScript = unitToAdd?.GetComponent<Unit>();
            if (unitScript != null)
            {
                unitScript.deselectUnit();
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
            unitScript = unit?.GetComponent<Unit>();
            if (unitScript != null)
            {
                unitScript.deselectUnit();
            }
        }
        unitsSelected.Clear();
    }

    public void SelectUnit(GameObject unitToSelect)
    {
        if (unitToSelect?.activeSelf == true)
        {
            unitsSelected.Add(unitToSelect);
            unitScript = unitToSelect.GetComponent<Unit>();
            if (unitScript != null)
            {
                unitScript.selectUnit();
                BuildingSelection.Instance.DeselectBuilding();
            }
        }
    }

    public void SetGroundMarker(GameObject groundMarkerObject, Vector3 groundMarkerPosition)
    {
        groundMarkerObject.transform.position = groundMarkerPosition;
        groundMarkerObject.SetActive(false);
        groundMarkerObject.SetActive(true);
    }
}
