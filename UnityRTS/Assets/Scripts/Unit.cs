using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages the behavior and interactions of units in the game, including shooting, movement, and selection.
/// </summary>
public class Unit : MonoBehaviour
{
    public enum UnitState
    {
        Idle,
        Shooting,
        Moving,
    }

    public UnitState currentState;

    public UnitSO unitSO;

    public LayerMask enemyLayerMask;
    public LayerMask buildingLayerMask;

    public Vector3 moveToPosition;

    public TroopHit troopHit;

    public AstarAI myAstarAI;

    [SerializeField] private Animator unitAnimator;

    public Healthbar unitHealthbar;
    [SerializeField] private GameObject unitFloorHighlight;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject deathEffect;

    [SerializeField] Transform muzzlePoint;
    [SerializeField] Transform raycastPoint;
    public Transform currentTarget;
    public Transform targetBuilding;

    public float unitHealth;
    private float nextShootTime;

    public bool isMoving;

    void Start()
    {
        SetupUnit();
    }

    /// <summary>
    /// Sets up the initial state and registers the unit with UnitSelection.
    /// </summary>
    private void SetupUnit()
    {
        //SetupHealth();

        UnitSelection.Instance.unitList.Add(this.gameObject);

        currentState = UnitState.Idle;

        myAstarAI = GetComponent<AstarAI>();
    }

    /// <summary>
    /// Sets up the initial health of the unit.
    /// </summary>
    public void SetupHealth()
    {
        unitHealth = unitSO.startingHealth;
        unitHealthbar.UpdateHealthBar(unitSO.startingHealth, unitHealth);
        unitHealthbar.gameObject.SetActive(false);
    }

    /// <summary>
    /// Update method is called every frame and manages the unit's behavior based on its state.
    /// </summary>
    private void Update()
    {
        // Checks if the unit is moving or not, if moving it should be in the moving state, else it should check if its idle or if its shooting
        if (myAstarAI.isAtDestination)
        {
            if (currentState != UnitState.Shooting)
            {
                currentState = UnitState.Idle;
            }
            else
            {
                currentState = UnitState.Shooting;
            }

        }
        else
        {
            currentState = UnitState.Moving;
        }

        // Check the unit's state and perform corresponding actions
        switch (currentState)
        {
            case UnitState.Idle:

                unitAnimator.Play("Idle");
                CheckForEnemies();

                break;

            case UnitState.Shooting:
                if (currentTarget == null)
                {
                    currentState = UnitState.Idle;
                    break;
                }
                    
                RotateToTarget();

                if (Time.time > nextShootTime)
                {

                    ShootEnemy();
                    nextShootTime = Time.time + unitSO.fireRate;

                }

                break;

            case UnitState.Moving:

                currentTarget = null;
                unitAnimator.Play("UnitBob");

                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Checks for nearby enemies and transitions to the shooting state if an enemy is found.
    /// </summary>
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

    /// <summary>
    /// Finds the closest enemy from the provided colliders.
    /// </summary>
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

    /// <summary>
    /// Rotates the unit towards its current target's transform.
    /// </summary>
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

    /// <summary>
    /// Shoots the current enemy target by shooting a raycast and check if it hit something on the enemy layer mask
    /// </summary>
    private void ShootEnemy()
    {

        RaycastHit hit;


        if (Physics.Raycast(raycastPoint.position, (currentTarget.position - raycastPoint.position).normalized, out hit, Mathf.Infinity, enemyLayerMask))
        {
            if (currentTarget.GetComponent<EnemyAI>() != null)
            {
                //Muzzle Flash
                //muzzleFlash.SetActive(false);
                //muzzleFlash.SetActive(true);

                //Sound Effect
                SoundFeedback.Instance.PlaySound(SoundType.Gun);

                currentTarget.GetComponent<EnemyAI>().TakeDamage(unitSO.attackDamage);

            }

            if (currentTarget.GetComponent<EnemyAI>().enemyHealth <= 0)
            {
                GameManager.instance.enemiesKilledCount++;
                currentTarget = null;
            }

        }

        // Check if the enemy is still in attack range
        if (currentTarget == null || Vector3.Distance(transform.position, currentTarget.position) > unitSO.attackRange)
        {
            // Enemy left the attack radius or is dead, find a new enemy
            currentState = UnitState.Idle;
            currentTarget = null;
        }

    }

    /// <summary>
    /// Deals damage to the unit and manages death-related actions.
    /// </summary>
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

    /// <summary>
    /// Selects the unit, showing floor highlight and health bar.
    /// </summary>
    public void selectUnit()
    {
        unitFloorHighlight.SetActive(true);

        if (unitSO.startingHealth == unitHealth)
        {
            unitHealthbar.gameObject.SetActive(false);

        }
        else
        {
            unitHealthbar.gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// Deselects the unit, hiding floor highlight and health bar.
    /// </summary>
    public void deselectUnit()
    {
        unitFloorHighlight.SetActive(false);
        unitHealthbar.gameObject.SetActive(false);
    }

}
