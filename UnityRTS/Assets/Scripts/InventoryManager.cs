using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // Singleton instance
    public static InventoryManager instance;

    public int enemiesKilledCount;
    public TextMeshProUGUI enemiesKilledCountText;

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
    public int houseCount;
    public int barracksCount;
    public int goldFactoryCount;
    public int lumberMillCount;

    // TextMeshPro fields for displaying resource quantities
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI energyText;

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

    private void Update()
    {
        enemiesKilledCountText.text = "Enemies killed: " + enemiesKilledCount.ToString();
    }

    // Add resources to the inventory
    public void AddResources(int addedPopulation, int addedGold, int addedCoal, int addedCopper, int addedEnergy)
    {
        
        gold += addedGold;
        wood += addedCoal;
        food += addedCopper;
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
    public void RemoveResources(int removedPopulation, int removedGold, int removedCoal, int removedCopper, int removedEnergy)
    {
        // Check if there are enough resources to remove
        if (gold >= removedGold && wood >= removedCoal && food >= removedCopper && energy >= removedEnergy)
        {
            
            gold -= removedGold;
            wood -= removedCoal;
            food -= removedCopper;
            energy -= removedEnergy;

            // Update TextMeshPro fields
            UpdateTextFields();

            // Trigger event to notify that the inventory has changed
            InventoryChanged?.Invoke();


        }


    }

    public bool AreResourcesAvailable(int requiredPopulation, int requiredGold, int requiredCoal, int requiredCopper, int requiredEnergy)
    {
        return maxedPopulation >= (currentPopulation + requiredPopulation) &&
               gold >= requiredGold &&
               wood >= requiredCoal &&
               food >= requiredCopper &&
               energy >= requiredEnergy;
    }

    public bool CheckBuildingCountAvailable(BuildingSO buildingSO)
    {

        if (buildingSO.buildingType == BuildingSO.BuildingType.House)
        {
            if (InventoryManager.instance.maxHouses > InventoryManager.instance.houseCount)
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
            if (InventoryManager.instance.maxBarracks > InventoryManager.instance.barracksCount)
            {
                
                return true;
            }
            else
            {
                return false;
            }
        } else if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {

            if (buildingSO.resourceType == BuildingSO.ResourceType.Gold)
            {

                if (InventoryManager.instance.maxGoldFactory > InventoryManager.instance.goldFactoryCount)
                {
                    
                    return true;
                }
                else
                {
                    return false;
                }

            } else if (buildingSO.resourceType == BuildingSO.ResourceType.Wood)
            {

                if (InventoryManager.instance.maxLumberMill > InventoryManager.instance.lumberMillCount)
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

            InventoryManager.instance.houseCount++;

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {

            InventoryManager.instance.barracksCount++;

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {

            if (buildingSO.resourceType == BuildingSO.ResourceType.Gold)
            {

                InventoryManager.instance.goldFactoryCount++;

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Wood)
            {

                InventoryManager.instance.lumberMillCount++;

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

            InventoryManager.instance.houseCount--;

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Barracks)
        {

            InventoryManager.instance.barracksCount--;

        }
        else if (buildingSO.buildingType == BuildingSO.BuildingType.Production)
        {

            if (buildingSO.resourceType == BuildingSO.ResourceType.Gold)
            {

                InventoryManager.instance.goldFactoryCount--;

            }
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Wood)
            {

                InventoryManager.instance.lumberMillCount--;

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

        if (energyText != null)
            energyText.text = "Energy: " + energy.ToString();
    }
}
