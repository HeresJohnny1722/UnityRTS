using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class UnitSO : ScriptableObject
{
    public enum UnitType
    {
        Gunner,
        Pikeman,
        Worker,
        Healer
    }

    public GameObject prefab;
    public UnitType unitType;
    public new string name;
    public float trainingTime;
    public float startingHealth;
    public float buildRange;
    public float attackRange;
    public float attackDamage;
    public float fireRate;
    public float speed;
    public float rotationSpeed;
    public string description;
    public float goldCost;
    public float woodCost;
    public float foodCost;
    public float populationCost;

    public float gatheringSpeed;
}
