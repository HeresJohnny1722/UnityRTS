using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // Singleton instance
    public static InventoryManager instance;

    // Inventory variables
    public int currentPopulation = 0;
    public int maxPopulation = 30;
    public int gold = 0;
    public int coal = 0;
    public int copper = 0;
    public int energy = 0;

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

    // Add resources to the inventory
    public void AddResources(int addedPopulation, int addedGold, int addedCoal, int addedCopper, int addedEnergy)
    {
        if (maxPopulation >= (currentPopulation += addedPopulation))
        {
            currentPopulation += addedPopulation;
        }
        gold += addedGold;
        coal += addedCoal;
        copper += addedCopper;
        energy += addedEnergy;

        // Update TextMeshPro fields
        UpdateTextFields();

        // Trigger event to notify that the inventory has changed
        InventoryChanged?.Invoke();
    }

    // Remove resources from the inventory
    public void RemoveResources(int removedPopulation, int removedGold, int removedCoal, int removedCopper, int removedEnergy)
    {
        // Check if there are enough resources to remove
        if (currentPopulation >= removedPopulation && gold >= removedGold && coal >= removedCoal && copper >= removedCopper && energy >= removedEnergy)
        {
            currentPopulation -= removedPopulation;
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
        return maxPopulation >= (currentPopulation += requiredPopulation) &&
               gold >= requiredGold &&
               coal >= requiredCoal &&
               copper >= requiredCopper &&
               energy >= requiredEnergy;
    }

    // Update TextMeshPro fields with current inventory values
    private void UpdateTextFields()
    {
        if (populationText != null)
            populationText.text = "Population: " + currentPopulation.ToString() + "/" + maxPopulation.ToString();

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
