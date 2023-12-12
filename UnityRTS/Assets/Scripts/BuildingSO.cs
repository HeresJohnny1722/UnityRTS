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
        Production,
        Defense,
        Wall,
        Trap
    }

    public enum ResourceType
    {
        Gold,
        Wood,
        Food,
        Energy,
    }

    public BuildingType buildingType;

    [Header("Only for Barracks Type")]
    public List<UnitSO> unitsToTrain = new List<UnitSO>();
    public float spawnFlagMoveToTroopOffset = 1f;

    [Header("Only for Production Type")]
    public ResourceType resourceType;
    public float outputPerSecond;
    public string nodeName;
    public string nodeDescription;
    public float stage = 1;


    [Header("Only for Defense Type")]
    public float fireRate;
    public float attackDamage;
    public float attackRange;
    public float turretRotationSpeed;


    [Header("Only Buildings that increase population")]
    public int populationIncrease;

    [Header("Needed for all Types")]
    public new string name;
    public string description;
    public float startingHealth;
    public float timeToBuild;
    public float goldCost;
    public float woodCost;
    public float foodCost;
    
}
