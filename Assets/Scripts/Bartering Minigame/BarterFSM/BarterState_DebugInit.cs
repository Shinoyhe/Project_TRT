using UnityEngine;

public class BarterState_DebugInit : BarterBaseState
{
    // Timer variables
    private readonly float _duration;
    private float _elapsed;

    public BarterState_DebugInit(string _stateName, BarterStateMachine _machine, float duration)
           : base(_stateName, _machine)
    {
        _duration = duration;
        _elapsed = 0;
    }

    public override void Enter(BarterBaseState previousState)
    {
        // _machine.PlayerCardUser.Shuffle(CardUser.CardPile.DrawPile);
        // _machine.OppCardUser.Shuffle(CardUser.CardPile.DrawPile);

        // _machine.PlayerCardUser.DrawHand();
        // _machine.OppCardUser.DrawHand();

        _elapsed = 0;
    }

    public override void UpdateState()
    {
        if (_elapsed > _duration) {
            _machine.CurrentState = _machine.EndState; 
            return;
        } else {
            _elapsed += Time.deltaTime;
        }
    }

    public override void Exit() {}
}
