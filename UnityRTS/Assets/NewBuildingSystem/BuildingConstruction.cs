using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingConstruction : MonoBehaviour
{
    public bool isUnderConstruction = false;
    public GameObject constructionPanel;
    public TextMeshProUGUI constructionText;
    public Image constructionProgressSprite;

    public Material blueConstructionMaterial;
    public Material yellowConstructingMaterial;
    private Material[] originalMaterials;

    private float constructionTimer = 0f;
    private float target = 1f;

    private Building building;

    public MeshRenderer[] meshComponents;


    private void Awake()
    {
        building = this.GetComponent<Building>();
    }

    public void UpdateConstruction()
    {
        if (isUnderConstruction)
        {
            ConstructBuilding();
        }
    }

    public void ConstructBuilding()
    {
        SetMaterial();

        if (building.workersInside.Count > 0)
        {
            //Debug.Log("is constructing");



            constructionTimer += Time.deltaTime * building.workersInside.Count;
            constructionText.text = "Workers currently constructing: " + building.workersInside.Count.ToString() + "/" + building.buildingSO.constructionCapacity;

            target = constructionTimer / building.buildingSO.timeToBuild;


            float speed = 0.001f;
            constructionProgressSprite.fillAmount = target;

            if (constructionTimer >= building.buildingSO.timeToBuild)
            {
                CompleteConstruction();
            }

            SetMaterial();
        }

    }

    public void SetMaterial()
    {
        BuildingManager buildingManager = gameObject.GetComponent<BuildingManager>();
        // Construction material
        if (isUnderConstruction)
        {
            if (building.workersInside.Count > 0)
            {
                Material matToApply = yellowConstructingMaterial;

                Material[] m; int nMaterials;
                foreach (MeshRenderer r in meshComponents)
                {
                    nMaterials = buildingManager.initialMaterials[r].Count;
                    m = new Material[nMaterials];
                    for (int i = 0; i < nMaterials; i++)
                        m[i] = matToApply;
                    r.sharedMaterials = m;
                }
            }
            else
            {
                Material matToApply = blueConstructionMaterial;

                Material[] m; int nMaterials;
                foreach (MeshRenderer r in meshComponents)
                {
                    nMaterials = buildingManager.initialMaterials[r].Count;
                    m = new Material[nMaterials];
                    for (int i = 0; i < nMaterials; i++)
                        m[i] = matToApply;
                    r.sharedMaterials = m;
                }
            }

        }
        // Set materials back to original when construction is complete
        else
        {
            foreach (MeshRenderer r in meshComponents)
                r.sharedMaterials = buildingManager.initialMaterials[r].ToArray();
        }
    }

    public void showConstructionPanel()
    {
        constructionPanel.SetActive(true);
    }

    private void CompleteConstruction()
    {
        InventoryManager.instance.changeMaxPopulation(building.buildingSO.populationIncrease);
        Debug.Log("completed constructing");
        building.removeAllWorkers();
        isUnderConstruction = false;
        SetMaterial();
        constructionTimer = 0f; // Reset the timer
        constructionPanel.SetActive(false);

        //Change the material of the building

        // Add any additional logic for a fully constructed building here
    }

}
