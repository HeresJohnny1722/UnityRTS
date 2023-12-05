using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class EnemyAISO : ScriptableObject
{
    public enum EnemyType
    {
        Melee
    }

    public EnemyType enemyType;
    public float speed;
    public float startingHealth;
    public float damageAmountPerAttack;
    public float fireRate;
    public float attackRange;
    public float searchRange;
    public float romingRange;
    public float roamingReachedPositionDistance;
    public float roamingWaitTime;
    public float rotationSpeed;
    public float bulletSpeed;
    public float stopChaseDistance;
    public float reachedStartPositionDistance;
}
