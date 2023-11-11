using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    [Header("Only for Barracks/Artillery Training")]

    [SerializeField] private GameObject barracksSpawnFlag;
    [SerializeField] private GameObject barracksPanel;
    [SerializeField] private GameObject barracksButtonLayout;
    

    [SerializeField] private TextMeshProUGUI barracksTrainingTimeLeftText;
    [SerializeField] private TextMeshProUGUI barracacksQueueSizeText;
    [SerializeField] private TextMeshProUGUI barracksUnitTrainingNameText;

    [Space(20)]
    [Header("General Building Stuff")]

    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Healthbar buildingHealthbar;

    [SerializeField] private TextMeshProUGUI buildingHealthText;
    [SerializeField] private BuildingSO buildingSO;


    private float buildingHealth;
    private float unitFlagOffset;

    private Transform unitSpawnPoint;
    private Transform unitMovePoint;

    private Queue<UnitSO> troopQueue = new Queue<UnitSO>();
    private bool isTraining = false;


    void Start()
    {
        HideShowBuildingStuff(false);
        buildingHealth = buildingSO.startingHealth;
        buildingHealthbar.UpdateHealthBar(buildingSO.startingHealth, buildingHealth);
        buildingHealthText.text = "Health: " + buildingHealth.ToString();
        BuildingSelection.Instance.buildingsList.Add(this.gameObject);
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            unitFlagOffset = buildingSO.spawnOffset;
        }
    }

    public void moveFlag(Vector3 point)
    {
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {

            barracksSpawnFlag.transform.position = point;
            
        }
    }

    public void takeDamage(float damageAmount)
    {


        buildingHealth -= damageAmount;

        if (buildingHealth <= 0)
        {
            deselectBuilding();
            
            BuildingSelection.Instance.buildingsList.Remove(this.gameObject);
            Destroy(this.gameObject);

        }
        buildingHealthText.text = "Health: " + buildingHealth.ToString();
        HideShowBuildingStuff(true);
        buildingHealthbar.UpdateHealthBar(buildingSO.startingHealth, buildingHealth);
    }

    public void BuildingSelected()
    {
        
        HideShowBuildingStuff(true);
        if (buildingSO.startingHealth == buildingHealth)
        {
            buildingHealthbar.gameObject.SetActive(false);

        }
        infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingSO.name;
        infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buildingSO.description;


        //Barracks
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            for (int i = 0; i < buildingSO.unitsToTrain.Count; i++)
            {
                //setting tool tips also
                barracksButtonLayout.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingSO.unitsToTrain[i].name;
            }
        }
    }

    public void deselectBuilding()
    {
        
        HideShowBuildingStuff(false);
        
    }

    void HideShowBuildingStuff(bool visible)
    {
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            barracksButtonLayout.SetActive(visible);
            barracksPanel.SetActive(visible);
            barracksSpawnFlag.SetActive(visible);
        }
        infoPanel.SetActive(visible);
        buildingHealthbar.gameObject.SetActive(visible);
        this.transform.GetChild(0).gameObject.SetActive(visible);

    }

    public void spawnTroop(int index)
    {
        
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

            barracksUnitTrainingNameText.text = unit.name;
            


            float trainingTime = unit.trainingTime;

            while (trainingTime > 0)
            {

                barracksTrainingTimeLeftText.text = "Training Time: " + trainingTime.ToString("0") + "s";
                yield return new WaitForSeconds(1);
                trainingTime -= 1;
                UpdateQueueSizeText();
            }



            Vector3 movePosition = new Vector3(unitMovePoint.position.x + Random.Range(-unitFlagOffset, unitFlagOffset), 0, unitMovePoint.position.z + Random.Range(-unitFlagOffset, unitFlagOffset));


            GameObject troop = Instantiate(unit.prefab, unitSpawnPoint.position, Quaternion.identity);
            // Reset time left text and unit name
            barracksTrainingTimeLeftText.text = "Training Time: 0s";
            barracksUnitTrainingNameText.text = "No unit training";

            UpdateQueueSizeText(); // Update the queue size text when a troop is done training
            NavMeshAgent unitAgent = troop.GetComponent<NavMeshAgent>();
            unitAgent.SetDestination(movePosition);



        }


        isTraining = false;
        UpdateQueueSizeText(); // Update the queue size text when all troops are done training
    }

    private void UpdateQueueSizeText()
    {
        barracacksQueueSizeText.text = "Queue Size: " + troopQueue.Count;
    }


}
