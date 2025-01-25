using UnityEngine;
using System.Linq;

public class BarterState_Init : BarterBaseState
{
    public BarterState_Init(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
        // Initialize the barter responses dictionary. We only need to do this once per minigame.
        _machine.Dir.BarterResponses.Initialize();

        // Likewise.
        _machine.OppCardUser.Initialize();
        _machine.PlayerCardUser.Initialize();

        _machine.OppCardUser.Shuffle(CardUser.CardPile.DrawPile);
        _machine.PlayerCardUser.Shuffle(CardUser.CardPile.DrawPile);

        _machine.PlayerCardUser.DrawHand();
        _machine.OppCardUser.DrawHand();

        // TODO: Lock player input for card selection

        _machine.CurrentState = _machine.TurnOppState;
    }

    public override void UpdateState() {}

    public override void Exit() {}
}
