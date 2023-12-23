using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public enum State
    {
        ChaseTarget,
        ShootingTarget,

    }

    private float nextShootTime;
    private float nextHealTime;

    public State state;

    public Transform targetTransform;

    [SerializeField] private Healthbar enemyHealthbar;

    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject meleeSlash;
    [SerializeField] GameObject deathEffect;


    public string targetType = "Building";

    public LayerMask playersLayerMask;
    public LayerMask buildingsLayerMask;
    public LayerMask enemyLayerMask;

    [SerializeField] Transform firePoint;
    [SerializeField] Transform raycastPoint;
    [SerializeField] EnemyAISO enemyAISO;

    public float enemyHealth;

    public TroopHit troopHit;

    private AstarAI myAstarAI;

    [SerializeField] private Animator unitAnimator;

    void Start()
    {
        myAstarAI = GetComponent<AstarAI>();
        myAstarAI.aiPath.maxSpeed = enemyAISO.speed;
        enemyHealth = enemyAISO.startingHealth;
        GameManager.instance.enemies.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyAISO.enemyType == EnemyAISO.EnemyType.Support)
        {
            CheckForEnemyUnitsInRangeToSupport();

            if (targetTransform == null)
                return;

            SetDestinationToTarget();

            RotateToTarget();

            // Check if the support is within a certain range of the target transform
            float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
            if (distanceToTarget <= enemyAISO.healRange)
            {
                //myAstarAI.ai.destination = transform.position;

                if (Time.time > nextHealTime)
                {
                    //Heal all enemies in a certain range
                    foreach (var enemy in GameManager.instance.enemies)
                    {
                        if (Vector3.Distance(transform.position, enemy.transform.position) < enemyAISO.healRange)
                        {
                            enemy.GetComponent<EnemyAI>().HealEnemyUnit(enemyAISO.healAmount);
                        }
                    }


                    nextShootTime = Time.time + enemyAISO.healCooldown;
                }
            }

        }
        else
        {



            CheckForUnitsAndBuildingsInRange();

            if (targetTransform == null)
                return;


            //Bobbing Animation
            unitAnimator.Play("UnitBob");

            if (targetType == "Unit")
            {
                SetDestinationToTarget();

                RotateToTarget();

                // Check if the good unit is within a certain range of the target transform
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
                //enemyAISO.attackRange
                if (distanceToTarget <= enemyAISO.attackRange)
                {
                    if (enemyAISO.enemyType == EnemyAISO.EnemyType.Ranged)
                    {
                        if (distanceToTarget <= enemyAISO.attackRange - 3)
                        {
                            myAstarAI.ai.destination = transform.position;
                            //unitAnimator.Play("Idle");
                        }
                    }

                    //
                    // Target within attack range
                    if (Time.time > nextShootTime)
                    {
                        state = State.ShootingTarget;
                        AttackUnit();
                        nextShootTime = Time.time + enemyAISO.fireRate;
                    }
                }
            }
            else if (targetType == "Building")
            {
                myAstarAI.ai.destination = myAstarAI.ai.destination = new Vector3(targetTransform.GetChild(1).position.x + Random.Range(-2, 2), targetTransform.GetChild(1).position.y, targetTransform.GetChild(1).position.z + Random.Range(-2, 2));

                RotateToTarget();

                // Check if the good unit is within a certain range of the target transform
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.GetChild(1).position);
                Debug.Log(targetTransform.GetComponent<BoxCollider>().size.x);
                if (distanceToTarget <= enemyAISO.attackRange)
                {
                    //If the enemy is a range enemy, stop it from moving any closer with a little offset in case there are troops behind it
                    if (enemyAISO.enemyType == EnemyAISO.EnemyType.Ranged)
                    {
                        if (distanceToTarget <= enemyAISO.attackRange - 3)
                        {
                            myAstarAI.ai.destination = transform.position;
                            //unitAnimator.Play("Idle");
                        }
                    }


                    //unitAnimator.Play("Idle");
                    // Target within attack range
                    if (Time.time > nextShootTime)
                    {
                        state = State.ShootingTarget;
                        AttackUnit();
                        nextShootTime = Time.time + enemyAISO.fireRate;
                    }
                }
            }

            //Transform is different b

        }
    }



    private void SetDestinationToTarget()
    {
        myAstarAI.ai.destination = new Vector3(targetTransform.position.x + Random.Range(-2, 2), targetTransform.position.y, targetTransform.position.z + Random.Range(-2, 2));
    }

    /// <summary>
    /// Rotates the target
    /// </summary>
    private void RotateToTarget()
    {
        if (targetTransform == null)
            return;

        if (targetType == "Building")
        {
            Vector3 directionToTarget = (targetTransform.GetChild(1).position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0f, directionToTarget.z));

            // Use Slerp instead of Lerp to smoothly rotate towards the target continuously
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * enemyAISO.rotationSpeed);
        }
        else
        {
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0f, directionToTarget.z));

            // Use Slerp instead of Lerp to smoothly rotate towards the target continuously
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * enemyAISO.rotationSpeed);
        }


    }


    private void AttackUnit()
    {


        if (targetType == "Unit")
        {


            RaycastHit hit;
            if (Physics.Raycast(raycastPoint.position, (targetTransform.position - raycastPoint.position).normalized, out hit, playersLayerMask))
            {
                if (enemyAISO.enemyType == EnemyAISO.EnemyType.Melee)
                {
                    meleeSlash.SetActive(false);
                    meleeSlash.SetActive(true);
                } else if (enemyAISO.enemyType == EnemyAISO.EnemyType.Ranged)
                {
                    muzzleFlash.SetActive(false);
                    muzzleFlash.SetActive(true);
                }

                float health = 0;
                float unitHealth;

                if (targetTransform.GetComponent<Unit>())
                {
                    unitHealth = (targetTransform.GetComponent<Unit>().unitHealth);
                    health = unitHealth - enemyAISO.damageAmountPerAttack;
                    targetTransform.GetComponent<Unit>().takeDamage(enemyAISO.damageAmountPerAttack);
                }

                state = State.ChaseTarget;

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

                    float health = 0;
                    float buildingHealth;
                    if (targetTransform.GetComponent<Building>())
                    {
                        buildingHealth = (targetTransform.GetComponent<Building>().buildingHealth);
                        health = buildingHealth - enemyAISO.damageAmountPerAttack;
                        targetTransform.GetComponent<Building>().takeDamage(enemyAISO.damageAmountPerAttack);
                    }


                    state = State.ChaseTarget;

                }
                else
                {
                    targetTransform = null;
                    //state = State.Roaming;
                }
            }
            else
            {
                targetTransform = null;
                //state = State.Roaming;
            }

        }

        //        throw new System.Exception("nothing attacked");
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

        // Check units
        foreach (var unit in UnitSelection.Instance.unitList)
        {
            float distance = Vector3.Distance(transform.position, unit.transform.position);

            // Check if the unit is within the search radius and closer than the current closest target
            if (distance < enemyAISO.searchRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = unit.transform;
                targetType = "Unit";
            }
        }

        // Check buildings
        foreach (var building in BuildingSelection.Instance.buildingsList)
        {
            if (building.GetComponent<Building>().stage != 1 && building.GetComponent<BuildingManager>().isFixed)
            {
                float distance = Vector3.Distance(transform.position, building.transform.GetChild(1).position);

                // Check if the building is within the search radius and closer than the current closest target
                if (distance < enemyAISO.searchRange && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = building.transform;
                    targetType = "Building";
                }
            }
        }

        // If a closest target is found, set targetTransform to the transform of that target
        if (closestTarget != null)
        {
            targetTransform = closestTarget;
        }
    }

    private void CheckForEnemyUnitsInRangeToSupport()
    {
        float closestDistance = float.MaxValue;
        Transform closestTarget = null;

        // Check units
        foreach (var enemy in GameManager.instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // Check if the unit is within the search radius and closer than the current closest target
            // Also, ensure that the enemy's transform is not the same as the current game object or its children
            // Also, checks if is not a support enemy type
            if (distance < enemyAISO.searchRange && distance < closestDistance && enemy.transform != transform && !IsChildOf(transform, enemy.transform))
            {

                //if (enemy.GetComponent<EnemyAI>() == null)
                    //return;

                //if (enemy.GetComponent<EnemyAI>().enemyAISO.enemyType == EnemyAISO.EnemyType.Support)
                    //return;

                closestDistance = distance;
                closestTarget = enemy.transform;
            }
        }

        // If a closest target is found, set targetTransform to the transform of that target
        if (closestTarget != null)
        {
            targetTransform = closestTarget;
        }
    }

    // Helper function to check if a given transform is a child of another transform
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
