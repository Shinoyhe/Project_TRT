using UnityEngine;

public class BarterState_CheckInfo : BarterBaseState
{
    // ============================================================================================
    //
    //                   TODO: THIS CLASS HAS NO FUNCTIONALITY RIGHT NOW!
    //
    // ============================================================================================

    public BarterState_CheckInfo(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState) 
    {
        _machine.Dir.SetWillingnessFrozen(true);
        _machine.CurrentState = _machine.TurnOppState;
    }

    public override void UpdateState() {}

    public override void Exit() 
    {
        _machine.Dir.SetWillingnessFrozen(false);
        _machine.Dir.AfterCheckInfoState();
    }
}
