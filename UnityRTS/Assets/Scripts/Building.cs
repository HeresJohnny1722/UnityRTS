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
    public GameObject removeButton;

    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI productionOutputRateText;

    public Healthbar buildingHealthbar;

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
        if (stage == 1)
            return;

        buildingConstruction.squashAndStretch.maximumScale = .95f;
        buildingConstruction.squashAndStretch.PlaySquashAndStretch();

        buildingHealth -= damageAmount;

        if (buildingHealth <= 0)
        {
            removeBuilding();
        }

        buildingHealthbar.gameObject.SetActive(true);
        buildingHealthbar.UpdateHealthBar(buildingSO.startingHealth, buildingHealth);

    }

    public void CancelConstruction()
    {
        DeathEffect();
        SoundFeedback.Instance.PlaySound(SoundType.Remove);
        deselectBuilding();
        BuildingSelection.Instance.buildingsList.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

    public void removeBuilding()
    {

        DeathEffect();
            

            BuildingSelection.Instance.buildingsList.Remove(this.gameObject);
            
            GameManager.instance.decreaseBuildingCount(buildingSO);
            AstarPath.active.UpdateGraphs(this.GetComponent<BoxCollider>().bounds);

        if (buildingSO.buildingType == BuildingSO.BuildingType.Townhall)
        {
            //Game Over
            GameManager.instance.GameOver();
        }

        if (!buildingConstruction.isUnderConstruction)
            {
                GameManager.instance.changeMaxPopulation(-buildingSO.populationIncrease);
                GameManager.instance.AddResources(0, Mathf.RoundToInt(buildingSO.goldCost * (buildingHealth / buildingSO.startingHealth)), Mathf.RoundToInt(buildingSO.woodCost * (buildingHealth / buildingSO.startingHealth)), Mathf.RoundToInt(buildingSO.foodCost * (buildingHealth / buildingSO.startingHealth)), 0);

            }
            else
            {
                if (buildingSO.startingHealth * 0.75 < buildingHealth)
                {
                    GameManager.instance.AddResources(0, buildingSO.goldCost, buildingSO.woodCost, buildingSO.foodCost, 0);
                }

            }

        Destroy(this.gameObject);

    }

    private void DeathEffect()
    {
        SoundFeedback.Instance.PlaySound(SoundType.Remove);
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
            productionOutputRateText.text = "";
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
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            buildingTraining.BarracksDeselected();
        }

        BuildingUIVisibility(false);

    }

    void BuildingUIVisibility(bool visible)
    {

        infoPanel.SetActive(visible);

        if (buildingSO.startingHealth == buildingHealth)
        {
            buildingHealthbar.gameObject.SetActive(false);

        }
        else
        {
            buildingHealthbar.gameObject.SetActive(true);
        }

        //Shows the white building highlight
        this.transform.GetChild(0).gameObject.SetActive(visible);

    }
}
