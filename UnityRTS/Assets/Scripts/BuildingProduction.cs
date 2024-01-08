using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the production functionality of a building, managing resource generation over time.
/// </summary>
public class BuildingProduction : MonoBehaviour
{
    private Building building;

    public GameObject nodePanel;
    public GameObject Node;
    public GameObject ProductionBuilding;

    public TextMeshProUGUI nodeGoldCostText;
    public TextMeshProUGUI nodeCoalCostText;
    public TextMeshProUGUI nodeCopperCostText;

    private float productionTimer = 0f;
    private float productionInterval = 1f;

    private void Awake()
    {
        building = this.GetComponent<Building>();
        productionInterval = building.buildingSO.outputTime;
    }

    /// <summary>
    /// Sets up the production properties based on the building's stage.
    /// </summary>
    public void SetUpProduction()
    {
        if (building.stage == 1)
        {
            SetStageOneProperties();
        }
        else if (building.stage == 2)
        {
            SetStageTwoProperties();
        }
    }

    /// <summary>
    /// Sets properties for the building in stage one.
    /// </summary>
    private void SetStageOneProperties()
    {
        Node.SetActive(true);
        ProductionBuilding.SetActive(false);
    }

    /// <summary>
    /// Sets properties for the building in stage two.
    /// </summary>
    private void SetStageTwoProperties()
    {
        Node.SetActive(false);
        ProductionBuilding.SetActive(true);
    }

    /// <summary>
    /// Updates the production process, triggering resource generation based on the interval.
    /// </summary>
    public void UpdateProduction()
    {
        if (building.buildingSO.buildingType == BuildingSO.BuildingType.Production &&
            !building.buildingConstruction.isUnderConstruction &&
            building.stage == 2)
        {
            productionTimer += Time.deltaTime;

            if (productionTimer >= productionInterval)
            {
                ProduceResource();
                productionTimer = 0f; // Reset the timer
            }
        }
    }

    /// <summary>
    /// Displays production-related information when the building is selected.
    /// </summary>
    public void SelectProduction()
    {
        if (building.stage == 2)
        {
            building.buildingNameText.text = building.buildingSO.name;
            building.productionOutputRateText.text = "Output: " +
                building.buildingSO.outputPerInterval.ToString() +
                " " + building.buildingSO.resourceType +
                "/" + productionInterval + " second(s)";
            nodePanel.SetActive(false);
        }
    }

    /// <summary>
    /// Produces resources based on the building's output rate and resource type.
    /// </summary>
    public void ProduceResource()
    {
        if (building.buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            float outputRate = building.buildingSO.outputPerInterval;

            switch (building.buildingSO.resourceType)
            {
                case BuildingSO.ResourceType.Gold:
                    GameManager.instance.AddResources(0, (int)outputRate, 0, 0, 0);
                    break;

                case BuildingSO.ResourceType.Wood:
                    GameManager.instance.AddResources(0, 0, (int)outputRate, 0, 0);
                    break;

                case BuildingSO.ResourceType.Food:
                    GameManager.instance.AddResources(0, 0, 0, (int)outputRate, 0);
                    break;

                case BuildingSO.ResourceType.Energy:
                    GameManager.instance.AddResources(0, 0, 0, 0, (int)outputRate);
                    break;
            }
        }
    }
}
