using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, IDataPersistence
{
    // Singleton instance
    public static GameManager instance;

    public GameObject VictoryScreen;
    public GameObject GameOverScreen;
    public bool isGameOver = false;

    public int enemiesKilledCount;
    public TextMeshProUGUI enemiesKilledCountText;
    public List<GameObject> enemies = new List<GameObject>();
    public bool areWavesDone;


    public List<GameObject> buildingPrefabs = new List<GameObject>();
    public List<GameObject> unitPrefabs = new List<GameObject>();
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    // Inventory variables
    public int currentPopulation = 0;
    public int maxedPopulation = 10;
    public int gold = 0;
    public int wood = 0;
    public int food = 0;
    public int energy = 0;

    public int maxHouses;
    public int maxBarracks;
    public int maxGoldFactory;
    public int maxLumberMill;
    public int maxGreenhouse;
    public int maxDefenseBuildingCount;
    public int maxWall;
    public int maxGate;
    public int houseCount;
    public int barracksCount;
    public int goldFactoryCount;
    public int lumberMillCount;
    public int greenhouseCount;
    public int defenseBuildingCount;
    public int wallCount;
    public int gateCount;

    // TextMeshPro fields for displaying resource quantities
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI towerText;

    public GameObject[] buildingButtons;

    // Events to update UI or trigger other actions when inventory changes
    public delegate void OnInventoryChanged();
    public event OnInventoryChanged InventoryChanged;

    private void Awake()
    {
        // Implementing the Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateTextFields();
    }

    public void Start()
    {
        UpdateTextFields();
    }

    private GameObject FindBuildingPrefab(string enemyName)
    {
        // Find the prefab in GameManager.instance.buildingPrefabs
        foreach (var prefab in this.enemyPrefabs)
        {
            // Compare the first 5 characters of the prefab name with the provided buildingName
            if (prefab.name.Length >= 5 && prefab.name.Substring(0, 5) == enemyName.Substring(0, 5))
            {
                return prefab;
            }
        }

        return null; // If no matching prefab is found
    }

    public void LoadData(GameData data)
    {
        this.enemiesKilledCount = data.enemiesKilled;
        this.currentPopulation = data.currentPopulation;
        this.maxedPopulation = data.maxPopulation;
        this.gold = data.gold;
        this.wood = data.wood;
        this.food = data.food;


        //Enemy loading

        enemies.Clear();

        for (int i = 0; i < data.enemyListName.Count; i++)
        {
            string enemyName = data.enemyListName[i];
            Vector3 enemyPosition = data.enemyListPositions[i];

            // Find the corresponding prefab in GameManager.instance.buildingPrefabs
            GameObject prefab = FindBuildingPrefab(enemyName);

            if (prefab != null)
            {
                // Instantiate the prefab and add it to the buildingsList and set its health to the saved health
                GameObject enemyObject = Instantiate(prefab, enemyPosition, Quaternion.identity);
                EnemyAI enemy = enemyObject.GetComponent<EnemyAI>();

                enemy.enemyHealth = data.enemyListHealth[i];
                Debug.Log(data.enemyListName[i]);

                //If the building has taken damage, we should see the healthbar
                if (enemy.enemyHealth != enemy.enemyAISO.startingHealth)
                {
                    enemy.enemyHealthbar.gameObject.SetActive(true);
                    enemy.enemyHealthbar.UpdateHealthBar(enemy.enemyAISO.startingHealth, enemy.enemyHealth);
                }
                else
                {
                    enemy.enemyHealthbar.gameObject.SetActive(false);
                    enemy.enemyHealthbar.UpdateHealthBar(enemy.enemyAISO.startingHealth, enemy.enemyHealth);
                }

                //buildingsList.Add(buildingObject);
            }
            else
            {
                Debug.LogWarning("Prefab not found for enemy: " + enemyName);
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.enemiesKilled = this.enemiesKilledCount;
        data.currentPopulation = this.currentPopulation;
        data.maxPopulation = this.maxedPopulation;
        data.gold = this.gold;
        data.wood = this.wood;
        data.food = this.food;

        //Enemies Save
        data.enemyListName.Clear();
        data.enemyListPositions.Clear();
        data.enemyListHealth.Clear();

        for (int i = 0; i < this.enemies.Count; i++)
        {
            data.enemyListName.Add(this.enemies[i].name);
            data.enemyListPositions.Add(this.enemies[i].transform.position);
            data.enemyListHealth.Add((int)this.enemies[i].GetComponent<EnemyAI>().enemyHealth);
        }

        //Debug.Log(data.enemyListName.Count);
    }

    public void GameOver()
    {
        if (VictoryScreen.activeSelf == false)
        {
            isGameOver = true;
            GameOverScreen.SetActive(true);
          //  Time.timeScale = .25f;
        }
        
    }

    public void Victory()
    {
        //isGameOver = false;
        
        VictoryScreen.SetActive(true);
       // Time.timeScale = .25f;
    }

    private void Update()
    {
        enemiesKilledCountText.text = "Enemies killed: " + enemiesKilledCount.ToString();
        towerText.text = "Towers: " + defenseBuildingCount + "/" + maxDefenseBuildingCount;

        if (areWavesDone)
        {
            if (enemies.Count == 0)
            {
                //Win
                if (!isGameOver)
                {
                    Victory();
                }
                
            }
        }
    }

    // Add resources to the inventory
    public void AddResources(int addedPopulation, int addedGold, int addedWood, int addedFood, int addedEnergy)
    {
        
        gold += addedGold;
        wood += addedWood;
        food += addedFood;
        energy += addedEnergy;

        // Update TextMeshPro fields
        UpdateTextFields();

        // Trigger event to notify that the inventory has changed
        InventoryChanged?.Invoke();
    }

    public void changeMaxPopulation(int populationIncrease)
    {
        maxedPopulation += populationIncrease;

        // Update TextMeshPro fields
        UpdateTextFields();

        // Trigger event to notify that the inventory has changed
        InventoryChanged?.Invoke();
    }

    public void changeCurrentPopulation(int populationIncrease)
    {
        currentPopulation += populationIncrease;

        // Update TextMeshPro fields
            UpdateTextFields();

            // Trigger event to notify that the inventory has changed
            InventoryChanged?.Invoke();
    }

    // Remove resources from the inventory
    public void RemoveResources(int removedPopulation, int removedGold, int removedWood, int removedFood, int removedEnergy)
    {
        // Check if there are enough resources to remove
        if (gold >= removedGold && wood >= removedWood && food >= removedFood && energy >= removedEnergy)
        {
            
            gold -= removedGold;
            wood -= removedWood;
            food -= removedFood;
            energy -= removedEnergy;

            // Update TextMeshPro fields
            UpdateTextFields();

            // Trigger event to notify that the inventory has changed
            InventoryChanged?.Invoke();


        }


    }

    public bool AreResourcesAvailable(int requiredPopulation, int requiredGold, int requiredWood, int requiredFood, int requiredEnergy)
    {
        return maxedPopulation >= (currentPopulation + requiredPopulation) &&
               gold >= requiredGold &&
               wood >= requiredWood &&
               food >= requiredFood &&
               energy >= requiredEnergy;
    }

    public bool CheckBuildingCountAvailable(BuildingSO buildingSO)
    {

        if (buildingSO.buildingType == BuildingSO.BuildingType.House)
        {
            if (GameManager.instance.maxHouses > GameManager.instance.houseCount)
            {

                return true;
            }
            else
            {
                return false;
            }
        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {
            if (GameManager.instance.maxBarracks > GameManager.instance.barracksCount)
            {

                return true;
            }
            else
            {
                return false;
            }
        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Defense)
        {
            if (GameManager.instance.maxDefenseBuildingCount > GameManager.instance.defenseBuildingCount)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        else if (buildingSO.buildingType == BuildingSO.BuildingType.Wall)
        {
            if (buildingSO.name == "Wood Gate")
            {
                if (GameManager.instance.maxGate > GameManager.instance.gateCount)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            } else
            {
                if (GameManager.instance.maxWall > GameManager.instance.wallCount)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {

            if (buildingSO.resourceType == BuildingSO.ResourceType.Gold)
            {

                if (GameManager.instance.maxGoldFactory > GameManager.instance.goldFactoryCount)
                {

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Wood)
            {

                if (GameManager.instance.maxLumberMill > GameManager.instance.lumberMillCount)
                {

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Food)
            {

                if (GameManager.instance.maxGreenhouse > GameManager.instance.greenhouseCount)
                {

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }
        else
        {
            return true;
        }
    }

    public void increaseBuildingCount(BuildingSO buildingSO)
    {

        if (buildingSO.buildingType == BuildingSO.BuildingType.House)
        {

            GameManager.instance.houseCount++;

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {

            GameManager.instance.barracksCount++;

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Defense)
        {

            GameManager.instance.defenseBuildingCount++;

        }

        else if (buildingSO.buildingType == BuildingSO.BuildingType.Wall)
        {

            if (buildingSO.name == "Wood Gate")
            {
                GameManager.instance.gateCount++;
            } else
            {
                GameManager.instance.wallCount++;
            }
            

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {

            if (buildingSO.resourceType == BuildingSO.ResourceType.Gold)
            {

                GameManager.instance.goldFactoryCount++;

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Wood)
            {

                GameManager.instance.lumberMillCount++;

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Food)
            {

                GameManager.instance.greenhouseCount++;

            }


        }

        // Update TextMeshPro fields
        UpdateTextFields();

        // Trigger event to notify that the inventory has changed
        InventoryChanged?.Invoke();

    }

    public void decreaseBuildingCount(BuildingSO buildingSO)
    {

        if (buildingSO.buildingType == BuildingSO.BuildingType.House)
        {

            GameManager.instance.houseCount--;

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {

            GameManager.instance.barracksCount--;

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Defense)
        {

            GameManager.instance.defenseBuildingCount--;

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Wall)
        {

            if (buildingSO.name == "Wood Gate")
            {
                GameManager.instance.gateCount--;
            }
            else
            {
                GameManager.instance.wallCount--;
            }


        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {

            if (buildingSO.resourceType == BuildingSO.ResourceType.Gold)
            {

                GameManager.instance.goldFactoryCount--;

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Wood)
            {

                GameManager.instance.lumberMillCount--;

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Food)
            {

                GameManager.instance.greenhouseCount--;

            }


        }

        // Update TextMeshPro fields
        UpdateTextFields();

        // Trigger event to notify that the inventory has changed
        InventoryChanged?.Invoke();

    }

    // Update TextMeshPro fields with current inventory values
    public void UpdateTextFields()
    {
        if (populationText != null)
            populationText.text = "Population: " + currentPopulation.ToString() + "/" + maxedPopulation.ToString();

        if (goldText != null)
            goldText.text = "Gold: " + gold.ToString();

        if (woodText != null)
            woodText.text = "Wood: " + wood.ToString();

        if (foodText != null)
            foodText.text = "Food: " + food.ToString();

        //if (energyText != null)
            //energyText.text = "Energy: " + energy.ToString();
    }

    public void HealAllEnemies(float healAmount)
    {
        foreach (var enemyUnit in enemies)
        {
            EnemyAI enemy = enemyUnit.GetComponent<EnemyAI>();

            try
            {
                enemy.HealEnemyUnit(healAmount);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }
}
