using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

/// <summary>
/// Manages the behavior of enemy AI units, including movement, attacking, healing, and target selection.
/// </summary>
public class EnemyAI : MonoBehaviour
{

    public  EnemyAISO enemyAISO;

    public Healthbar enemyHealthbar;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject meleeSlash;
    [SerializeField] private GameObject deathEffect;

    public LayerMask playersLayerMask;
    public LayerMask buildingsLayerMask;
    public LayerMask enemyLayerMask;

    private float nextShootTime;
    private float nextHealTime;
    public float enemyHealth;

    [SerializeField] private Transform raycastPoint;
    public Transform targetTransform;

    public string targetType;

    public TroopHit troopHit;

    private AstarAI myAstarAI;

    [SerializeField] private Animator unitAnimator;

    void Start()
    {
        SetupEnemy();
        //AssignSavedStats();
    }

    

    /// <summary>
    /// Sets up the initial state and registers the enemy with GameManager.
    /// </summary>
    private void SetupEnemy()
    {
        myAstarAI = GetComponent<AstarAI>();
        myAstarAI.aiPath.maxSpeed = enemyAISO.speed;
        enemyHealth = enemyAISO.startingHealth;
        GameManager.instance.enemies.Add(gameObject);
        enemyHealthbar.gameObject.SetActive(false);
    }

    /// <summary>
    /// Update method is called every frame and manages the enemy's behavior based on its type.
    /// </summary>
    void Update()
    {
        if (enemyAISO.enemyType == EnemyAISO.EnemyType.Support)
        {
            if (targetTransform == null)
                CheckForEnemyUnitsInRangeToSupport();

            if (targetTransform == null)
                return;

            MoveAndHealTarget();

            
        }
        else
        {
            CheckForUnitsAndBuildingsInRange();

            if (targetTransform == null)
                return;

            unitAnimator.Play("UnitBob");

            if (targetType == "Unit")
            {
                MoveAndAttackTarget(targetTransform);

            } else if (targetType == "Building")
            {
                MoveAndAttackTarget(targetTransform.GetChild(1));
            }
        }
    }

    /// <summary>
    /// Moves toward and heals nearby enemy units.
    /// </summary>
    private void MoveAndHealTarget()
    {
        if (targetTransform.position != myAstarAI.ai.destination)
        {
            SetDestinationToTargetTransform(targetTransform);
        }

        
        RotateToTarget();

        float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
        if (distanceToTarget <= enemyAISO.healRange)
        {
            if (Time.time > nextHealTime)
            {
                foreach (var enemy in GameManager.instance.enemies)
                {
                    if ((Vector3.Distance(transform.position, enemy.transform.position) < enemyAISO.healRange) && enemy != gameObject)
                    {
                        enemy.GetComponent<EnemyAI>().HealEnemyUnit(enemyAISO.healAmount);
                    }
                }

                nextHealTime = Time.time + enemyAISO.healCooldown;
            }
        }
    }

    /// <summary>
    /// Moves toward and attacks the target (unit or building) based on enemy type.
    /// </summary>
    private void MoveAndAttackTarget(Transform transformTarget)
    {

        if (transformTarget.position != myAstarAI.ai.destination)
        {
            SetDestinationToTargetTransform(transformTarget);
        }
            
            RotateToTarget();

            float distanceToTarget = Vector3.Distance(transform.position, transformTarget.position);
            if (distanceToTarget <= enemyAISO.attackRange)
            {
                if (enemyAISO.enemyType == EnemyAISO.EnemyType.Ranged && distanceToTarget <= enemyAISO.attackRange - 3)
                {
                    myAstarAI.ai.destination = transform.position;
                }

                if (Time.time > nextShootTime)
                {
                    
                    AttackUnit();
                    nextShootTime = Time.time + enemyAISO.fireRate;
                }
            }
    }

    /// <summary>
    /// Sets the destination for the A* pathfinding to the target's position.
    /// </summary>
    private void SetDestinationToTargetTransform(Transform transformTarget)
    {
        myAstarAI.ai.destination = transformTarget.position;
    }

    /// <summary>
    /// Rotates the enemy towards its current target.
    /// </summary>
    private void RotateToTarget()
    {
        if (targetTransform == null)
            return;

        Rotate(targetTransform);

        if (targetType == "Building")
        {
            Rotate(targetTransform.GetChild(1));
        }
        else
        {
            Rotate(targetTransform);
            
        }
    }

    private void Rotate(Transform transformTarget)
    {
        Vector3 directionToTarget = (transformTarget.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0f, directionToTarget.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * enemyAISO.rotationSpeed);
    }

    /// <summary>
    /// Attacks the current target (unit or building) based on enemy type.
    /// </summary>
    private void AttackUnit()
    {
        if (targetType == "Unit")
        {

            if (targetTransform == null)
                return;

            RaycastHit hit;
            if (Physics.Raycast(raycastPoint.position, (targetTransform.position - raycastPoint.position).normalized, out hit, playersLayerMask))
            {
                //MuzzleFlash();

                if (targetTransform.GetComponent<Unit>())
                {
                    targetTransform.GetComponent<Unit>().takeDamage(enemyAISO.damageAmountPerAttack);
                }

            }
            else
            {
                targetTransform = null;
            }
        }
        else if (targetType == "Building")
        {

            if (targetTransform == null)
                return;

            if (targetTransform.GetComponent<Building>().stage != 1)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (targetTransform.GetChild(1).position - raycastPoint.position).normalized, out hit, buildingsLayerMask))
                {
                    //MuzzleFlash();

                    if (targetTransform.GetComponent<Building>())
                    {
                        targetTransform.GetComponent<Building>().takeDamage(enemyAISO.damageAmountPerAttack);
                    }

                }
                else
                {
                    targetTransform = null;
                }
            }
            else
            {
                targetTransform = null;
            }

        }
    }

    /// <summary>
    /// Plays the muzzle flash or melee slash visual effect.
    /// </summary>
    private void MuzzleFlash()
    {
        if (enemyAISO.enemyType == EnemyAISO.EnemyType.Melee)
        {
            meleeSlash.SetActive(false);
            meleeSlash.SetActive(true);
        }
        else if (enemyAISO.enemyType == EnemyAISO.EnemyType.Ranged)
        {
            muzzleFlash.SetActive(false);
            muzzleFlash.SetActive(true);
        }
    }

    /// <summary>
    /// Deals damage to the enemy and manages death-related actions.
    /// </summary>
    public void TakeDamage(float damageAmount)
    {
        enemyHealth -= damageAmount;

        if (enemyHealth <= 0)
        {
            GameObject deathEfct = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(deathEfct, 2f);
            Destroy(this.gameObject);
            GameManager.instance.enemies.Remove(gameObject);
        }

        troopHit.HitAnimation();
        enemyHealthbar.gameObject.SetActive(true);
        enemyHealthbar.UpdateHealthBar(enemyAISO.startingHealth, enemyHealth);
    }

    /// <summary>
    /// Heals the enemy unit by the specified amount.
    /// </summary>
    public void HealEnemyUnit(float healAmount)
    {
        enemyHealth += healAmount;

        if (enemyHealth > enemyAISO.startingHealth)
        {
            enemyHealth = enemyAISO.startingHealth;
        }

        enemyHealthbar.gameObject.SetActive(true);
        enemyHealthbar.UpdateHealthBar(enemyAISO.startingHealth, enemyHealth);
    }

    /// <summary>
    /// Checks for units and buildings in range to determine the next target.
    /// </summary>
    private void CheckForUnitsAndBuildingsInRange()
    {
        float closestDistance = float.MaxValue;
        Transform closestTarget = null;
        foreach (var unit in UnitSelection.Instance.unitList)
        {
            float distance = Vector3.Distance(transform.position, unit.transform.position);
            if (distance < enemyAISO.searchRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = unit.transform;
                targetType = "Unit";
            }
        }
        foreach (var building in BuildingSelection.Instance.buildingsList)
        {
            if (building.GetComponent<Building>().stage != 1 && building.GetComponent<BuildingManager>().isFixed)
            {
                float distance = Vector3.Distance(transform.position, building.transform.GetChild(1).position);
                if (distance < enemyAISO.searchRange && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = building.transform;
                    targetType = "Building";
                }
            }
        }
        if (closestTarget != null)
        {
            targetTransform = closestTarget;
        } else
        {
            targetTransform = null;
        }
    }

    /// <summary>
    /// Checks for enemy units in range to support.
    /// </summary>
    private void CheckForEnemyUnitsInRangeToSupport()
    {
        float closestDistance = float.MaxValue;
        Transform closestTarget = null;

        foreach (var enemy in GameManager.instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < enemyAISO.searchRange && distance < closestDistance && enemy.transform != transform && !IsChildOf(transform, enemy.transform))
            {
                closestDistance = distance;
                closestTarget = enemy.transform;
            }
        }

        if (closestTarget != null)
        {
            targetTransform = closestTarget;
        }
    }

    /// <summary>
    /// Checks if the given child transform is a child of the parent transform.
    /// </summary>
    private bool IsChildOf(Transform parent, Transform child)
    {
        while (child != null)
        {
            if (child == parent)
            {
                return true;
            }
            child = child.parent;
        }
        return false;
    }
}
