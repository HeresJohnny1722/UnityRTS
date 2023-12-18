using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{

    public enum State
    {
        ChaseTarget,
        ShootingTarget,

    }

    private float nextShootTime;
    public State state;

    public Transform targetTransform;

    [SerializeField] private Healthbar enemyHealthbar;

    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject meleeSlash;
    [SerializeField] GameObject deathEffect;

    public string targetType = "Building";

    public LayerMask playersLayerMask;
    public LayerMask buildingsLayerMask;

    [SerializeField] Transform firePoint;
    [SerializeField] Transform raycastPoint;
    [SerializeField] EnemyAISO enemyAISO;

    public float enemyHealth;

    public TroopHit troopHit;

    private AstarAI myAstarAI;

    void Start()
    {
        myAstarAI = GetComponent<AstarAI>();
        enemyHealth = enemyAISO.startingHealth;
        GameManager.instance.enemies.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

            CheckForUnitsAndBuildingsInRange();
            if (targetTransform != null)
            {
                if (targetType == "Unit")
            {
                myAstarAI.ai.destination = targetTransform.position;
            } else if (targetType == "Building")
            {
                myAstarAI.ai.destination = targetTransform.GetChild(1).position;
            }
                

                if (myAstarAI.isAtDestination)
            {
                

                    // Target within attack range
                    if (Time.time > nextShootTime)
                    {


                        state = State.ShootingTarget;
                        AttackUnit();

                        nextShootTime = Time.time + enemyAISO.fireRate;

                    }
                
            }

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
                if (Physics.Raycast(raycastPoint.position, (targetTransform.GetChild(1).position - raycastPoint.position).normalized, out hit, buildingsLayerMask))
                {
                    if (enemyAISO.enemyType == EnemyAISO.EnemyType.Melee)
                    {
                        meleeSlash.SetActive(false);
                        meleeSlash.SetActive(true);
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

    }

    public void takeDamage(float damageAmount)
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
}
