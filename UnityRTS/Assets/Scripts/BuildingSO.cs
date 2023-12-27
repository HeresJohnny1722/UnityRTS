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
        Townhall
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
    public float outputPerInterval;
    public float outputTime;
    public string nodeName;
    public string nodeDescription;
    public float stage = 1;


    [Header("Only for Defense Type")]
    public float fireRate;
    public float attackDamage;
    public float attackRange;
    public float turretRotationSpeed;

    public bool bulletsExplode;
    public float explodeRadius;
    public GameObject explodeParticleSystem;


    [Header("Only Buildings that increase population")]
    public int populationIncrease;

    [Header("Needed for all Types")]
    public new string name;
    public string description;
    public int startingHealth;
    public int timeToBuild;
    public int goldCost;
    public int woodCost;
    public int foodCost;
    
}
