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

        // TODO: Subscribe
        _machine.Dir.OnPlayerAllCardsSet -= SubmitPlayerCards;
        _machine.Dir.OnPlayerAllCardsSet += SubmitPlayerCards;

        _machine.Dir.PlayerHandController.Unlock();

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
        _machine.Dir.OnPlayerAllCardsSet -= SubmitPlayerCards;
        _machine.Dir.PlayerHandController.Lock();
    }

    // Callback Methods ===========================================================================

    private void SubmitPlayerCards()
    {
        _machine.CurrentState = _machine.ComputeState;
    }
}
