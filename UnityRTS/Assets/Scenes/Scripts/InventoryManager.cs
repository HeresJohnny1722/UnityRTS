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
    public int coal = 0;
    public int copper = 0;
    public int energy = 0;

    public int maxHouses;
    public int maxBarracks;
    public int maxGoldFactory;
    public int maxCoalFactory;
    public int houseCount;
    public int barracksCount;
    public int goldFactoryCount;
    public int coalFactoryCount;

    // TextMeshPro fields for displaying resource quantities
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI coalText;
    public TextMeshProUGUI copperText;
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
        coal += addedCoal;
        copper += addedCopper;
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
        if (gold >= removedGold && coal >= removedCoal && copper >= removedCopper && energy >= removedEnergy)
        {
            
            gold -= removedGold;
            coal -= removedCoal;
            copper -= removedCopper;
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
               coal >= requiredCoal &&
               copper >= requiredCopper &&
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

            } else if (buildingSO.resourceType == BuildingSO.ResourceType.Coal)
            {

                if (InventoryManager.instance.maxCoalFactory > InventoryManager.instance.coalFactoryCount)
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
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Coal)
            {

                InventoryManager.instance.coalFactoryCount++;

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
            else if (buildingSO.resourceType == BuildingSO.ResourceType.Coal)
            {

                InventoryManager.instance.coalFactoryCount--;

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

        if (coalText != null)
            coalText.text = "Coal: " + coal.ToString();

        if (copperText != null)
            copperText.text = "Copper: " + copper.ToString();

        if (energyText != null)
            energyText.text = "Energy: " + energy.ToString();
    }
}
