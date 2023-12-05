
using UnityEngine;

public abstract class UnitBaseState
{
    public abstract void EnterState(UnitStateManager unit);

    public abstract void UpdateState(UnitStateManager unit);

    public abstract void OnCollisionEnter(UnitStateManager unit);
}
