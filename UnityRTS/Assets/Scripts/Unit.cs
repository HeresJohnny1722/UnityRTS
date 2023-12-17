using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public UnitSO unitSO;

    [SerializeField] private GameObject unitFloorHighlight;
    [SerializeField] private Healthbar unitHealthbar;

    public GameObject muzzleFlash;

    [SerializeField] GameObject deathEffect;

    public float unitHealth;
    private float nextShootTime;

    [SerializeField] Transform muzzlePoint;
    [SerializeField] Transform raycastPoint;

    private NavMeshAgent myAgent;

    

    public enum UnitState
    {
        Idle,
        Shooting,
        Moving,
    }

    public UnitState currentState;

    public Transform currentTarget;
    public Transform targetBuilding;

    public bool isMoving;

    public LayerMask enemyLayerMask;
    public LayerMask buildingLayerMask;

    public Vector3 moveToPosition;

    public TroopHit troopHit;

    public float movingAnimationTimeWaitTime;

    public float bobbingHeight = 0.2f; // Adjust this value to control how much the unit bobs up and down
    public float bobbingSpeed = 1.0f; // Adjust this value to control how fast the unit bobs

    private bool isBobbingUp = false;
    private Vector3 originalPosition;

    void Start()
    {
      //  myAgent = GetComponent<NavMeshAgent>();
//        myAgent.speed = unitSO.speed;
        unitHealth = unitSO.startingHealth;
        unitHealthbar.UpdateHealthBar(unitSO.startingHealth, unitHealth);
        unitHealthbar.gameObject.SetActive(false);

        UnitSelection.Instance.unitList.Add(this.gameObject);

        //moveToPosition = transform.position;
        currentState = UnitState.Idle;

        originalPosition = transform.position;
        //previousPosition = transform.position;

    }

    private void Update()
    {


        

        AstarAI myAstarAI = GetComponent<AstarAI>();
        if (myAstarAI.isMoving)
        {
            currentState = UnitState.Moving;
        } else
        {
            if (currentState != UnitState.Shooting)
            {
                currentState = UnitState.Idle;
            }
            
        }



        switch (currentState)
        {
            case UnitState.Idle:
                muzzleFlash.SetActive(false);
                //if gunner
                if (unitSO.unitType == UnitSO.UnitType.Gunner)
                {
                    CheckForEnemies();
                } else if (unitSO.unitType == UnitSO.UnitType.Worker)
                {
                    //else if worker
                    CheckForBuildings();
                }
                
                
                break;

            case UnitState.Shooting:
                if (currentTarget != null)
                {
                    RotateToTarget();

                    if (Time.time > nextShootTime)
                    {
                        
                        ShootEnemy();

                        nextShootTime = Time.time + unitSO.fireRate;

                    }
                    
                } else
                {
                    currentState = UnitState.Idle;
                }
                
                break;

            case UnitState.Moving:
                // Handle moving state logic here if needed
                muzzleFlash.SetActive(false);
                currentTarget = null;
                originalPosition = transform.position;
                BobbingAnimation();
                //move animation



                break;

            default:
                break;
        }
    }

    private void BobbingAnimation()
    {
        float yOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;

        // Update the unit's position with the continuous bobbing effect
        transform.position = new Vector3(transform.position.x, originalPosition.y + yOffset, transform.position.z);
    }

    private void CheckForBuildings()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, unitSO.buildRange, buildingLayerMask);

        if (colliders.Length > 0)
        {
            Debug.Log("There are " + colliders.Length + " buildings in the unit's range");

            // Initialize closestBuilding and closestDistance
            Transform closestBuilding = null;
            float closestDistance = float.MaxValue;

            foreach (Collider collider in colliders)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.GetChild(1).position);

                // Check if the current building is closer than the previous closestBuilding
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBuilding = collider.transform;
                }
            }

            if (closestBuilding != null)
            {
                Building buildingBuilding = closestBuilding.GetComponent<Building>();
                if (buildingBuilding.buildingConstruction.isUnderConstruction)
                {
                    targetBuilding = closestBuilding;
                    Debug.Log("Need to enter " + targetBuilding.name);
                }
            }
            else
            {
                targetBuilding = null;
                Debug.Log("No valid target building found");
            }
        }
    }




    private void CheckForEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, unitSO.attackRange, enemyLayerMask);

        if (colliders.Length > 0)
        {
            // Found an enemy
            Transform closestEnemy = GetClosestEnemy(colliders);
            if (closestEnemy != null)
            {
                currentTarget = closestEnemy;
                currentState = UnitState.Shooting;
            }
        }
    }

    private Transform GetClosestEnemy(Collider[] colliders)
    {
        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = collider.transform;
            }
        }

        return closestEnemy;
    }

    private void RotateToTarget()
    {
        if (currentTarget != null)
        {
            Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0f, directionToTarget.z));

            // Use Slerp instead of Lerp to smoothly rotate towards the target continuously
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * unitSO.rotationSpeed);
        }
    }


    private void ShootEnemy()
    {
        
            float sphereRadius = .25f; // Adjust the sphere radius as needed

            RaycastHit hit;


            if (Physics.Raycast(raycastPoint.position, (currentTarget.position - raycastPoint.position).normalized, out hit, Mathf.Infinity, enemyLayerMask))
            {

                //Debug.Log("SphereCast hit something on the enemy layer");
                if (currentTarget.GetComponent<EnemyAI>() != null)
                {
                    //Debug.Log("SphereCast hit something WITH A ENEMYAI");

                    //GameObject mzlLFlash = Instantiate(muzzleFlash, muzzlePoint.position, muzzlePoint.rotation);
                    //Destroy(mzlLFlash, unitSO.fireRate);
                    //muzzleFlash.SetActive(false);
                    muzzleFlash.SetActive(true);

                    currentTarget.GetComponent<EnemyAI>().takeDamage(unitSO.attackDamage);
                    //Debug.Log(currentTarget.GetComponent<EnemyAI>().enemyHealth);



                    //Debug.Log(currentTarget.GetComponent<EnemyAI>().enemyHealth);
                }

                if (currentTarget.GetComponent<EnemyAI>().enemyHealth <= 0)
                {
                //state = State.Roaming;
                //FindTarget();
                //return 0;
                InventoryManager.instance.enemiesKilledCount++;
                    currentTarget = null;
                }


                // Additional actions for hitting a player can be added here
            }
            else
            {
                //Probably switch the target because the target is blocked probably
            }


            // Check if the enemy is still in attack range
            if (currentTarget == null || Vector3.Distance(transform.position, currentTarget.position) > unitSO.attackRange)
            {
                // Enemy left the attack radius or is dead, find a new enemy
                currentState = UnitState.Idle;
                currentTarget = null;
            }
        
        
    }


    public void takeDamage(float damageAmount)
    {
        troopHit.HitAnimation();
        unitHealth -= damageAmount;

        if (unitHealth <= 0)
        {
            
            deselectUnit();
            InventoryManager.instance.changeCurrentPopulation(-(int)unitSO.populationCost);

            if (UnitSelection.Instance.unitsSelected.Contains(this.gameObject))
            {
                UnitSelection.Instance.unitsSelected.Remove(this.gameObject);
            }
            
            UnitSelection.Instance.unitList.Remove(this.gameObject);


            GameObject deathEfct = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(deathEfct, 2f);

            Destroy(this.gameObject);

            

        }
        unitHealthbar.gameObject.SetActive(true);
        unitHealthbar.UpdateHealthBar(unitSO.startingHealth, unitHealth);
    }

    public void selectUnit()
    {
        unitFloorHighlight.SetActive(true);
        //only if health stuff

        if (unitSO.startingHealth == unitHealth)
        {
            unitHealthbar.gameObject.SetActive(false);

        } else
        {
            unitHealthbar.gameObject.SetActive(true);
        }
        
    }

    public void deselectUnit()
    {
        unitFloorHighlight.SetActive(false);
        unitHealthbar.gameObject.SetActive(false);
    }

    public bool isEnteringProduction = false;
    private Transform buildingTransform = null;

    public void productionBuildingStuff(Transform buildingToEnter)
    {
        myAgent = GetComponent<NavMeshAgent>();
        float radius = 1f;

        NavMesh.SamplePosition(buildingTransform.position, out NavMeshHit hit, radius, NavMesh.AllAreas);

        // Set destination for the agent to the closest point on the nav mesh
        myAgent.SetDestination(hit.position);

        //myAgent.SetDestination(buildingToEnter.position);
        buildingTransform = buildingToEnter;
    }



}
