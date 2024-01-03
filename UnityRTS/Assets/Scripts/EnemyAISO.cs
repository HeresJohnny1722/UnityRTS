using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class EnemyAISO : ScriptableObject
{
    public enum EnemyType
    {
        Melee,
        Ranged,
        Support
    }

    public EnemyType enemyType;
    public float speed;
    public float startingHealth;
    public float damageAmountPerAttack;
    public float fireRate;
    public float attackRange;
    public float searchRange;
    public float rotationSpeed;
    
    [Header("Wizard")]
    public float healDuration;
    public float healRange;
    public float healAmount;
    public float healCooldown;
}
