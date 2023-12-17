using UnityEngine;
using System.Collections;
using Pathfinding;

public class AstarAI : MonoBehaviour
{
    public Vector3 targetPosition;
    private Seeker seeker;
    private CharacterController controller;
    public Path path;
    public float speed = 100;
    public float rotationSpeed = 5; // Adjust the rotation speed as needed
    public float nextWaypointDistance = 0;
    private int currentWaypoint = 0;
    public bool isMoving = false;

    public void StartPath()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            isMoving = false;
        }
    }

    public void Update()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            isMoving = false;
            return;
        }

        isMoving = true;

        IAstarAI ai = GetComponent<IAstarAI>();

        if (ai.reachedEndOfPath)
        {
            isMoving = false;
        } else
        {
            isMoving = true;
        }
        /*
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;

        // Apply constant speed to the direction
        dir *= speed * Time.deltaTime;
        controller.SimpleMove(dir);

        // Rotate to face the targetPosition on the Y-axis
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            // Keep only the Y rotation
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if close enough to the next waypoint
        float distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (distanceToWaypoint < nextWaypointDistance)
        {
            currentWaypoint++;
        }*/
    }
}
