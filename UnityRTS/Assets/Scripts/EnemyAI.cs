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
                navMeshAgent.SetDestination(playerTransform.position);

                //aimShootAnims.SetAimTarget(playerTransform);

                float attackRange = 3f;
                if (Vector3.Distance(transform.position, playerTransform.position) < attackRange)
                {
                    // Target within attack range
                    if (Time.time > nextShootTime)
                    {
                        navMeshAgent.isStopped = true;
                        state = State.ShootingTarget;
                        /*aimShootAnims.ShootTarget(playerTransform.position, () => {
                            
                        });*/
                        //shoot target
                        state = State.ChaseTarget;
                        float fireRate = 0.15f;
                        nextShootTime = Time.time + fireRate;
                    }
                }

                float stopChaseDistance = 80f;
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
