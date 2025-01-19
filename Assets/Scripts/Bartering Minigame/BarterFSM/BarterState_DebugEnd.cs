using UnityEngine;

public class BarterState_DebugEnd : BarterBaseState
{
    // Timer variables
    private readonly float _duration;
    private float _elapsed;

    public BarterState_DebugEnd(string _stateName, BarterStateMachine _machine, float duration)
           : base(_stateName, _machine)
    {
        _duration = duration;
        _elapsed = 0;
    }

    public override void Enter(BarterBaseState previousState)
    {
        // _machine.PlayerCardUser.DiscardHand();
        // _machine.OppCardUser.DiscardHand();

        // _machine.PlayerCardUser.ShuffleDiscardIntoDrawpile();
        // _machine.OppCardUser.ShuffleDiscardIntoDrawpile();

        _elapsed = 0;
    }

    public override void UpdateState()
    {
        if (_elapsed > _duration) {
            _machine.CurrentState = _machine.InitState; 
            return;
        } else {
            _elapsed += Time.deltaTime;
        }
    }

    public override void Exit() 
    {
        Debug.Log("BarterState_DebugEnd Message: We're exiting the state! Yaha!");
    }
}
