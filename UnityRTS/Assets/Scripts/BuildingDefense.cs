using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDefense : MonoBehaviour
{
    private Building building;

    public bool hasTurretMount;
    public bool turretIsUnit;


    public GameObject turretObject;
    [SerializeField] private GameObject attackFlash;

    public Transform currentTarget;
    public Transform raycastPoint;
    [SerializeField] private Transform flashTransform;

    public LayerMask enemyLayerMask;

    private float nextShootTime;

    private void Awake()
    {
        building = this.GetComponent<Building>();
    }

    private void RotateToTarget()
    {
        Vector3 directionToTarget = currentTarget.position - turretObject.transform.position;
        directionToTarget.y = 0f; // Ensure rotation only happens in the horizontal plane


        Quaternion targetRotation;
        if (turretIsUnit)
        {
            targetRotation = Quaternion.LookRotation(directionToTarget) * Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(directionToTarget) * Quaternion.Euler(0f, 90f, 0f);
        }


        // Smoothly interpolate between the current rotation and the target rotation
        turretObject.transform.rotation = Quaternion.Slerp(turretObject.transform.rotation, targetRotation, Time.deltaTime * building.buildingSO.turretRotationSpeed);
    }

    private void Update()
    {
        if (!building.buildingConstruction.isUnderConstruction && GetComponent<BuildingManager>().isFixed)
        {
            CheckForEnemies();

            if (currentTarget == null)
                return;

            if (hasTurretMount)
            {
                RotateToTarget();
            }

            if (Time.time > nextShootTime)
            {

                ShootEnemy();

                nextShootTime = Time.time + building.buildingSO.fireRate;

            }

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
        RaycastHit hit;

        if (Physics.Raycast(raycastPoint.position, (currentTarget.position - raycastPoint.position).normalized, out hit, Mathf.Infinity, enemyLayerMask))
        {
            if (currentTarget.GetComponent<EnemyAI>() == null)
                return;


            if (building.buildingSO.bulletsExplode)
            {


                GameObject explosionParticle = Instantiate(building.buildingSO.explodeParticleSystem, hit.transform.position, Quaternion.identity);
                Destroy(explosionParticle, 5f);

                attackFlash.SetActive(false);
                attackFlash.SetActive(true);

                Collider[] hitColliders = Physics.OverlapSphere(hit.transform.position, building.buildingSO.explodeRadius, enemyLayerMask);

                foreach (var collider in hitColliders)
                {
                    if (collider.GetComponent<EnemyAI>() != null)
                    {
                        collider.GetComponent<EnemyAI>().TakeDamage(building.buildingSO.attackDamage);

                        if (collider.GetComponent<EnemyAI>().enemyHealth <= 0)
                        {
                            GameManager.instance.enemiesKilledCount++;
                            currentTarget = null;
                        }

                    }
                }

            }
            else
            {
                attackFlash.SetActive(false);
                attackFlash.SetActive(true);

                currentTarget.GetComponent<EnemyAI>().TakeDamage(building.buildingSO.attackDamage);

                if (currentTarget.GetComponent<EnemyAI>().enemyHealth <= 0)
                {
                    GameManager.instance.enemiesKilledCount++;
                    currentTarget = null;
                }
            }
        }

        // Check if the enemy is still in attack range
        if (currentTarget == null || Vector3.Distance(transform.position, currentTarget.position) > building.buildingSO.attackRange)
        {
            // Enemy left the attack radius or is dead, find a new enemy
            currentTarget = null;
        }
    }

}
