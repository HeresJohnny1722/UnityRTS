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

    public SquashAndStretch squashAndStretch;

    private float constructionTimer = 0f;
    private float target = 1f;

    private Building building;

    public MeshRenderer[] meshComponents;

    public bool useWorkers = false;


    private void Awake()
    {
        building = this.GetComponent<Building>();
        squashAndStretch = this.GetComponent<SquashAndStretch>();
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
        //Sets the construction progress bar canvas to true
        constructionProgressSprite.transform.parent.parent.gameObject.SetActive(true);

        //Gets set to construction material
        SetMaterial();


        constructionTimer += Time.deltaTime * 1;

        target = constructionTimer / building.buildingSO.timeToBuild;



        constructionProgressSprite.fillAmount = target;

        if (constructionTimer >= building.buildingSO.timeToBuild)
        {
            CompleteConstruction();
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

        //Sets the construction progress bar canvas to true
        constructionProgressSprite.transform.parent.parent.gameObject.SetActive(true);
    }

    private void CompleteConstruction()
    {
        GameManager.instance.changeMaxPopulation(building.buildingSO.populationIncrease);

        isUnderConstruction = false;
        SetMaterial();
        constructionTimer = 0f; // Reset the timer
        constructionPanel.SetActive(false);

        //Sets the construction progress bar canvas to false
        constructionProgressSprite.transform.parent.parent.gameObject.SetActive(false);
        squashAndStretch.maximumScale = 1.5f;
        squashAndStretch.PlaySquashAndStretch();
        BuildEffect();

    }

    private void BuildEffect()
    {
        Vector3 effectPosition = new Vector3(building.transform.GetChild(1).position.x, building.transform.GetChild(1).localScale.y / 1.5f, building.transform.GetChild(1).position.z);
        GameObject deathEffect = Instantiate(building.buildingDeathEffect, effectPosition, building.buildingDeathEffect.transform.rotation);
        deathEffect.transform.localScale *= transform.GetComponent<BoxCollider>().size.x;
        Destroy(deathEffect, 2f);
    }

}
