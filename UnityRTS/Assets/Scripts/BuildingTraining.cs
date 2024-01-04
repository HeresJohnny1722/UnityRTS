using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BuildingTraining : MonoBehaviour
{

    private Building building;

    public GameObject barracksSpawnFlag;
    public GameObject barracksPanel;
    public GameObject barracksButtonLayout;

    public Image trainingProgressSprite;

    public TextMeshProUGUI barracksTrainingTimeLeftText;
    public TextMeshProUGUI barracacksQueueSizeText;
    public TextMeshProUGUI barracksUnitTrainingNameText;

    public float unitFlagOffset;
    public float baracksStartFlagRadius = 5f;

    public Transform unitSpawnPoint;
    public Transform unitMovePoint;

    public Queue<UnitSO> troopQueue = new Queue<UnitSO>();

    public bool isTraining = false;

    [HideInInspector]
    public Vector3 movePosition;

    private void Awake()
    {
        building = this.GetComponent<Building>();
    }

    private void Update()
    {
        if (!building.buildingConstruction.isUnderConstruction)
            return;

        HideShowBarracks(false);
    }

    public void BarracksSelected()
    {
        //Sets the text of each button in the training menu to the correct name of the corresponding unit
        for (int i = 0; i < building.buildingSO.unitsToTrain.Count; i++)
        {
            barracksButtonLayout.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = building.buildingSO.unitsToTrain[i].name;
        }

        HideShowBarracks(true);
    }

    public void BarracksDeselected()
    {
        HideShowBarracks(false);
    }

    public void HideShowBarracks(bool visible)
    {
        barracksPanel.SetActive(visible);

        if (building.buildingConstruction.isUnderConstruction)
        {
            barracksSpawnFlag.SetActive(false);
        } else
        {
            barracksSpawnFlag.SetActive(visible);
        }
        
    }

    /// <summary>
    /// Sets the flag position to a certain position within 2 meters from the center of the building, purpose is for the flag to not overlap the barracks
    /// </summary>
    public void SetFlagStartPosition()
    {

        unitFlagOffset = building.buildingSO.spawnFlagMoveToTroopOffset;

        // Adjust the distance value to set the minimum distance between the barracks and the spawn flag
        float minDistance = 2.0f;
        LayerMask buildingLayerMask = LayerMask.GetMask("Building"); // Assuming "Building" is the layer name

        Vector3 randomOffset = new Vector3(Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius), 0, Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius));

        // Calculate the new position with a minimum distance from the barracks
        Vector3 newPosition = transform.GetChild(1).position + Vector3.one + randomOffset;
        float distance = Vector3.Distance(newPosition, transform.GetChild(1).position);

        while (distance < minDistance || Physics.Raycast(newPosition, Vector3.down, 1.0f, buildingLayerMask))
        {
            randomOffset = new Vector3(Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius), 0, Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius));
            newPosition = transform.GetChild(1).position + Vector3.one + randomOffset;
            distance = Vector3.Distance(newPosition, transform.GetChild(1).position);
        }

        barracksSpawnFlag.transform.position = new Vector3(newPosition.x, barracksSpawnFlag.transform.position.y, newPosition.z);

    }

    public void moveFlag(Vector3 point)
    {
        
            barracksSpawnFlag.transform.position = point;
        
    }

    public void spawnTroop(int index)
    {
        if (GameManager.instance.AreResourcesAvailable((int)building.buildingSO.unitsToTrain[index].populationCost * 3, (int)building.buildingSO.unitsToTrain[index].goldCost, (int)building.buildingSO.unitsToTrain[index].coalCost, (int)building.buildingSO.unitsToTrain[index].copperCost, 0))
        {
            GameManager.instance.RemoveResources(0, (int)building.buildingSO.unitsToTrain[index].goldCost, (int)building.buildingSO.unitsToTrain[index].coalCost, (int)building.buildingSO.unitsToTrain[index].copperCost, 0);
            GameManager.instance.changeCurrentPopulation((int)building.buildingSO.unitsToTrain[index].populationCost * 3);
            unitSpawnPoint = this.transform.GetChild(2).transform;
            unitMovePoint = barracksSpawnFlag.transform;


            UpdateQueueSizeText();
            troopQueue.Enqueue(building.buildingSO.unitsToTrain[index]);
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
        UpdateQueueSizeText();
        isTraining = true;

        while (troopQueue.Count > 0)
        {
            UpdateQueueSizeText();
            UnitSO unit = troopQueue.Dequeue();
            UpdateQueueSizeText();
            barracksUnitTrainingNameText.text = unit.name;

            float trainingTime = unit.trainingTime;

            while (trainingTime > 0)
            {

                barracksTrainingTimeLeftText.text = "Training Time: " + trainingTime.ToString("0") + "s";

                yield return new WaitForSeconds(1);

                trainingTime -= 1;
                UpdateQueueSizeText();

            }

            for (int i = 0; i < 3; i++)
            {
                GameObject troop = Instantiate(unit.prefab, unitSpawnPoint.position, Quaternion.identity);

                bool isOccupied;

                do
                {
                    // Generate a new random position
                    movePosition = new Vector3(unitMovePoint.position.x + Random.Range(-unitFlagOffset, unitFlagOffset), 0, unitMovePoint.position.z + Random.Range(-unitFlagOffset, unitFlagOffset));

                    // Check if there's already a troop within the specified radius
                    isOccupied = IsPositionOccupied(movePosition, troop.transform.localScale.x);
                } while (isOccupied);


                UpdateQueueSizeText();

                // Reset time left text and unit name
                barracksTrainingTimeLeftText.text = "Training Time: 0s";
                barracksUnitTrainingNameText.text = "No unit training";

                UpdateQueueSizeText(); // Update the queue size text when a troop is done training
                troop.GetComponent<Unit>().moveToPosition = movePosition;
                AstarAI myAstarAI = troop.GetComponent<AstarAI>();
                myAstarAI.ai.destination = movePosition;
            }
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
        float queueCount = troopQueue.Count + 1;
        barracacksQueueSizeText.text = "Queue Size: " + queueCount;
    }
}
