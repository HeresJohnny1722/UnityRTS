using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelection : MonoBehaviour
{
    public List<GameObject> buildingsList = new List<GameObject>();
    public Transform selectedBuilding = null;

    private static BuildingSelection instance;
    public static BuildingSelection Instance { get { return instance; } }

    private Building building;

    [SerializeField] BuildingGridPlacer buildingGridPlacer;

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

    public void TakeDamageBuildingTest(float damage)
    {
        if (selectedBuilding != null)
        {
            selectedBuilding.GetComponent<Building>()?.TakeDamage(damage);
        }
    }

    public void SelectBuilding(Transform buildingToSelect)
    {
        DeselectBuilding();

        if (buildingGridPlacer._buildingPrefab != null)
        {
            Debug.Log("Sorry, you're in building mode");
        }
        else
        {
            building = buildingToSelect.GetComponent<Building>();

            UnitSelection.Instance.DeselectAll();
            DeselectBuilding();
            selectedBuilding = buildingToSelect;

            building?.BuildingSelected();
        }
    }

    public void DeselectBuilding()
    {
        building?.DeselectBuilding();
        selectedBuilding = null;
    }

    public void MoveFlag(Vector3 point)
    {
        building?.buildingTraining?.MoveFlag(point);
    }
}
