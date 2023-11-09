using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class BuildingSO : ScriptableObject
{
    public enum BuildingType
    {
        House,
        Barracks
    }

    [Header("If a barracks")]
    public List<UnitSO> unitsToTrain = new List<UnitSO>();

    public BuildingType buildingType;
    public new string name;
    public string description;
    public float spawnOffset = 1f;
    public float startingHealth;
    public float timeToBuild;
    public float goldCost;
    public float coalCost;
    public float copperCost;
}
