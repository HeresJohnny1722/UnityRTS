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
    [SerializeField] private float baracksStartFlagRadius = 5f;
    [SerializeField] private GameObject barracksPanel;
    [SerializeField] private GameObject barracksButtonLayout;


    [SerializeField] private TextMeshProUGUI barracksTrainingTimeLeftText;
    [SerializeField] private TextMeshProUGUI barracacksQueueSizeText;
    [SerializeField] private TextMeshProUGUI barracksUnitTrainingNameText;

    [Header("Production buildings")]
    public List<GameObject> workersCurrentlyWorking = new List<GameObject>();
    [SerializeField] private GameObject productionPanel;
    [SerializeField] private TextMeshProUGUI workersCurrentlyWorkingText;
    [SerializeField] private TextMeshProUGUI outputRateText;
    [SerializeField] private TextMeshProUGUI resourceTypeText;

    [SerializeField] private GameObject nodePanel;
    [SerializeField] private TextMeshProUGUI nodeGoldCostText;
    [SerializeField] private TextMeshProUGUI nodeCoalCostText;
    [SerializeField] private TextMeshProUGUI nodeCopperCostText;

    public GameObject Node;
    public GameObject ProductionBuilding;

    private float outputRate;
    [HideInInspector]
    public float workersCurrentlyInTheBuilding = 0;


    [Space(20)]
    [Header("General Building Stuff")]

    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Healthbar buildingHealthbar;

    [SerializeField] private TextMeshProUGUI buildingHealthText;
    public BuildingSO buildingSO;
    public GameObject removeButton;


    private float buildingHealth;
    private float unitFlagOffset;

    private Transform unitSpawnPoint;
    private Transform unitMovePoint;

    private Queue<UnitSO> troopQueue = new Queue<UnitSO>();
    private bool isTraining = false;

    public float stage;

    void Start()
    {
        InventoryManager.instance.populationCap += buildingSO.populationIncrease;
        //InventoryManager.instance.UpdateTextFields();




        removeButton.SetActive(true);
        HideShowBuildingStuff(false);
        buildingHealth = buildingSO.startingHealth;
        buildingHealthbar.UpdateHealthBar(buildingSO.startingHealth, buildingHealth);
        buildingHealthText.text = "Health: " + buildingHealth.ToString();
        BuildingSelection.Instance.buildingsList.Add(this.gameObject);
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            unitFlagOffset = buildingSO.spawnFlagMoveToTroopOffset;

            // Adjust the distance value to set the minimum distance between the barracks and the spawn flag
            float minDistance = 1.0f;
            LayerMask buildingLayerMask = LayerMask.GetMask("Building"); // Assuming "Building" is the layer name

            Vector3 randomOffset = new Vector3(Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius), 0, Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius));

            // Calculate the new position with a minimum distance from the barracks
            Vector3 newPosition = transform.position + Vector3.one + randomOffset;
            float distance = Vector3.Distance(newPosition, transform.position);

            while (distance < minDistance || Physics.Raycast(newPosition, Vector3.down, 1.0f, buildingLayerMask))
            {
                randomOffset = new Vector3(Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius), 0, Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius));
                newPosition = transform.position + Vector3.one + randomOffset;
                distance = Vector3.Distance(newPosition, transform.position);
            }

            barracksSpawnFlag.transform.position = new Vector3(newPosition.x, barracksSpawnFlag.transform.position.y, newPosition.z);


            //barracksSpawnFlag.transform.position = new Vector3((transform.position.x + 1) + Random.Range(-baracksStartRadius, baracksStartRadius), barracksSpawnFlag.transform.position.y, (transform.position.z + 1) + Random.Range(-baracksStartRadius, baracksStartRadius));
        }

        //NavmeshManager.Instance.UpdateNavmesh();

        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            stage = buildingSO.stage;
            if (stage == 1)
            {
                removeButton.SetActive(false);
                buildingHealthText.text = "";
                Node.SetActive(true);
                ProductionBuilding.SetActive(false);
            }
            else if (stage == 2)
            {
                removeButton.SetActive(true);
                Node.SetActive(false);
                ProductionBuilding.SetActive(true);
            }
        }

        


    }

    public void moveFlag(Vector3 point)
    {
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {

            barracksSpawnFlag.transform.position = point;

        }
    }

    public void removeAllWorkers()
    {
        if (workersCurrentlyWorking.Count > 0)
        {
            float radius = 3f; // Set your desired radius here
            int workerCounter = (int) workersCurrentlyInTheBuilding;

            for (int i = 0; i < workerCounter; i++)
            {
                Vector2 randomPoint = Random.insideUnitCircle * radius;
                Vector3 newPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

                workersCurrentlyWorking[i].transform.position = newPosition;
                workersCurrentlyWorking[i].SetActive(true);

                UnitSelection.Instance.unitList.Add(workersCurrentlyWorking[i]);
                
            }
            workersCurrentlyWorking.Clear();


            workersCurrentlyInTheBuilding = 0;

            //Update the production stuff, capacity and output
        }

        //Production
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            workersCurrentlyWorkingText.text = workersCurrentlyWorking.Count + "/" + buildingSO.workerCapacity.ToString() + " Workers";

        }

    }

    public void removeOneWorker()
    {
        if (workersCurrentlyWorking.Count > 0)
        {
            float radius = 3f; // Set your desired radius here

            Vector2 randomPoint = Random.insideUnitCircle * radius;
            Vector3 newPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

            workersCurrentlyWorking[0].transform.position = newPosition;
            workersCurrentlyWorking[0].SetActive(true);

            UnitSelection.Instance.unitList.Add(workersCurrentlyWorking[0]);
            workersCurrentlyWorking.Remove(workersCurrentlyWorking[0]);

            workersCurrentlyInTheBuilding--;

            //Update the production stuff, capacity and output
        }

        //Production
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            workersCurrentlyWorkingText.text = workersCurrentlyWorking.Count + "/" + buildingSO.workerCapacity.ToString() + " Workers";

        }
    }

    public void addWorker(GameObject worker)
    {
        workersCurrentlyWorking.Add(worker);


        //Production
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            workersCurrentlyWorkingText.text = workersCurrentlyWorking.Count + "/" + buildingSO.workerCapacity.ToString() + " Workers";
            outputRateText.text = "Output rate: " + outputRate + " " + buildingSO.resourceType.ToString() + "/second";
            resourceTypeText.text = buildingSO.resourceType.ToString();
        }

    }

    public void produceResource()
    {
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production && workersCurrentlyWorking.Count > 0)
        {
            float outputRate = workersCurrentlyWorking.Count * buildingSO.outputWorkerMultiplyer;
            Debug.Log(outputRate);
            //buildingSO.resourceOutputRate *

            workersCurrentlyWorkingText.text = workersCurrentlyWorking.Count + "/" + buildingSO.workerCapacity.ToString() + " Workers";
            outputRateText.text = "Output rate: " + outputRate + " " + buildingSO.resourceType.ToString() + "/second";
            resourceTypeText.text = buildingSO.resourceType.ToString();

            // Update your resource quantity here (assuming you have an InventoryManager)
            // You can replace "InventoryManager.instance" with your actual reference to the InventoryManager
            if (buildingSO.resourceType == BuildingSO.ResourceType.Gold)
            {
                InventoryManager.instance.AddResources(0, (int)outputRate, 0, 0, 0);

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Coal)
            {
                InventoryManager.instance.AddResources(0, 0, (int)outputRate, 0, 0);

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Copper)
            {
                InventoryManager.instance.AddResources(0, 0, 0, (int)outputRate, 0);

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Energy)
            {
                InventoryManager.instance.AddResources(0, 0, 0, 0, (int)outputRate);
            }


        }
    }

    private float productionTimer = 0f;
    private float productionInterval = 1f; // Set the production interval to 1 second

    private void Update()
    {
        productionTimer += Time.deltaTime;

        if (productionTimer >= productionInterval)
        {
            produceResource();
            productionTimer = 0f; // Reset the timer
        }
    }


    public void takeDamage(float damageAmount)
    {
        if (stage == 1)
        {
            buildingHealthText.text = "";
        }
        else
        {




            buildingHealth -= damageAmount;

            if (buildingHealth <= 0)
            {

                if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
                {
                    removeAllWorkers();
                    if (buildingSO.resourceType == BuildingSO.ResourceType.Energy)
                    {
                        
                        removeButton.SetActive(false);
                        buildingToNode();
                        buildingHealth = buildingSO.startingHealth;
                        buildingHealthbar.UpdateHealthBar(buildingSO.startingHealth, buildingHealth);
                    }
                    else
                    {
                        //Sound for destroying building
                        //SoundFeedback.Instance.PlaySound(SoundType.Remove);
                        removeButton.SetActive(true);
                        deselectBuilding();

                        BuildingSelection.Instance.buildingsList.Remove(this.gameObject);
                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    InventoryManager.instance.populationCap += buildingSO.populationIncrease;
                    InventoryManager.instance.UpdateTextFields();


                    deselectBuilding();
                    //SoundFeedback.Instance.PlaySound(SoundType.Remove);
                    BuildingSelection.Instance.buildingsList.Remove(this.gameObject);
                    Destroy(this.gameObject);
                }


            }
            buildingHealthText.text = "Health: " + buildingHealth.ToString();
            HideShowBuildingStuff(true);
            buildingHealthbar.UpdateHealthBar(buildingSO.startingHealth, buildingHealth);
        }


    }

    public void removeBuilding()
    {
        if (stage == 1)
        {
            //buildingHealthText.text = "";
        }
        else
        {


            SoundFeedback.Instance.PlaySound(SoundType.Remove);
            buildingHealth -= buildingHealth;
            InventoryManager.instance.AddResources(0, (int)buildingSO.goldCost, (int)buildingSO.coalCost, (int)buildingSO.copperCost, 0);

            //InventoryManager.instance.RemoveResources((int)buildingSO.populationIncrease,0, 0, 0, 0);
            InventoryManager.instance.populationCap -= buildingSO.populationIncrease;

            InventoryManager.instance.UpdateTextFields();

            if (buildingHealth <= 0)
            {

                deselectBuilding();

                BuildingSelection.Instance.buildingsList.Remove(this.gameObject);
                Destroy(this.gameObject);

            }

        }


    }

    public void BuildingSelected()
    {
        //Production
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            workersCurrentlyWorkingText.text = workersCurrentlyWorking.Count + "/" + buildingSO.workerCapacity.ToString() + " Workers";
            outputRateText.text = "Output rate: " + outputRate + " " + buildingSO.resourceType.ToString() + "/second";
            resourceTypeText.text = buildingSO.resourceType.ToString();
        }
        HideShowBuildingStuff(true);
        if (buildingSO.startingHealth == buildingHealth)
        {
            buildingHealthbar.gameObject.SetActive(false);

        }

        if (stage == 1)
        {
            infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingSO.nodeName;
            infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buildingSO.nodeDescription;
            if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
            {
                nodeGoldCostText.text = "Gold: " + buildingSO.goldCost.ToString();
                nodeCoalCostText.text = "Coal: " + buildingSO.coalCost.ToString();
                nodeCopperCostText.text = "Copper: " + buildingSO.copperCost.ToString();
                nodePanel.SetActive(true);
            }
            buildingHealthText.text = "";

        }
        else if (stage == 2)
        {
            infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingSO.name;
            infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buildingSO.description;
            if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
            {
                nodePanel.SetActive(false);
            }

        }



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
        if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            nodePanel.SetActive(false);

        }

        HideShowBuildingStuff(false);

    }

    public void nodeToBuilding()
    {
        if (InventoryManager.instance.AreResourcesAvailable(0, (int)buildingSO.goldCost, (int)buildingSO.coalCost, (int)buildingSO.copperCost, 0))
        {
            if (buildingSO.resourceType == BuildingSO.ResourceType.Energy)
            {
                removeButton.SetActive(false);
            }
            else
            {
                removeButton.SetActive(true);
            }

            InventoryManager.instance.RemoveResources(0, (int)buildingSO.goldCost, (int)buildingSO.coalCost, (int)buildingSO.copperCost, 0);
            stage = 2;
            buildingHealthText.text = "Health: " + buildingHealth.ToString();
            Node.SetActive(false);
            ProductionBuilding.SetActive(true);
            productionPanel.SetActive(true);
            nodePanel.SetActive(false);
            //Make it cost resources
            infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingSO.name;
            infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buildingSO.description;

        }
    }

    public void buildingToNode()
    {

        
        removeButton.SetActive(false);

        buildingHealthbar.gameObject.SetActive(false);
        stage = 1;
        buildingHealthText.text = "";
        Node.SetActive(true);
        ProductionBuilding.SetActive(false);
        productionPanel.SetActive(false);
        nodePanel.SetActive(true);
        
        infoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingSO.nodeName;
        infoPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buildingSO.nodeDescription;


    }

    void HideShowBuildingStuff(bool visible)
    {
        if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            barracksButtonLayout.SetActive(visible);
            barracksPanel.SetActive(visible);
            barracksSpawnFlag.SetActive(visible);
        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {
            if (stage == 2)
            {
                productionPanel.SetActive(visible);
            }

        }
        infoPanel.SetActive(visible);
        buildingHealthbar.gameObject.SetActive(visible);
        this.transform.GetChild(0).gameObject.SetActive(visible);

    }

    public void spawnTroop(int index)
    {
        if (InventoryManager.instance.AreResourcesAvailable((int)buildingSO.unitsToTrain[index].populationCost, (int)buildingSO.unitsToTrain[index].goldCost, (int)buildingSO.unitsToTrain[index].coalCost, (int)buildingSO.unitsToTrain[index].copperCost, 0))
        {
            InventoryManager.instance.RemoveResources(0, (int)buildingSO.unitsToTrain[index].goldCost, (int)buildingSO.unitsToTrain[index].coalCost, (int)buildingSO.unitsToTrain[index].copperCost, 0);
            unitSpawnPoint = this.transform.GetChild(2).transform;
            unitMovePoint = barracksSpawnFlag.transform;

            //if (checkIfEnoughResources(resources that are being taken, would be in the buildingSO, cost
            troopQueue.Enqueue(buildingSO.unitsToTrain[index]);
            UpdateQueueSizeText();

            if (!isTraining)
            {

                StartCoroutine(TrainTroops());
            }
        }
        else
        {
            Debug.Log("Not enough resources");
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



            //Vector3 movePosition = new Vector3(unitMovePoint.position.x + Random.Range(-unitFlagOffset, unitFlagOffset), 0, unitMovePoint.position.z + Random.Range(-unitFlagOffset, unitFlagOffset));




            Vector3 movePosition;
            bool isOccupied;

            do
            {
                // Generate a new random position
                movePosition = new Vector3(unitMovePoint.position.x + Random.Range(-unitFlagOffset, unitFlagOffset), 0, unitMovePoint.position.z + Random.Range(-unitFlagOffset, unitFlagOffset));

                // Check if there's already a troop within the specified radius
                isOccupied = IsPositionOccupied(movePosition, unit.prefab.gameObject.transform.localScale.x / 2 + 1 / 16);
            } while (isOccupied);

            // Use the new movePosition for your logic


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

    bool IsPositionOccupied(Vector3 position, float radius)
    {
        foreach (GameObject unit in UnitSelection.Instance.unitList)
        {
            // Check the distance from the unit to the specified position
            float distance = Vector3.Distance(unit.transform.position, position);

            // If the distance is within the radius, the position is considered occupied
            if (distance < radius)
            {
                return true;
            }
        }

        // No units found within the radius
        return false;
    }


    private void UpdateQueueSizeText()
    {
        barracacksQueueSizeText.text = "Queue Size: " + troopQueue.Count;
    }


}
