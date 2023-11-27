using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CodeMonkey.Utils;

public class EnemyAI : MonoBehaviour
{
    private enum State
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
    private State state;

    [SerializeField] GameObject muzzleFlash;

    public Transform playerTransform;

    public LayerMask playersLayerMask;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] EnemyAISO enemyAISO;

    private float startingHealth;
    private float fireRate;
    private float attackRange;
    private float roamingReachedPositionDistance;
    private float rotationSpeed;
    private float bulletSpeed;
    private float stopChaseDistance;
    private float reachedStartPositionDistance;
    private float searchRange;

    private void Awake()
    {
        //muzzleFlash.SetActive(false);
        navMeshAgent = GetComponent<NavMeshAgent>();
        //aimShootAnims = GetComponent<IAimShootAnims>();
        state = State.Roaming;
        startingHealth = enemyAISO.startingHealth;
        fireRate = enemyAISO.fireRate;
        attackRange = enemyAISO.attackRange;
        roamingReachedPositionDistance = enemyAISO.roamingReachedPositionDistance;
        rotationSpeed = enemyAISO.rotationSpeed;
        bulletSpeed = enemyAISO.bulletSpeed;
        stopChaseDistance = enemyAISO.stopChaseDistance;
        reachedStartPositionDistance = enemyAISO.reachedStartPositionDistance;
        searchRange = enemyAISO.searchRange;

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

                //float reachedPositionDistance = 1f;
                if (Vector3.Distance(transform.position, roamPosition) < roamingReachedPositionDistance)
                {
                    // Reached Roam Position
                    roamPosition = GetRoamingPosition();
                }

                FindTarget();
                break;
            case State.ChaseTarget:
                RotateTowardsPlayer();
                //navMeshAgent.SetDestination(playerTransform.position);

                //aimShootAnims.SetAimTarget(playerTransform);

                //float attackRange = 3f;
                if (Vector3.Distance(transform.position, playerTransform.position) < attackRange)
                {
                    // Target within attack range
                    if (Time.time > nextShootTime)
                    {
                        navMeshAgent.SetDestination(transform.position);
                        state = State.ShootingTarget;

                        // shoot target
                        Debug.Log("Enemy shooting");
                        ShootAtPlayer();

                        StartCoroutine(WaitAndContinue(2f, () =>
                        {
                            //navMeshAgent.isStopped = false;
                            state = State.ChaseTarget;
                            //float fireRate = 0.15f;
                            nextShootTime = Time.time + fireRate;
                        }));
                    }
                } else
                {
                    navMeshAgent.SetDestination(playerTransform.position);
                }

                //float stopChaseDistance = 15f;
                if (Vector3.Distance(transform.position, playerTransform.position) > stopChaseDistance)
                {
                    // Too far, stop chasing
                    state = State.GoingBackToStart;
                }
                break;
            case State.ShootingTarget:
                break;
            case State.GoingBackToStart:
                navMeshAgent.SetDestination(startingPosition);

                //reachedStartPositionDistance = 10f;
                if (Vector3.Distance(transform.position, startingPosition) < reachedStartPositionDistance)
                {
                    // Reached Start Position
                    state = State.Roaming;
                }
                break;
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0f, directionToPlayer.z));

        // Use Lerp to smoothly rotate the enemy towards the player
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private IEnumerator WaitAndContinue(float waitTime, System.Action onWaitComplete)
    {
        yield return new WaitForSeconds(waitTime);

        // Code to be executed after waiting for 'waitTime' seconds
        Debug.Log("Waited for " + waitTime + " seconds.");

        onWaitComplete?.Invoke(); // Invoke the provided action after the wait time
    }

    private void ShootAtPlayer()
    {
        //muzzleFlash.SetActive(true);
        GameObject mzlLFlash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
        float sphereRadius = 0.5f; // Adjust the sphere radius as needed

        RaycastHit hit;

        if (Physics.SphereCast(firePoint.position, sphereRadius, firePoint.forward, out hit, Mathf.Infinity, playersLayerMask))
        {
            Debug.Log("SphereCast hit something on the player layer");
            hit.transform.GetComponent<Unit>().takeDamage(enemyAISO.damageAmountPerBullet);
            // Additional actions for hitting a player can be added here
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
        
        if (Vector3.Distance(transform.position, playerTransform.position) < searchRange)
        {
            // Player within target range
            state = State.ChaseTarget;
        }
    }
}
