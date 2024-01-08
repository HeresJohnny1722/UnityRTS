using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the construction process of a building.
/// </summary>
public class BuildingConstruction : MonoBehaviour
{
    // Flag indicating if the building is under construction
    public bool isUnderConstruction = false;

    // UI elements for construction progress
    public TextMeshProUGUI constructionText;
    public Image constructionProgressSprite;

    // Materials for construction phases
    public Material blueConstructionMaterial;
    public Material yellowConstructingMaterial;
    private Material[] originalMaterials;

    // Reference to SquashAndStretch component
    public SquashAndStretch squashAndStretch;

    private float constructionTimer = 0f;
    private float target = 1f;

    // Reference to the associated building
    private Building building;

    // Array of mesh components for material adjustments
    public MeshRenderer[] meshComponents;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        building = this.GetComponent<Building>();
        squashAndStretch = this.GetComponent<SquashAndStretch>();
    }

    /// <summary>
    /// Updates the construction process if the building is under construction.
    /// </summary>
    public void UpdateConstruction()
    {
        if (isUnderConstruction)
        {
            ConstructBuilding();
        }
    }

    /// <summary>
    /// Progresses the construction of the building.
    /// </summary>
    public void ConstructBuilding()
    {
        constructionProgressSprite.transform.parent.parent.gameObject.SetActive(true);
        SetMaterial();

        constructionTimer += Time.deltaTime * 1;
        target = constructionTimer / building.buildingSO.timeToBuild;
        constructionProgressSprite.fillAmount = target;

        if (constructionTimer >= building.buildingSO.timeToBuild)
        {
            CompleteConstruction();
        }
    }

    /// <summary>
    /// Sets the construction materials based on the construction phase.
    /// </summary>
    public void SetMaterial()
    {
        BuildingManager buildingManager = gameObject.GetComponent<BuildingManager>();

        if (isUnderConstruction)
        {
            Material matToApply = blueConstructionMaterial;

            Material[] m;
            int nMaterials;
            foreach (MeshRenderer r in meshComponents)
            {
                nMaterials = buildingManager.initialMaterials[r].Count;
                m = new Material[nMaterials];
                for (int i = 0; i < nMaterials; i++)
                    m[i] = matToApply;
                r.sharedMaterials = m;
            }
        }
        // Set materials back to original when construction is complete
        else
        {
            foreach (MeshRenderer r in meshComponents)
                r.sharedMaterials = buildingManager.initialMaterials[r].ToArray();
        }
    }

    /// <summary>
    /// Completes the construction of the building.
    /// </summary>
    private void CompleteConstruction()
    {
        GameManager.instance.changeMaxPopulation(building.buildingSO.populationIncrease);

        Debug.Log("completed constructing");

        isUnderConstruction = false;
        SetMaterial();
        constructionTimer = 0f; // Reset the timer

        constructionProgressSprite.transform.parent.parent.gameObject.SetActive(false);

        BuildEffect();
    }

    /// <summary>
    /// Triggers a visual effect when the building is constructed.
    /// </summary>
    private void BuildEffect()
    {
        squashAndStretch.maximumScale = 1.5f;
        squashAndStretch.PlaySquashAndStretch();
        Vector3 effectPosition = new Vector3(building.transform.GetChild(1).position.x,
            building.transform.GetChild(1).localScale.y / 1.5f, building.transform.GetChild(1).position.z);
        GameObject deathEffect = Instantiate(building.buildingDeathEffect, effectPosition,
            building.buildingDeathEffect.transform.rotation);
        deathEffect.transform.localScale *= transform.GetComponent<BoxCollider>().size.x;
        Destroy(deathEffect, 2f);
    }
}
