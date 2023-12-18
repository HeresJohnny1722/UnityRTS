using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CodeMonkey.Utils;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
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

    public TroopHit troopHit;

    private bool isWaiting;

    private AstarAI myAstarAI;



    ///public EnemyManager enemyManager;


    private void Awake()
    {

        
        //state = State.Roaming;
        enemyHealth = enemyAISO.startingHealth;
        myAstarAI = GetComponent<AstarAI>();
        


    }





    private void Start()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        InventoryManager.instance.enemies.Add(gameObject);
        //navMeshAgent.SetDestination(playerTransform.position);
    }

    public void takeDamage(float damageAmount)
    {
        enemyHealth -= damageAmount;

        if (enemyHealth <= 0)
        {

            GameObject deathEfct = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(deathEfct, 2f);

            Destroy(this.gameObject);
            InventoryManager.instance.enemies.Remove(gameObject);


        }

        troopHit.HitAnimation();
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
            /*case State.Roaming:
                //AstarAI myAstarAI = GetComponent<AstarAI>();
                myAstarAI.targetPosition = roamPosition;
                myAstarAI.StartPath();

                if (!isWaiting && Vector3.Distance(transform.position, roamPosition) < enemyAISO.roamingReachedPositionDistance)
                {
                    StartCoroutine(WaitAndChangeRoamingPosition());
                }

                CheckForUnitsAndBuildingsInRange();
                FindTarget();
                break;
            */
            case State.ChaseTarget:

                CheckForUnitsAndBuildingsInRange();
                FindTarget();

                if (targetTransform != null)
                {
                    RotateTowardsPlayerOrBuilding();
                    

                    if (targetType == "Unit")
                    {
                        
                        myAstarAI.targetPosition = targetTransform.position;
                        myAstarAI.StartPath();
                        
                        if (Vector3.Distance(transform.position, targetTransform.position) < enemyAISO.attackRange)
                        {
                            myAstarAI.targetPosition = transform.position;
                            myAstarAI.StartPath();

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
                        
                        myAstarAI.targetPosition = targetTransform.GetChild(1).position;
                        myAstarAI.StartPath();
                        if (Vector3.Distance(transform.position, targetTransform.GetChild(1).position) < enemyAISO.attackRange + targetTransform.GetComponent<BoxCollider>().size.x / 1.25)
                        {
                            //Debug.Log(targetTransform.GetComponent<BoxCollider>().size.x);
                            myAstarAI.targetPosition = transform.position;
                            myAstarAI.StartPath();

                            // Target within attack range
                            if (Time.time > nextShootTime)
                            {
//                                Debug.Log("Shooting a building");

                                state = State.ShootingTarget;
                                AttackUnit();

                                nextShootTime = Time.time + enemyAISO.fireRate;

                            }
                        }

                    }

                }
                //else
                //{
                   // state = State.Roaming;
                //}
                break;
            case State.ShootingTarget:
                break;
            case State.GoingBackToStart:
                
                myAstarAI.targetPosition = startingPosition;
                myAstarAI.StartPath();


                //reachedStartPositionDistance = 10f;
                if (Vector3.Distance(transform.position, startingPosition) < enemyAISO.reachedStartPositionDistance)
                {
                    // Reached Start Position
                    //state = State.Roaming;
                }
                break;
        }

    }

    private IEnumerator WaitAndChangeRoamingPosition()
    {
        isWaiting = true;

        // Wait for 2 seconds
        yield return new WaitForSeconds(Random.Range(3, 7));

        // Get the next roaming position after waiting
        roamPosition = GetRoamingPosition();

        // Reset the flag to allow movement again
        isWaiting = false;
    }

    private void RotateTowardsPlayerOrBuilding()
    {
        if (targetTransform != null)
        {
            if (targetType == "Unit")
            {
                Vector3 directionToPlayer = (targetTransform.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0f, directionToPlayer.z));

                // Use Lerp to smoothly rotate the enemy towards the player
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * enemyAISO.rotationSpeed);
            } else if (targetType == "Building")
            {
                Vector3 directionToPlayer = (targetTransform.GetChild(1).position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0f, directionToPlayer.z));

                // Use Lerp to smoothly rotate the enemy towards the player
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * enemyAISO.rotationSpeed);
            }
             
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


    private void AttackUnit()
    {
        RotateTowardsPlayerOrBuilding();
        //muzzleFlash.SetActive(true);



        if (targetType == "Unit")
        {

            //float sphereRadius = .25f; // Adjust the sphere radius as needed

            RaycastHit hit;


            if (Physics.Raycast(raycastPoint.position, (targetTransform.position - raycastPoint.position).normalized, out hit, playersLayerMask))
            {
                if (enemyAISO.enemyType == EnemyAISO.EnemyType.Melee)
                {
                    meleeSlash.SetActive(false);
                    meleeSlash.SetActive(true);
                }

                float health = 0;
                float unitHealth;
                //Debug.Log("SphereCast hit something on the player layer");
                if (targetTransform.GetComponent<Unit>())
                {
                    unitHealth = (targetTransform.GetComponent<Unit>().unitHealth);
                    health = unitHealth - enemyAISO.damageAmountPerAttack;
                    targetTransform.GetComponent<Unit>().takeDamage(enemyAISO.damageAmountPerAttack);
                }

                //if (health <= 0)
                //{
                    //state = State.Roaming;

                //}
                //else
                //{
                    state = State.ChaseTarget;
                //}


                // Additional actions for hitting a player can be added here
            }

            else
            {
                targetTransform = null;
                //state = State.Roaming;
            }

        }
        else if (targetType == "Building")
        {
            if (targetTransform.GetComponent<Building>().stage != 1)
            {


                //float sphereRadius = .5f; // Adjust the sphere radius as needed
                //float sphereRadius = targetTransform.GetComponent<BoxCollider>().size.x / 2f;

                RaycastHit hit;

//                Debug.Log("Trying to shoot a building");
                if (Physics.Raycast(raycastPoint.position, (targetTransform.GetChild(1).position - raycastPoint.position).normalized, out hit, buildingsLayerMask))
                {
                    if (enemyAISO.enemyType == EnemyAISO.EnemyType.Melee)
                    {
                        meleeSlash.SetActive(false);
                        meleeSlash.SetActive(true);
                    }

                    float health = 0;
                    float buildingHealth;
//                    Debug.Log("SphereCast hit something on the player layer");
                    if (targetTransform.GetComponent<Building>())
                    {
                        buildingHealth = (targetTransform.GetComponent<Building>().buildingHealth);
                        health = buildingHealth - enemyAISO.damageAmountPerAttack;
                        targetTransform.GetComponent<Building>().takeDamage(enemyAISO.damageAmountPerAttack);
                    }

                    //if (health <= 0)
                    //{
                        //state = State.Roaming;

                    //}
                    //else
                    //{
                        state = State.ChaseTarget;
                   // }


                    // Additional actions for hitting a player can be added here
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
            if (targetType == "Building")
            {

                if (Vector3.Distance(transform.position, targetTransform.GetChild(1).position) < enemyAISO.searchRange)
                {
                    // Player within target range
                    state = State.ChaseTarget;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, targetTransform.position) < enemyAISO.searchRange)
                {
                    // Player within target range
                    state = State.ChaseTarget;
                }
            }

        }

    }


}
