using UnityEngine;

public class BarterState_EndWin : BarterBaseState
{
    public BarterState_EndWin(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
        Debug.Log("Player wins!");
    }

    public override void UpdateState()
    {
        
    }

    public override void Exit() {}
}
