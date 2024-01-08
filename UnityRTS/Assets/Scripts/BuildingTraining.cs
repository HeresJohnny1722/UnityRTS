using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Manages building training functionality, including spawning troops and updating training progress.
/// </summary>
public class BuildingTraining : MonoBehaviour
{
    /// <summary>
    /// Reference to the Building component of the building.
    /// </summary>
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

    /// <summary>
    /// Initializes the BuildingTraining component.
    /// </summary>
    private void Awake()
    {
        building = this.GetComponent<Building>();
    }

    /// <summary>
    /// Updates the building training functionality.
    /// </summary>
    private void Update()
    {
        if (!building.buildingConstruction.isUnderConstruction)
            return;

        HideShowBarracks(false);
    }

    /// <summary>
    /// Handles the selection of the barracks.
    /// </summary>
    public void BarracksSelected()
    {
        for (int i = 0; i < building.buildingSO.unitsToTrain.Count; i++)
        {
            barracksButtonLayout.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = building.buildingSO.unitsToTrain[i].name;
        }

        HideShowBarracks(true);
    }

    /// <summary>
    /// Handles the deselection of the barracks.
    /// </summary>
    public void BarracksDeselected()
    {
        HideShowBarracks(false);
    }

    /// <summary>
    /// Shows or hides the barracks panel and spawn flag.
    /// </summary>
    public void HideShowBarracks(bool visible)
    {
        barracksPanel.SetActive(visible);

        if (building.buildingConstruction.isUnderConstruction)
        {
            barracksSpawnFlag.SetActive(false);
        }
        else
        {
            barracksSpawnFlag.SetActive(visible);
        }
    }

    /// <summary>
    /// Sets the initial position for the unit spawn flag.
    /// </summary>
    public void SetFlagStartPosition()
    {
        unitFlagOffset = building.buildingSO.spawnFlagMoveToTroopOffset;

        float minDistance = 2.0f;
        LayerMask buildingLayerMask = LayerMask.GetMask("Building");

        Vector3 randomOffset = new Vector3(Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius), 0, Random.Range(-baracksStartFlagRadius, baracksStartFlagRadius));
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

    /// <summary>
    /// Moves the troop spawn flag to the specified position.
    /// </summary>
    public void moveFlag(Vector3 point)
    {
        barracksSpawnFlag.transform.position = point;
    }

    /// <summary>
    /// Initiates the training of a troop at the specified index.
    /// </summary>
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

    /// <summary>
    /// Coroutine for training troops in the barracks.
    /// </summary>
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
                troop.GetComponent<Unit>().SetupHealth();

                bool isOccupied;

                do
                {
                    movePosition = new Vector3(unitMovePoint.position.x + Random.Range(-unitFlagOffset, unitFlagOffset), 0, unitMovePoint.position.z + Random.Range(-unitFlagOffset, unitFlagOffset));
                    isOccupied = IsPositionOccupied(movePosition, troop.transform.localScale.x);
                } while (isOccupied);

                UpdateQueueSizeText();
                barracksTrainingTimeLeftText.text = "Training Time: 0s";
                barracksUnitTrainingNameText.text = "No unit training";
                UpdateQueueSizeText();

                troop.GetComponent<Unit>().moveToPosition = movePosition;
                AstarAI myAstarAI = troop.GetComponent<AstarAI>();
                myAstarAI.ai.destination = movePosition;
            }
        }

        isTraining = false;
        UpdateQueueSizeText();
    }

    /// <summary>
    /// Checks if the specified position is occupied by other troops.
    /// </summary>
    private bool IsPositionOccupied(Vector3 position, float radius)
    {
        foreach (GameObject unit in UnitSelection.Instance.unitList)
        {
            float distance = Vector3.Distance(unit.transform.position, position);

            if (distance < radius)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Updates the queue size text.
    /// </summary>
    private void UpdateQueueSizeText()
    {
        float queueCount = troopQueue.Count + 1;
        barracacksQueueSizeText.text = "Queue Size: " + queueCount;
    }
}
