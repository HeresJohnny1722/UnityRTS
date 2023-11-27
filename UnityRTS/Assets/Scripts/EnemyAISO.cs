using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class EnemyAISO : ScriptableObject
{
    public enum EnemyType
    {
        Melee,
        Ranged
    }

    public EnemyType enemyType;
    public float startingHealth;
    public float damageAmountPerBullet;
    public float fireRate;
    public float attackRange;
    public float searchRange;
    public float romingRange;
    public float roamingReachedPositionDistance;
    public float rotationSpeed;
    public float bulletSpeed;
    public float stopChaseDistance;
    public float reachedStartPositionDistance;
}
