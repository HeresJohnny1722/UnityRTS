using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateManager : MonoBehaviour
{

    UnitBaseState currentState;
    public UnitIdleState IdleState = new UnitIdleState();
    //public UnitBuildingState BuildingState = new UnitBuildingState();
    public UnitGatheringState GatheringState = new UnitGatheringState();
    public UnitMoveState MovingState = new UnitMoveState();

    // Start is called before the first frame update
    void Start()
    {
        currentState = IdleState;

        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(UnitBaseState state)
    {
        currentState = state;
        state.EnterState(this);

    }
}
