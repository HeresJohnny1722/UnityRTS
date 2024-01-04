using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private void SetStageOneProperties()
    {
        Node.SetActive(true);
        ProductionBuilding.SetActive(false);
    }

    private void SetStageTwoProperties()
    {
        Node.SetActive(false);
        ProductionBuilding.SetActive(true);
    }

    public void UpdateProduction()
    {
        if (building.buildingSO.buildingType == BuildingSO.BuildingType.Production && !building.buildingConstruction.isUnderConstruction && building.stage == 2)
        {
            productionTimer += Time.deltaTime;

            if (productionTimer >= productionInterval)
            {
                ProduceResource();
                productionTimer = 0f; // Reset the timer
            }
        }
    }

    public void SelectProduction()
    {

        if (building.stage == 2)
        {
            building.buildingNameText.text = building.buildingSO.name;
            building.productionOutputRateText.text = "Output: " + building.buildingSO.outputPerInterval.ToString() + " " + building.buildingSO.resourceType + "/" + productionInterval + " second(s)";
            nodePanel.SetActive(false);

        }
    }

    public void ProduceResource()
    {
        if (building.buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            float outputRate = building.buildingSO.outputPerInterval;

            if (building.buildingSO.resourceType == BuildingSO.ResourceType.Gold)
            {
                GameManager.instance.AddResources(0, (int)outputRate, 0, 0, 0);

            }
            else if (building.buildingSO.resourceType == BuildingSO.ResourceType.Wood)
            {
                GameManager.instance.AddResources(0, 0, (int)outputRate, 0, 0);

            }
            else if (building.buildingSO.resourceType == BuildingSO.ResourceType.Food)
            {
                GameManager.instance.AddResources(0, 0, 0, (int)outputRate, 0);

            }
            else if (building.buildingSO.resourceType == BuildingSO.ResourceType.Energy)
            {
                GameManager.instance.AddResources(0, 0, 0, 0, (int)outputRate);
            }


        }
    }

}
