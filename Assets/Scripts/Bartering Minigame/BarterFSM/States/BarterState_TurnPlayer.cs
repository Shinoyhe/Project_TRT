using System.Collections;
using UnityEngine;

public class BarterState_TurnPlayer : BarterBaseState
{
    // State Methods ==============================================================================

    /// <summary>
    /// Returns a new instance of this state.
    /// </summary>
    /// <param name="stateName">string - the internal ID of this state.</param>
    /// <param name="machine">BarterStateMachine - the FSM that holds this state.</param>
    public BarterState_TurnPlayer(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
        // Cache references for code density reasons.
        int cardsToPlay = _machine.Dir.CardsToPlay;
        var handList = _machine.PlayerCardUser.HandList;
        
        // The player must have enough cards in its hand to play. If we don't, exit.
        if (cardsToPlay > handList.Count) {
            Debug.LogError("BarterState_TurnPlayer Error: Enter failed. The BarterDirector wants "
                        + $"the player CardUser to play more cards ({cardsToPlay}) than it has in "
                        + $"its hand ({handList.Count})");
            return;
        }

        // Once the player has submitted all cards, proceed.
        _machine.Dir.OnPlayerAllCardsSet -= SubmitPlayerCards;
        _machine.Dir.OnPlayerAllCardsSet += SubmitPlayerCards;

        // Unlock the handcontroller, allowing the user to interact with cards.
        if (_machine.Dir.PlayerHandController != null && 
            _machine.Dir.PlayerHandController.isActiveAndEnabled) {
            _machine.Dir.PlayerHandController.Unlock();
        }
    }

    public override void UpdateState()
    {
        // Decay willingness over time and lose if it hits zero.

        _machine.Dir.DecayWillingness();

        if (_machine.Dir.GetWillingness() <= 0) {
            _machine.Dir.StopAllCoroutines();
            _machine.CurrentState = _machine.EndLossState;
        }
    }

    public override void Exit() 
    {
        // Unsubscribe from the submission action and lock the controller, stopping player input.
        _machine.Dir.OnPlayerAllCardsSet -= SubmitPlayerCards;
        if (_machine.Dir.PlayerHandController != null && 
            _machine.Dir.PlayerHandController.isActiveAndEnabled) {
            _machine.Dir.PlayerHandController.Lock();
        }
    }

    // Callback Methods ===========================================================================

    private void SubmitPlayerCards()
    {
        // SubmitPlayerCards is called via callback when all cards have been submitted!
        // We use it to change state.
        // ================

        _machine.CurrentState = _machine.ComputeState;
    }
}
