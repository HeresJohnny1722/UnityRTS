using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] private EnemyAISO enemyAISO;

    [SerializeField] private Healthbar enemyHealthbar;
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
    }

    private void SetupEnemy()
    {
        myAstarAI = GetComponent<AstarAI>();
        myAstarAI.aiPath.maxSpeed = enemyAISO.speed;
        enemyHealth = enemyAISO.startingHealth;
        GameManager.instance.enemies.Add(gameObject);
        enemyHealthbar.gameObject.SetActive(false);
    }

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

    private void MoveAndHealTarget()
    {
        SetDestinationToTargetTransform(targetTransform);
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

    private void MoveAndAttackTarget(Transform transformTarget)
    {
        
            SetDestinationToTargetTransform(transformTarget);
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

    private void SetDestinationToTargetTransform(Transform transformTarget)
    {
        myAstarAI.ai.destination = new Vector3(transformTarget.position.x + Random.Range(-4, 4), transformTarget.position.y, transformTarget.position.z + Random.Range(-4, 4));
    }

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

    private void AttackUnit()
    {
        if (targetType == "Unit")
        {
            RaycastHit hit;
            if (Physics.Raycast(raycastPoint.position, (targetTransform.position - raycastPoint.position).normalized, out hit, playersLayerMask))
            {
                MuzzleFlash();

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
            if (targetTransform.GetComponent<Building>().stage != 1)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (targetTransform.GetChild(1).position - raycastPoint.position).normalized, out hit, buildingsLayerMask))
                {
                    MuzzleFlash();

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
            
        }
    }

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
        }
    }

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
