using UnityEngine;
using System.Collections;
using Pathfinding;

public class AstarAI : MonoBehaviour
{
    public bool isAtDestination;

    public IAstarAI ai;
    public AIPath aiPath;

    private void Awake()
    {
        ai = GetComponent<IAstarAI>();
        aiPath = GetComponent<AIPath>();
    }
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
