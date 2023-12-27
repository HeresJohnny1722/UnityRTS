using UnityEngine;
using System.Collections;
using Pathfinding;

public class AstarAI : MonoBehaviour
{
    public bool isAtDestination;

    public IAstarAI ai;
    public AIPath aiPath;
    
    //Transform tr;

    private void Awake()
    {
        //base.Awake();
        ai = GetComponent<IAstarAI>();
        aiPath = GetComponent<AIPath>();
        //tr = GetComponent<Transform>();
    }
    void Update()
    {
        if (ai.reachedEndOfPath)
        {
            if (!isAtDestination) //OnTargetReached();
                isAtDestination = true;
        }
        else
        {
            isAtDestination = false;
        }
    }
}
