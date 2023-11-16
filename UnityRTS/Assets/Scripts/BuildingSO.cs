using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class BuildingSO : ScriptableObject
{
    public enum BuildingType
    {
        House,
        Barracks,
        Production
    }

    public enum ResourceType
    {
        Gold,
        Copper,
        Coal
    }

    public BuildingType buildingType;

    [Header("Only for Barracks Type")]
    public List<UnitSO> unitsToTrain = new List<UnitSO>();
    public float spawnFlagMoveToTroopOffset = 1f;

    [Header("Only for Production Type")]
    public ResourceType resourceType;
    public float resourceOutputRate;
    public float outputWorkerMultiplyer;
    public float workerCapacity;

    [Header("Needed for all Types")]
    public new string name;
    public string description;
    
    public float startingHealth;
    public float timeToBuild;
    public float goldCost;
    public float coalCost;
    public float copperCost;
    
}
