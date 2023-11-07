using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Building : MonoBehaviour
{

    [SerializeField]
    private GameObject trainingPanel;
    [SerializeField]
    private GameObject infoPanel;

    public BuildingSO buildingSO;

    public void BuildingSelected()
    {
        Debug.Log("Selecting " + this.gameObject.name);
        HideShowBuildingStuff(true);
        infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingSO.name;
        infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buildingSO.description;
    }

    public void BuildingDeSelected()
    {
        Debug.Log("Deselecting " + this.gameObject.name);
        HideShowBuildingStuff(false);
    }

    void HideShowBuildingStuff(bool visible)
    {
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            trainingPanel.SetActive(visible);
        }
        infoPanel.SetActive(visible);
        this.transform.GetChild(0).gameObject.SetActive(visible);
        this.transform.GetChild(1).gameObject.SetActive(visible);
    }

    public void spawnTroop(int index)
    {

    }

    void Start()
    {
        trainingPanel.SetActive(false);
        infoPanel.SetActive(false);
        Selections.Instance.buildingsList.Add(this.gameObject);


    }

    void OnDestroy()
    {
        Selections.Instance.buildingsList.Remove(this.gameObject);
        trainingPanel.SetActive(false);
    }
}
