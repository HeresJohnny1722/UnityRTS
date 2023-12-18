using UnityEngine;
using System.Collections;
using Pathfinding;

public class AstarAI : MonoBehaviour
{
    public bool isAtDestination;

    public IAstarAI ai;
    
    //Transform tr;

    private void Awake()
    {
        //base.Awake();
        ai = GetComponent<IAstarAI>();
        //tr = GetComponent<Transform>();
    }
    void Update()
    {
        if (ai.reachedEndOfPath)
        {
            if (!isAtDestination) //OnTargetReached();
            isAtDestination = true;
        }
        else isAtDestination = false;
    }
}
