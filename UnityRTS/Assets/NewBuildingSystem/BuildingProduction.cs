using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingProduction : MonoBehaviour
{
    private Building building;

    
    //public GameObject productionPanel;
    //public TextMeshProUGUI workersCurrentlyWorkingText;
    //public TextMeshProUGUI outputRateText;
    //public TextMeshProUGUI resourceTypeText;

    public GameObject nodePanel;
    public TextMeshProUGUI nodeGoldCostText;
    public TextMeshProUGUI nodeCoalCostText;
    public TextMeshProUGUI nodeCopperCostText;

    public GameObject Node;
    public GameObject ProductionBuilding;

    private float outputRate;

    private float productionTimer = 0f;
    private float productionInterval = 1f; // Set the production interval to 1 second

    private void Awake()
    {
        building = this.GetComponent<Building>();
        productionInterval = building.buildingSO.outputTime;
    }

    public void UpdateProductionUI()
    {
        //workersCurrentlyWorkingText.text = building.workersInside.Count + "/" + building.buildingSO.workerCapacity.ToString() + " Workers";
       // outputRateText.text = "Output rate: " + outputRate + " " + building.buildingSO.resourceType.ToString() + "/second";
       // resourceTypeText.text = building.buildingSO.resourceType.ToString();
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
       // building.removeButton.SetActive(false);
//        building.buildingHealthText.text = "";
        Node.SetActive(true);
        ProductionBuilding.SetActive(false);
    }

    private void SetStageTwoProperties()
    {
       // building.removeButton.SetActive(true);
        Node.SetActive(false);
        ProductionBuilding.SetActive(true);
    }

    public void UpdateProduction()
    {
        if (building.buildingSO.buildingType == BuildingSO.BuildingType.Production && !building.buildingConstruction.isUnderConstruction && building.stage == 2)
        {
            Debug.Log("Producing: " + building.buildingSO.resourceType);
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
        HideShowProduction(true);

        if (building.stage == 1)
        {
            building.buildingNameText.text = building.buildingSO.nodeName;
            //building.infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = building.buildingSO.nodeDescription;
            if (building.buildingSO.buildingType == BuildingSO.BuildingType.Production)
            {
                nodeGoldCostText.text = "Gold: " + building.buildingSO.goldCost.ToString();
                nodeCoalCostText.text = "Wood: " + building.buildingSO.woodCost.ToString();
                nodeCopperCostText.text = "Food: " + building.buildingSO.foodCost.ToString();
                nodePanel.SetActive(true);
            }
            //building.buildingHealthText.text = "";
            building.removeButton.SetActive(false);

        }
        else if (building.stage == 2)
        {
            building.buildingNameText.text = building.buildingSO.name;
            building.productionOutputRateText.text = "Output: " + building.buildingSO.outputPerInterval.ToString() + " " + building.buildingSO.resourceType + "/" + productionInterval + " second(s)";
            nodePanel.SetActive(false);
            

        }


    }

    public void DeselectProduction()
    {
        HideShowProduction(false);
    }

    public void HideShowProduction(bool visible)
    {
        //productionPanel.SetActive(visible);
        nodePanel.SetActive(visible);
    }

    public void ProduceResource()
    {
        if (building.buildingSO.buildingType == BuildingSO.BuildingType.Production)// && building.workersInside.Count > 0)
        {
            float outputRate = building.buildingSO.outputPerInterval;//building.workersInside.Count * building.buildingSO.outputWorkerMultiplyer;
            //Debug.Log(outputRate);
            //buildingSO.resourceOutputRate *

            //workersCurrentlyWorkingText.text = building.workersInside.Count + "/" + building.buildingSO.workerCapacity.ToString() + " Workers";
            //outputRateText.text = "Output rate: " + outputRate + " " + building.buildingSO.resourceType.ToString() + "/second";
            //resourceTypeText.text = building.buildingSO.resourceType.ToString();

            // Update your resource quantity here (assuming you have an InventoryManager)
            // You can replace "InventoryManager.instance" with your actual reference to the InventoryManager
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



    public void nodeToBuilding()
    {
        if (GameManager.instance.AreResourcesAvailable(0, (int)building.buildingSO.goldCost, (int)building.buildingSO.woodCost, (int)building.buildingSO.foodCost, 0))
        {
            building.stage = 2;
            building.buildingConstruction.isUnderConstruction = true;
            if (building.buildingSO.resourceType == BuildingSO.ResourceType.Energy)
            {
               // building.removeButton.SetActive(false);
            }
            else
            {
             //   building.removeButton.SetActive(true);
            }

            if (!BuildingSelection.Instance.buildingsList.Contains(building.gameObject))
            {
                BuildingSelection.Instance.buildingsList.Add(building.gameObject);
            }

            GameManager.instance.RemoveResources(0, (int)building.buildingSO.goldCost, (int)building.buildingSO.woodCost, (int)building.buildingSO.foodCost, 0);

            //building.buildingHealthText.text = "Health: " + building.buildingHealth.ToString();
            Node.SetActive(false);
            ProductionBuilding.SetActive(true);
            //building.productionPanel.SetActive(true);
            nodePanel.SetActive(false);
            //Make it cost resources
            building.infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.buildingSO.name;
            building.infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = building.buildingSO.description;

        }
    }

    public void buildingToNode()
    {


//        building.removeButton.SetActive(false);
        building.buildingHealthbar.gameObject.SetActive(false);
        building.buildingHealth = building.buildingSO.startingHealth;
        building.buildingHealthbar.UpdateHealthBar(building.buildingSO.startingHealth, building.buildingHealth);
        //building.buildingHealthbar.gameObject.SetActive(false);
        building.stage = 1;
        //building.buildingHealthText.text = "";
        Node.SetActive(true);
        ProductionBuilding.SetActive(false);
        //productionPanel.SetActive(false);
        nodePanel.SetActive(true);
        BuildingSelection.Instance.buildingsList.Remove(building.gameObject);
        building.infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.buildingSO.nodeName;
        building.infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = building.buildingSO.nodeDescription;
        building.buildingConstruction.isUnderConstruction = false;
        building.buildingConstruction.constructionProgressSprite.transform.parent.parent.gameObject.SetActive(false);


    }

}
