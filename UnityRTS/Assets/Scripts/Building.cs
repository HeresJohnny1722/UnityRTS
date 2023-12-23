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

    public BuildingDefense buildingDefense;

    [Header("General Building")]


    public BuildingSO buildingSO;

    public float buildingHealth;
    public float stage;

    public GameObject buildingDeathEffect;

    public GameObject infoPanel;
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI productionOutputRateText;
    public GameObject removeButton;

    public Healthbar buildingHealthbar;
    //public TextMeshProUGUI buildingHealthText;

    public List<GameObject> workersInside = new List<GameObject>();



    void Start()
    {

        InitializeBuilding();

    }

    private void InitializeBuilding()
    {

        stage = buildingSO.stage;
        InitalizeHealth();
        BuildingUIVisibility(false);


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


        //buildingHealthText.text = "Health: " + buildingHealth.ToString();


    }


    private void Update()
    {
        if (buildingProduction != null)
        {
            buildingProduction.UpdateProduction();
        }

        buildingConstruction.UpdateConstruction();


    }


    public void takeDamage(float damageAmount)
    {
        if (stage != 1)
        {
            buildingConstruction.squashAndStretch.maximumScale = .85f;
            buildingConstruction.squashAndStretch.PlaySquashAndStretch();

            buildingHealth -= damageAmount;

            if (buildingHealth <= 0)
            {
                removeBuilding();
                //InventoryManager.instance.RemoveResources(0, (int)buildingSO.goldCost, (int)buildingSO.woodCost, (int)buildingSO.foodCost, 0);
            }

            //buildingHealthText.text = "Health: " + buildingHealth.ToString();
            //BuildingUIVisibility(true);
            buildingHealthbar.gameObject.SetActive(true);
            buildingHealthbar.UpdateHealthBar(buildingSO.startingHealth, buildingHealth);
        }

    }

    public void CancelBuilding()
    {
        DeathEffect();
        SoundFeedback.Instance.PlaySound(SoundType.Remove);
        deselectBuilding();
        BuildingSelection.Instance.buildingsList.Remove(this.gameObject);
        Destroy(this.gameObject);
        Debug.Log("Hello");
    }
    
    public void removeBuilding()
    {

        //removeAllWorkers();

        if (buildingSO.buildingType == BuildingSO.BuildingType.Production && buildingSO.resourceType == BuildingSO.ResourceType.Energy)
        {


            DeathEffect();
            //Play like a destroying building sound effect
            buildingProduction.buildingToNode();


        }
        else
        {
            //deselectBuilding();
            DeathEffect();
            SoundFeedback.Instance.PlaySound(SoundType.Remove);
            
            BuildingSelection.Instance.buildingsList.Remove(this.gameObject);
            Destroy(this.gameObject);
            GameManager.instance.decreaseBuildingCount(buildingSO);
            AstarPath.active.UpdateGraphs(this.GetComponent<BoxCollider>().bounds);


            if (!buildingConstruction.isUnderConstruction)
            {
                GameManager.instance.changeMaxPopulation(-buildingSO.populationIncrease);
                GameManager.instance.AddResources(0, Mathf.RoundToInt(buildingSO.goldCost * (buildingHealth/buildingSO.startingHealth)), Mathf.RoundToInt(buildingSO.woodCost * (buildingHealth / buildingSO.startingHealth)), Mathf.RoundToInt(buildingSO.foodCost * (buildingHealth / buildingSO.startingHealth)), 0);
                
            } else
            {
                if (buildingSO.startingHealth  * 0.75 < buildingHealth )
                {
                    GameManager.instance.AddResources(0, buildingSO.goldCost, buildingSO.woodCost, buildingSO.foodCost, 0);
                }
                
            }

            if (buildingSO.buildingType == BuildingSO.BuildingType.Townhall)
            {
                //Game Over
                GameManager.instance.GameOver();
            }

        }



    }

    

    private void DeathEffect()
    {
        Vector3 effectPosition = new Vector3(transform.GetChild(1).position.x, transform.GetChild(1).localScale.y / 2, transform.GetChild(1).position.z);
        GameObject deathEffect = Instantiate(buildingDeathEffect, effectPosition, buildingDeathEffect.transform.rotation);
        deathEffect.transform.localScale *= transform.GetComponent<BoxCollider>().size.x;
        Destroy(deathEffect, 2f);
    }

    public void BuildingSelected()
    {
        removeButton.SetActive(true);

        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            buildingProduction.UpdateProductionUI();
            buildingProduction.SelectProduction();
        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            buildingTraining.BarracksSelected();
            productionOutputRateText.text = "";
        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Townhall)
        {
            removeButton.SetActive(false);
        }
        else
        {
            productionOutputRateText.text = "";
        }

        BuildingUIVisibility(true);

        if (buildingSO.startingHealth == buildingHealth)
        {
            buildingHealthbar.gameObject.SetActive(false);

        }
        else
        {
            buildingHealthbar.gameObject.SetActive(true);
        }

        buildingNameText.text = buildingSO.name;

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
            buildingConstruction.showConstructionPanel();
        }

        infoPanel.SetActive(visible);
        if (buildingSO.startingHealth == buildingHealth)
        {
            buildingHealthbar.gameObject.SetActive(false);

        }
        else
        {
            buildingHealthbar.gameObject.SetActive(true);
        }
        this.transform.GetChild(0).gameObject.SetActive(visible);

    }




}
