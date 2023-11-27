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

    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject meleeSlash;

    public Transform playerTransform;

    public LayerMask playersLayerMask;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform raycastPoint;
    [SerializeField] EnemyAISO enemyAISO;

    private float startingHealth;

    ///public EnemyManager enemyManager;


    private void Awake()
    {
        //enemyManager.enemyList.Add(gameObject);
        //muzzleFlash.SetActive(false);
        navMeshAgent = GetComponent<NavMeshAgent>();
        //aimShootAnims = GetComponent<IAimShootAnims>();
        state = State.Roaming;
        startingHealth = enemyAISO.startingHealth;
        

    }

    private void Start()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        //navMeshAgent.SetDestination(playerTransform.position);
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

                    CheckForUnitsInRange(); // Check for units before transitioning to ChaseTarget
                    FindTarget();
                break;

            case State.ChaseTarget:
                    RotateTowardsPlayer();
                //navMeshAgent.SetDestination(playerTransform.position);

                //aimShootAnims.SetAimTarget(playerTransform);

                //float attackRange = 3f;
                if (playerTransform != null)
                {
                    if (Vector3.Distance(transform.position, playerTransform.position) < enemyAISO.attackRange)
                    {
                        // Target within attack range
                        if (Time.time > nextShootTime)
                        {
                            navMeshAgent.SetDestination(transform.position);
                            state = State.ShootingTarget;

                            // shoot target
                            //Debug.Log("Enemy shooting");
                            float playerHealth = AttackUnit();


                            

                            StartCoroutine(WaitAndContinue(0f, () =>
                            {

                                if (playerHealth == 0)
                                {
                                    
                                    state = State.Roaming;
                                    //Means the player is dead
                                }
                                else if (playerHealth == -1)
                                {
                                    //Player is not dead
                                    state = State.ChaseTarget;
                                }
                                else if (playerHealth == 1)
                                {
                                    //Did not hit a unit
                                    state = State.ChaseTarget;
                                }

                                //navMeshAgent.isStopped = false;
                                //state = State.ChaseTarget;
                                //float fireRate = 0.15f;
                                nextShootTime = Time.time + enemyAISO.fireRate;
                            }));
                        }
                    }
                    else
                    {
                        navMeshAgent.SetDestination(playerTransform.position);
                    }


                    //float stopChaseDistance = 15f;
                    if (Vector3.Distance(transform.position, playerTransform.position) > enemyAISO.stopChaseDistance)
                    {
                        // Too far, stop chasing
                        state = State.GoingBackToStart;
                    }
                } else
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

    private void RotateTowardsPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
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

    private void CheckForUnitsInRange()
    {
        float closestDistance = float.MaxValue;
        Transform closestUnit = null;

        foreach (var unit in UnitSelection.Instance.unitList)
        {
            float distance = Vector3.Distance(transform.position, unit.transform.position);

            // Check if the unit is within the search radius and closer than the current closest unit
            if (distance < enemyAISO.searchRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestUnit = unit.transform;
            }
        }

        // If a closest unit is found, set playerTransform to the transform of that unit
        if (closestUnit != null)
        {
            playerTransform = closestUnit;
        }
    }

    private float AttackUnit()
    {
        RotateTowardsPlayer();
        //muzzleFlash.SetActive(true);
        if (enemyAISO.enemyType == EnemyAISO.EnemyType.Ranged)
        {
            GameObject mzlLFlash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
            Destroy(mzlLFlash, .5f);

        } else if (enemyAISO.enemyType == EnemyAISO.EnemyType.Melee)
        {
            GameObject mleSlash = Instantiate(meleeSlash, firePoint.position, firePoint.rotation);
            Destroy(mleSlash, .5f);
        }
        
        float sphereRadius = .25f; // Adjust the sphere radius as needed

        RaycastHit hit;

      
        if (Physics.SphereCast(raycastPoint.position, sphereRadius, raycastPoint.forward, out hit, Mathf.Infinity, playersLayerMask))
        {
            float health = 0;
            float unitHealth;
            Debug.Log("SphereCast hit something on the player layer");
            if (hit.transform.GetComponent<Unit>())
            {
                unitHealth = (hit.transform.GetComponent<Unit>().unitHealth);
                health = unitHealth - enemyAISO.damageAmountPerAttack;
                hit.transform.GetComponent<Unit>().takeDamage(enemyAISO.damageAmountPerAttack);
            }

            if (health <= 0)
            {
                //state = State.Roaming;
                //FindTarget();
                return 0;
            } else
            {

                //Unit is not dead
                return -1;
            }
            
            // Additional actions for hitting a player can be added here
        } else
        {
            //Did not hit a player
            return 1;
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
        if (playerTransform != null)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) < enemyAISO.searchRange)
            {
                // Player within target range
                state = State.ChaseTarget;
            }
        }
        
    }

    
}
