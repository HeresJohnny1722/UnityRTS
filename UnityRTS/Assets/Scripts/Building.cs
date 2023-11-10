using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    [Header("Only for Barracks/Artillery Training")]
    [SerializeField]
    private GameObject buttonLayout;
    [SerializeField]
    private GameObject barracksPanel;
    [SerializeField]
    public TextMeshProUGUI timeLeftText;
    public TextMeshProUGUI queueSizeText;
    public TextMeshProUGUI unitNameText;
    
    


    [Space(20)]
    [Header("General Building Stuff")]
    [SerializeField]
    private GameObject infoPanel;
    public TextMeshProUGUI buildingHealthText;
    private float buildingHealth;

    public BuildingSO buildingSO;

    private Transform unitSpawnPoint;
    private Transform unitMovePoint;
    
    private Queue<UnitSO> troopQueue = new Queue<UnitSO>();
    private bool isTraining = false;

    private float unitFlagOffset;





    public void BuildingSelected()
    {
        Debug.Log("Selecting " + this.gameObject.name);
        HideShowBuildingStuff(true);
        infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingSO.name;
        infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buildingSO.description;


        //Barracks
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            for (int i = 0; i < buildingSO.unitsToTrain.Count; i++)
            {
                //setting tool tips also
                buttonLayout.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingSO.unitsToTrain[i].name;
            }
        }
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
            buttonLayout.SetActive(visible);
            barracksPanel.SetActive(visible);
            this.transform.GetChild(1).gameObject.SetActive(visible);
        }
        infoPanel.SetActive(visible);
        this.transform.GetChild(0).gameObject.SetActive(visible);
        
    }

    public void spawnTroop(int index)
    {
        Debug.Log(buildingSO.unitsToTrain[index]);
        unitSpawnPoint = this.transform.GetChild(2).transform;
        unitMovePoint = this.transform.GetChild(1).transform;

        //if (checkIfEnoughResources(resources that are being taken, would be in the buildingSO, cost
        troopQueue.Enqueue(buildingSO.unitsToTrain[index]);
        UpdateQueueSizeText();

        if (!isTraining)
        {
           
            StartCoroutine(TrainTroops());
        }


    }

    private IEnumerator TrainTroops()
    {
        
        isTraining = true;

        while (troopQueue.Count > 0)
        {
            
            UnitSO unit = troopQueue.Dequeue();

            unitNameText.text = unit.name;
            Debug.Log("Training " + unit.name);


            float trainingTime = unit.trainingTime;
            
            while (trainingTime > 0)
            {
                
                timeLeftText.text = "Training Time: " + trainingTime.ToString("0") + "s";
                yield return new WaitForSeconds(1);
                trainingTime -= 1;
                UpdateQueueSizeText();
            }


            
            Vector3 movePosition = new Vector3(unitMovePoint.position.x + Random.Range(-unitFlagOffset, unitFlagOffset), 0, unitMovePoint.position.z + Random.Range(-unitFlagOffset, unitFlagOffset));

            
            GameObject troop = Instantiate(unit.prefab, unitSpawnPoint.position, Quaternion.identity);
            // Reset time left text and unit name
            timeLeftText.text = "Training Time: 0s";
            unitNameText.text = "No unit training";

            UpdateQueueSizeText(); // Update the queue size text when a troop is done training
            NavMeshAgent unitAgent = troop.GetComponent<NavMeshAgent>();
            unitAgent.SetDestination(movePosition);


            
        }


        isTraining = false;
        UpdateQueueSizeText(); // Update the queue size text when all troops are done training
    }

    private void UpdateQueueSizeText()
    {
        queueSizeText.text = "Queue Size: " + troopQueue.Count;
    }

    void Start()
    {
        HideShowBuildingStuff(false);
        buildingHealth = buildingSO.startingHealth;
        buildingHealthText.text = "Health: " + buildingHealth.ToString();
        //Selections.Instance.buildingsList.Add(this.gameObject);
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            unitFlagOffset = buildingSO.spawnOffset;
        }
        



}

void OnDestroy()
    {
        //Selections.Instance.buildingsList.Remove(this.gameObject);
        HideShowBuildingStuff(false);
    }
}
