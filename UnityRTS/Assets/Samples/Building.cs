using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    
    public BuildingTraining buildingTraining;
    
    public BuildingProduction buildingProduction;
   
    public BuildingConstruction buildingConstruction;

    [Header("General Building")]

    public BuildingSO buildingSO;

    public float buildingHealth;
    public float stage;

    public GameObject infoPanel;
    public GameObject removeButton;

    public Healthbar buildingHealthbar;
    public TextMeshProUGUI buildingHealthText;

    public List<GameObject> workersInside = new List<GameObject>();



    void Start()
    {
        
        InitializeBuilding();

    }

    private void InitializeBuilding()
    {

        stage = buildingSO.stage;
        BuildingUIVisibility(false);
        InitalizeHealth();

        BuildingSelection.Instance.buildingsList.Add(this.gameObject);

        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            buildingTraining.SetFlagStartPosition();
            buildingTraining.BarracksDeselected();
        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            buildingProduction.SetUpProduction();
        }

    }

    private void InitalizeHealth()
    {
        buildingHealth = buildingSO.startingHealth;
        buildingHealthbar.UpdateHealthBar(buildingSO.startingHealth, buildingHealth);
        buildingHealthText.text = "Health: " + buildingHealth.ToString();
    }

    private void SetStageOneProperties()
    {
        removeButton.SetActive(false);
        buildingHealthText.text = "";
        buildingProduction.Node.SetActive(true);
        buildingProduction.ProductionBuilding.SetActive(false);
    }

    private void SetStageTwoProperties()
    {
        removeButton.SetActive(true);
        buildingProduction.Node.SetActive(false);
        buildingProduction.ProductionBuilding.SetActive(true);
    }

    private void Update()
    {
        if (buildingProduction != null)
        {
            buildingProduction.UpdateProduction();
        }
        if (buildingConstruction != null)
        {
            buildingConstruction.UpdateConstruction();
        }

    }

    public void removeAllWorkers()
    {
        if (workersInside.Count > 0)
        {
            float radius = 3f; // Set your desired radius here
            int workerCounter = (int)workersInside.Count;

            for (int i = 0; i < workerCounter; i++)
            {
                Vector2 randomPoint = Random.insideUnitCircle * radius;
                Vector3 newPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

                workersInside[i].transform.position = newPosition;
                workersInside[i].SetActive(true);

                UnitSelection.Instance.unitList.Add(workersInside[i]);

            }
            workersInside.Clear();


            //buildingProduction.workersCurrentlyInTheBuilding = 0;

            //Update the production stuff, capacity and output
        }

        //Production
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            buildingProduction.UpdateProductionUI();
        }

    }

    public void removeOneWorker()
    {
        if (workersInside.Count > 0)
        {
            float radius = 3f; // Set your desired radius here

            Vector2 randomPoint = Random.insideUnitCircle * radius;
            Vector3 newPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

            workersInside[0].transform.position = newPosition;
            workersInside[0].SetActive(true);

            UnitSelection.Instance.unitList.Add(workersInside[0]);
            workersInside.Remove(workersInside[0]);

            //buildingProduction.workersCurrentlyInTheBuilding--;

            //Update the production stuff, capacity and output
        }

        //Production
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            buildingProduction.UpdateProductionUI();
        }
    }

    public void addWorker(GameObject worker)
    {
        workersInside.Add(worker);


        //Production
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            buildingProduction.UpdateProductionUI();
        }

    }

    public void takeDamage(float damageAmount)
    {
        buildingHealth -= damageAmount;

        if (buildingHealth <= 0)
        {
            removeBuilding();
        }

        buildingHealthText.text = "Health: " + buildingHealth.ToString();
        BuildingUIVisibility(true);
        buildingHealthbar.UpdateHealthBar(buildingSO.startingHealth, buildingHealth);


    }

    public void removeBuilding()
    {

        removeAllWorkers();

        if (buildingSO.buildingType == BuildingSO.BuildingType.Production && buildingSO.resourceType == BuildingSO.ResourceType.Energy)
        {
            //Play like a destroying building sound effect
            buildingProduction.buildingToNode();

        }
        else
        {

            SoundFeedback.Instance.PlaySound(SoundType.Remove);
            deselectBuilding();
            BuildingSelection.Instance.buildingsList.Remove(this.gameObject);
            Destroy(this.gameObject);
            InventoryManager.instance.decreaseBuildingCount(buildingSO);
            InventoryManager.instance.AddResources(0, (int)buildingSO.goldCost, (int)buildingSO.coalCost, (int)buildingSO.copperCost, 0);

            if (!buildingConstruction.isUnderConstruction)
            {
                InventoryManager.instance.changeMaxPopulation(-buildingSO.populationIncrease);
            }

        }

        

    }

    public void BuildingSelected()
    {
        //Production
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            buildingProduction.UpdateProductionUI();
            buildingProduction.SelectProduction();
        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            buildingTraining.BarracksSelected();
        }

        BuildingUIVisibility(true);

        if (buildingSO.startingHealth == buildingHealth)
        {
            buildingHealthbar.gameObject.SetActive(false);

        }

        if (!buildingConstruction.isUnderConstruction)
        {
            buildingConstruction.constructionPanel.SetActive(false);
        }
        else
        {
            removeButton.SetActive(false);
        }

    }

    public void deselectBuilding()
    {
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            buildingProduction.DeselectProduction();


        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            buildingTraining.BarracksDeselected();
        }

        BuildingUIVisibility(false);
        buildingConstruction.constructionPanel.SetActive(false);

    }



    void BuildingUIVisibility(bool visible)
    {

        if (buildingConstruction.isUnderConstruction)
        {
            buildingConstruction.constructionPanel.SetActive(true);
        }

        infoPanel.SetActive(visible);
        buildingHealthbar.gameObject.SetActive(visible);
        this.transform.GetChild(0).gameObject.SetActive(visible);

    }




}
