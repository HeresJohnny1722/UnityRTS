using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDefense : MonoBehaviour
{
    private Building building;

    public Transform currentTarget;
    public Transform raycastPoint;

    public LayerMask enemyLayerMask;

    [SerializeField] private GameObject attackFlash;
    [SerializeField] private float flashDistance = 1.0f;

    private float nextShootTime;

    private void Awake()
    {
        building = this.GetComponent<Building>();
    }

    private void Update()
    {
        CheckForEnemies();

        if (currentTarget != null)
        {
            //RotateToTarget();

            if (Time.time > nextShootTime)
            {
                attackFlash.SetActive(false);
                ShootEnemy();

                nextShootTime = Time.time + building.buildingSO.fireRate;

            }

        } else
        {
            attackFlash.SetActive(false);
        }

    }

    private void CheckForEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, building.buildingSO.attackRange, enemyLayerMask);

        if (colliders.Length > 0)
        {
            // Found an enemy
            Transform closestEnemy = GetClosestEnemy(colliders);
            if (closestEnemy != null)
            {
                currentTarget = closestEnemy;
                
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

    private void ShootEnemy()
    {

        float sphereRadius = .25f; // Adjust the sphere radius as needed

        RaycastHit hit;


        if (Physics.SphereCast(raycastPoint.position, sphereRadius, (currentTarget.position - raycastPoint.position).normalized, out hit, Mathf.Infinity, enemyLayerMask))
        {

            
            if (currentTarget.GetComponent<EnemyAI>() != null)
            {
                Debug.Log("SphereCast hit something WITH A ENEMYAI");

                Vector3 direction = (currentTarget.position - raycastPoint.position).normalized;

                // Calculate the position for attackFlash
                Vector3 flashPosition = raycastPoint.position + flashDistance * direction;

                // Set the attack flash's position
                attackFlash.transform.position = new Vector3(flashPosition.x, raycastPoint.position.y, flashPosition.z);

                
                attackFlash.SetActive(true);

                currentTarget.GetComponent<EnemyAI>().takeDamage(building.buildingSO.attackDamage);
            }

            if (currentTarget.GetComponent<EnemyAI>().enemyHealth <= 0)
            {
                
                currentTarget = null;
            }


            // Additional actions for hitting a player can be added here
        }
        else
        {
            //Probably switch the target because the target is blocked probably
        }


        // Check if the enemy is still in attack range
        if (currentTarget == null || Vector3.Distance(transform.position, currentTarget.position) > building.buildingSO.attackRange)
        {
            // Enemy left the attack radius or is dead, find a new enemy
            //currentState = UnitState.Idle;
            currentTarget = null;
        }


    }

}
