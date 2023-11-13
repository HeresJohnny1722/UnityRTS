using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGatheringState : UnitBaseState
{
    public override void EnterState(UnitStateManager unit)
    {
        Debug.Log("Hello from the Gathering State");
    }

    public override void UpdateState(UnitStateManager unit)
    {

    }

    public override void OnCollisionEnter(UnitStateManager unit)
    {

    }
}
