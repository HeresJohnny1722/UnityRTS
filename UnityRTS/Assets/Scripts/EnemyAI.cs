using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CodeMonkey.Utils;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Roaming,
        ChaseTarget,
        ShootingTarget,
        GoingBackToStart,
    }

    //private IAimShootAnims aimShootAnims;
    private NavMeshAgent navMeshAgent;
    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private float nextShootTime;
    public State state;

    [SerializeField] private Healthbar enemyHealthbar;

    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject meleeSlash;
    [SerializeField] GameObject deathEffect;

    public Transform targetTransform;
    public string targetType;

    public LayerMask playersLayerMask;
    public LayerMask buildingsLayerMask;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform raycastPoint;
    [SerializeField] EnemyAISO enemyAISO;

    public float enemyHealth;

    ///public EnemyManager enemyManager;


    private void Awake()
    {
        //enemyManager.enemyList.Add(gameObject);
        //muzzleFlash.SetActive(false);
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = enemyAISO.speed;
        //aimShootAnims = GetComponent<IAimShootAnims>();
        state = State.Roaming;
        enemyHealth = enemyAISO.startingHealth;


    }

    private void Start()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        //navMeshAgent.SetDestination(playerTransform.position);
    }

    public void takeDamage(float damageAmount)
    {
        enemyHealth -= damageAmount;

        if (enemyHealth <= 0)
        {
            //deselectUnit();
            //InventoryManager.instance.changeCurrentPopulation(-(int)unitSO.populationCost);

            //if (UnitSelection.Instance.unitsSelected.Contains(this.gameObject))
            //{
            //UnitSelection.Instance.unitsSelected.Remove(this.gameObject);
            //}

            //UnitSelection.Instance.unitList.Remove(this.gameObject);
            GameObject deathEfct = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(deathEfct, 2f);

            Destroy(this.gameObject);



        }
        enemyHealthbar.gameObject.SetActive(true);
        enemyHealthbar.UpdateHealthBar(enemyAISO.startingHealth, enemyHealth);
        //unitHealthbar.gameObject.SetActive(true);
        //unitHealthbar.UpdateHealthBar(unitSO.startingHealth, unitHealth);
    }

    private void Update()
    {

        switch (state)
        {
            default:
            case State.Roaming:
                navMeshAgent.SetDestination(roamPosition);

                if (Vector3.Distance(transform.position, roamPosition) < enemyAISO.roamingReachedPositionDistance)
                {
                    roamPosition = GetRoamingPosition();
                }

                CheckForUnitsAndBuildingsInRange(); // Check for units before transitioning to ChaseTarget
                FindTarget();
                break;

            case State.ChaseTarget:


                if (targetTransform != null)
                {
                    RotateTowardsPlayerOrBuilding();
                    navMeshAgent.SetDestination(targetTransform.position);

                    if (targetType == "Unit")
                    {
                        if (Vector3.Distance(transform.position, targetTransform.position) < enemyAISO.attackRange)
                        {
                            navMeshAgent.SetDestination(transform.position);

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
                        if (Vector3.Distance(transform.position, targetTransform.position) < enemyAISO.attackRange + targetTransform.GetComponent<BoxCollider>().size.x / 1.25)
                        {
                            //Debug.Log(targetTransform.GetComponent<BoxCollider>().size.x);
                            navMeshAgent.SetDestination(transform.position);

                            // Target within attack range
                            if (Time.time > nextShootTime)
                            {
                                Debug.Log("Shooting a building");
                                
                                state = State.ShootingTarget;
                                AttackUnit();

                                nextShootTime = Time.time + enemyAISO.fireRate;

                            }
                        }

                    }

                    }
                    else
                    {
                        state = State.Roaming;
                    }
                    break;
            case State.ShootingTarget:
                break;
            case State.GoingBackToStart:
                navMeshAgent.SetDestination(startingPosition);

                //reachedStartPositionDistance = 10f;
                if (Vector3.Distance(transform.position, startingPosition) < enemyAISO.reachedStartPositionDistance)
                {
                    // Reached Start Position
                    state = State.Roaming;
                }
                break;
        }

    }

    private void RotateTowardsPlayerOrBuilding()
    {
        if (targetTransform != null)
        {
            Vector3 directionToPlayer = (targetTransform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0f, directionToPlayer.z));

            // Use Lerp to smoothly rotate the enemy towards the player
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * enemyAISO.rotationSpeed);
        }
    }

    private IEnumerator WaitAndContinue(float waitTime, System.Action onWaitComplete)
    {
        yield return new WaitForSeconds(waitTime);

        // Code to be executed after waiting for 'waitTime' seconds
        Debug.Log("Waited for " + waitTime + " seconds.");

        onWaitComplete?.Invoke(); // Invoke the provided action after the wait time
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
            if (building.GetComponent<Building>().stage != 1)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);

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


    private void AttackUnit()
    {
        RotateTowardsPlayerOrBuilding();
        //muzzleFlash.SetActive(true);



        if (targetType == "Unit")
        {

            float sphereRadius = .25f; // Adjust the sphere radius as needed

            RaycastHit hit;


            if (Physics.SphereCast(raycastPoint.position, sphereRadius, (targetTransform.position - raycastPoint.position).normalized, out hit, Mathf.Infinity, playersLayerMask))
            {
                if (enemyAISO.enemyType == EnemyAISO.EnemyType.Ranged)
                {
                    GameObject mzlLFlash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
                    Destroy(mzlLFlash, .5f);

                }
                else if (enemyAISO.enemyType == EnemyAISO.EnemyType.Melee)
                {
                    GameObject mleSlash = Instantiate(meleeSlash, firePoint.position, firePoint.rotation);
                    Destroy(mleSlash, .5f);
                }

                float health = 0;
                float unitHealth;
                //Debug.Log("SphereCast hit something on the player layer");
                if (hit.transform.GetComponent<Unit>())
                {
                    unitHealth = (hit.transform.GetComponent<Unit>().unitHealth);
                    health = unitHealth - enemyAISO.damageAmountPerAttack;
                    hit.transform.GetComponent<Unit>().takeDamage(enemyAISO.damageAmountPerAttack);
                }

                if (health <= 0)
                {
                    state = State.Roaming;
                    
                }
                else
                {
                    state = State.ChaseTarget;
                }


                // Additional actions for hitting a player can be added here
            }

            else
            {
                targetTransform = null;
                state = State.Roaming;
            }

        }
        else if (targetType == "Building")
        {
            float sphereRadius = 1f; // Adjust the sphere radius as needed

            RaycastHit hit;

            Debug.Log("Trying to shoot a building");
            if (Physics.SphereCast(raycastPoint.position, sphereRadius, (targetTransform.position - raycastPoint.position).normalized, out hit, Mathf.Infinity, buildingsLayerMask))
            {
                Debug.Log("Shot" + targetTransform);
                if (enemyAISO.enemyType == EnemyAISO.EnemyType.Ranged)
                {
                    GameObject mzlLFlash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
                    Destroy(mzlLFlash, .5f);

                }
                else if (enemyAISO.enemyType == EnemyAISO.EnemyType.Melee)
                {
                    GameObject mleSlash = Instantiate(meleeSlash, firePoint.position, firePoint.rotation);
                    Destroy(mleSlash, .5f);
                }

                float health = 0;
                float buildingHealth;
                Debug.Log("SphereCast hit something on the player layer");
                if (hit.transform.GetComponent<Building>())
                {
                    buildingHealth = (hit.transform.GetComponent<Building>().buildingHealth);
                    health = buildingHealth - enemyAISO.damageAmountPerAttack;
                    hit.transform.GetComponent<Building>().takeDamage(enemyAISO.damageAmountPerAttack);
                }

                if (health <= 0)
                {
                    state = State.Roaming;
                    
                } else
                {
                    state = State.ChaseTarget;
                }
                

                // Additional actions for hitting a player can be added here
            } else
            {
                targetTransform = null;
                state = State.Roaming;
            }
            
        }
        

        //no bullets
        //just a muzzle flash
    }



    private Vector3 GetRoamingPosition()
    {
        //float range = 5f; // Half of the desired square area size
        return startingPosition + new Vector3(Random.Range(-enemyAISO.romingRange, enemyAISO.romingRange), 0f, Random.Range(-enemyAISO.romingRange, enemyAISO.romingRange));
    }


    private void FindTarget()
    {
        if (targetTransform != null)
        {
            if (Vector3.Distance(transform.position, targetTransform.position) < enemyAISO.searchRange)
            {
                // Player within target range
                state = State.ChaseTarget;
            }
        }

    }


}
