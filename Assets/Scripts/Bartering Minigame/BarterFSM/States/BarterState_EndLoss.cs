using UnityEngine;

public class BarterState_EndLoss : BarterBaseState
{
    public BarterState_EndLoss(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
        Debug.Log("Player loses!");
    }

    public override void UpdateState()
    {
        
    }

    public override void Exit() {}
}
