using System.Collections;
using UnityEngine;

public class BarterState_TurnPlayer : BarterBaseState
{
    // State Methods ==============================================================================

    public BarterState_TurnPlayer(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
        // Cache references.
        int cardsToPlay = _machine.Dir.CardsToPlay;
        var handList = _machine.PlayerCardUser.HandList;
        
        // The player must have enough cards in its hand to play. If we don't, exit.
        if (cardsToPlay > handList.Count) {
            Debug.LogError("BarterState_TurnPlayer Error: Enter failed. The BarterDirector wants "
                        + $"the player CardUser to play more cards ({cardsToPlay}) than it has in "
                        + $"its hand ({handList.Count})");
            return;
        }

        // TODO: Unlock player input

        // PlayerCardInput.OnSubmitCards -= SubmitPlayerCards;
        // PlayerCardInput.OnSubmitCards += SubmitPlayerCards;
        // PlayerCardInput.Unlock();

        // TODO: How to handle UI stuff?
    }

    public override void UpdateState()
    {
        _machine.Dir.DecayWillingness();

        if (_machine.Dir.GetWillingness() <= 0) {
            _machine.Dir.StopAllCoroutines();
            _machine.CurrentState = _machine.EndLossState;
        }
    }

    public override void Exit() 
    {
        // TODO: Lock player input

        // PlayerCardInput.OnSubmitCards -= SubmitPlayerCards;
        // PlayerCardInput.Lock();
    }

    // Callback Methods ===========================================================================

    private void SubmitPlayerCards(PlayingCard[] submittedCards)
    {
        // Validate the array.
        if (submittedCards.Length != _machine.Dir.CardsToPlay) {
            Debug.LogError("BarterState_TurnPlayer Error: SubmitPlayerCards failed. The provided "
                        + $"array has {submittedCards.Length} cards, but we were expecting "
                        + $"{_machine.Dir.CardsToPlay} cards.");
            return;
        }

        // Send the complete array of cards to the director.
        _machine.Dir.SetPlayerCards(submittedCards);

        _machine.CurrentState = _machine.ComputeState;
    }

    
}
