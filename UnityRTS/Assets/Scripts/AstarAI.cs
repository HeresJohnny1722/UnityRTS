using UnityEngine;
using System.Collections;
using Pathfinding;

/// <summary>
/// Manages A* pathfinding behavior for an AI-controlled unit, detecting when the unit reaches its destination.
/// </summary>
public class AstarAI : MonoBehaviour
{
    public bool isAtDestination;

    public IAstarAI ai;
    public AIPath aiPath;

    /// <summary>
    /// Initializes the A* AI and AIPath components on Awake.
    /// </summary>
    private void Awake()
    {
        ai = GetComponent<IAstarAI>();
        aiPath = GetComponent<AIPath>();
    }

    /// <summary>
    /// Checks whether the AI has reached the end of its path and updates the 'isAtDestination' flag accordingly.
    /// </summary>
    void Update()
    {
        if (ai.reachedEndOfPath)
        {
            if (!isAtDestination)
                isAtDestination = true;
        }
        else
        {
            isAtDestination = false;
        }
    }
}
