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

    public AstarAI myAstarAI;

    [SerializeField] private Animator unitAnimator;

    void Start()
    {

        unitHealth = unitSO.startingHealth;
        unitHealthbar.UpdateHealthBar(unitSO.startingHealth, unitHealth);
        unitHealthbar.gameObject.SetActive(false);

        UnitSelection.Instance.unitList.Add(this.gameObject);

        currentState = UnitState.Idle;

        myAstarAI = GetComponent<AstarAI>();

    }

    private void Update()
    {

        if (myAstarAI.isAtDestination)
        {
            if (currentState != UnitState.Shooting)
            {
                currentState = UnitState.Idle;
            } else
            {
                currentState = UnitState.Shooting;
            }

        } else
        {
            currentState = UnitState.Moving;
        }

        switch (currentState)
        {
            case UnitState.Idle:
                muzzleFlash.SetActive(false);
                unitAnimator.Play("Idle");
                CheckForEnemies();
                
                
                
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

                muzzleFlash.SetActive(false);
                currentTarget = null;
                unitAnimator.Play("UnitBob");

                break;

            default:
                break;
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
        
            //float sphereRadius = .25f; // Adjust the sphere radius as needed

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
                GameManager.instance.enemiesKilledCount++;
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
            GameManager.instance.changeCurrentPopulation(-(int)unitSO.populationCost);

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

}
