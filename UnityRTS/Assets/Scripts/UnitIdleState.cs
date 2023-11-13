using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitIdleState : UnitBaseState
{
    public override void EnterState(UnitStateManager unit)
    {
        Debug.Log("Hello from the idle state");
    }

    public override void UpdateState(UnitStateManager unit)
    {
        //Play unit idle animation
        unit.gameObject.GetComponent<Animator>().enabled = true;
        Debug.Log("just idling around");
        unit.SwitchState(unit.GatheringState);
        //want it to switch to building/gathering/ which means we have to move first
    }

    public override void OnCollisionEnter(UnitStateManager unit)
    {

    }
}
