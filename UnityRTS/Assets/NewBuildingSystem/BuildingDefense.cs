using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDefense : MonoBehaviour
{
    private Building building;

    public bool hasTurretMount;
    public bool turretIsUnit;
    public GameObject turretObject;

    public Animator animator;

    public Transform currentTarget;
    public Transform raycastPoint;

    public LayerMask enemyLayerMask;

    [SerializeField] private Transform flashTransform;
    [SerializeField] private GameObject attackFlash;
    [SerializeField] private float flashDistance = 1.0f;

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

            if (currentTarget != null)
            {
                //RotateToTarget();
                //Rotate turret
                if (hasTurretMount)
                {
                    RotateToTarget();
                }

                if (Time.time > nextShootTime)
                {
                    attackFlash.SetActive(false);
                    ShootEnemy();

                    nextShootTime = Time.time + building.buildingSO.fireRate;

                }

            }
            else
            {
                attackFlash.SetActive(false);
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

        //float sphereRadius = .25f; // Adjust the sphere radius as needed

        RaycastHit hit;


        if (Physics.Raycast(raycastPoint.position, (currentTarget.position - raycastPoint.position).normalized, out hit, Mathf.Infinity, enemyLayerMask))
        {


            if (currentTarget.GetComponent<EnemyAI>() != null)
            {
                if (building.buildingSO.bulletsExplode)
                {
                    

                    GameObject explosionParticle = Instantiate(building.buildingSO.explodeParticleSystem, hit.transform.position, Quaternion.identity);
                    Destroy(explosionParticle, 5f);

                    //animator.Play("CannonShoot");
                    
                   // animator.Play("IdleCannon");
                    Debug.Log("Shooting cannon");

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


                    //Debug.Log("SphereCast hit something WITH A ENEMYAI");

                    /*Vector3 direction = (currentTarget.position - raycastPoint.position).normalized;

                    // Calculate the position for attackFlash
                    Vector3 flashPosition = raycastPoint.position + flashDistance * direction;

                    // Set the attack flash's position
                    if (hasTurretMount)
                    {
                        attackFlash.transform.position = flashTransform.transform.position;
                    }
                    else
                    {
                        attackFlash.transform.position = new Vector3(flashPosition.x, raycastPoint.position.y, flashPosition.z);
                    }
                    */

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


            // Additional actions for hitting a player can be added here
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
