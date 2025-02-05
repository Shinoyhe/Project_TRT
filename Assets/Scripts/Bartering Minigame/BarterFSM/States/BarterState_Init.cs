using UnityEngine;
using System.Linq;

public class BarterState_Init : BarterBaseState
{
    // State Methods ==============================================================================
    
    /// <summary>
    /// Returns a new instance of this state.
    /// </summary>
    /// <param name="stateName">string - the internal ID of this state.</param>
    /// <param name="machine">BarterStateMachine - the FSM that holds this state.</param>
    public BarterState_Init(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
        // Initialize the barter responses dictionary. We only need to do this once per minigame.
        _machine.Dir.BarterResponses.Initialize();

        // Likewise.
        _machine.OppCardUser.Initialize();
        _machine.PlayerCardUser.Initialize();

        // Initialize both CardUsers.
        _machine.OppCardUser.Shuffle(CardUser.CardPile.DrawPile);
        _machine.PlayerCardUser.Shuffle(CardUser.CardPile.DrawPile);
        _machine.OppCardUser.DrawHand();
        _machine.PlayerCardUser.DrawHand();

        // Lock player input for card selection.
        if (_machine.Dir.PlayerHandController != null && 
            _machine.Dir.PlayerHandController.isActiveAndEnabled) {
            _machine.Dir.PlayerHandController.Lock();
        }

        // Begin!
        _machine.CurrentState = _machine.TurnOppState;
    }

    public override void UpdateState() {}

    public override void Exit() {}
}
