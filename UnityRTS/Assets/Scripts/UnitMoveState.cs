using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveState : UnitBaseState
{
    public override void EnterState(UnitStateManager unit)
    {
        Debug.Log("Hello from the moving state");
    }

    public override void UpdateState(UnitStateManager unit)
    {
        //Play unit idle animation
        Debug.Log("just idling around");
    }

    public override void OnCollisionEnter(UnitStateManager unit)
    {

    }
}
