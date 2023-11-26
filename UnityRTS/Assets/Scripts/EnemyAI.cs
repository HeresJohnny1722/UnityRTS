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

    public Transform playerTransform;

    public LayerMask playersLayerMask;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float bulletSpeed;

    [SerializeField] float rotationSpeed = 5f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //aimShootAnims = GetComponent<IAimShootAnims>();
        state = State.Roaming;
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

                float reachedPositionDistance = 1f;
                if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance)
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

                float attackRange = 3f;
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
                            float fireRate = 0.15f;
                            nextShootTime = Time.time + fireRate;
                        }));
                    }
                } else
                {
                    navMeshAgent.SetDestination(playerTransform.position);
                }

                float stopChaseDistance = 15f;
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

                reachedPositionDistance = 10f;
                if (Vector3.Distance(transform.position, startingPosition) < reachedPositionDistance)
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
        float sphereRadius = 0.5f; // Adjust the sphere radius as needed

        RaycastHit hit;

        if (Physics.SphereCast(firePoint.position, sphereRadius, firePoint.forward, out hit, Mathf.Infinity, playersLayerMask))
        {
            Debug.Log("SphereCast hit something on the player layer");
            // Additional actions for hitting a player can be added here
        }

        // Perform the shooting logic (instantiating a bullet, applying force, destroying the bullet, etc.)
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation) as GameObject;
        Rigidbody bulletRig = bulletObj.GetComponent<Rigidbody>();
        bulletRig.AddForce(bulletRig.transform.forward * bulletSpeed * Time.deltaTime);
        Destroy(bulletObj, 1f);
    }



    private Vector3 GetRoamingPosition()
    {
        float range = 5f; // Half of the desired square area size
        return startingPosition + new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
    }


    private void FindTarget()
    {
        float targetRange = 5f;
        if (Vector3.Distance(transform.position, playerTransform.position) < targetRange)
        {
            // Player within target range
            state = State.ChaseTarget;
        }
    }
}
