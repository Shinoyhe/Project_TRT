using System.Collections;
using System.Linq;
using UnityEngine;

public class BarterState_TurnOpp : BarterBaseState
{
    // Misc Internal Variables ====================================================================

    // The cards that the opponent will play.
    private PlayingCard[] _playedCards = null;
    // The amount of time, in seconds, spent in this state so far.
    private float _elapsed = 0;
    // The amount of time, in seconds, we will spend in this state.
    private readonly float _duration = 1;

    // State Methods ==============================================================================

    /// <summary>
    /// Returns a new instance of this state.
    /// </summary>
    /// <param name="stateName">string - the internal ID of this state.</param>
    /// <param name="machine">BarterStateMachine - the FSM that holds this state.</param>
    /// <param name="duration">float - how long this state lasts before we exit it.</param>
    public BarterState_TurnOpp(string stateName, BarterStateMachine machine, float duration) 
                        : base(stateName, machine) 
    {
        _duration = duration;
    }

    public override void Enter(BarterBaseState previousState)
    {
        // Clear the submitted match array- important if we're entering from the Compute state.
        if (previousState == _machine.ComputeState) {
            _machine.Dir.SetMatchArray(null);
            _machine.Dir.PlayerHandController.Lock();
        }

        // Also, initialize our timer!
        _elapsed = 0;

        // Cache references.
        int cardsToPlay = _machine.Dir.CardsToPlay;
        var handList = _machine.OppCardUser.HandList;
        
        // The opponent must have enough cards in its hand to play. If we don't, exit.
        if (cardsToPlay > handList.Count ) {
            Debug.LogError("BarterState_TurnOpp Error: Enter failed. The BarterDirector wants the "
                        + $"opponent CardUser to play more cards ({cardsToPlay}) than it has in "
                        + $"its hand ({handList.Count})");
            return;
        }

        // Lazily init our card array... or reinit, if cardsToPlay changed.
        if (_playedCards == null || _playedCards.Length != cardsToPlay) {
            _playedCards = new PlayingCard[cardsToPlay];   
        }
        // Enemy picks cards randomly and stores them in an array!
        for (int i = 0; i < cardsToPlay; i++) {
            int handIndex = Random.Range(0, handList.Count);
            _playedCards[i] = handList[handIndex];
        }

        // Send the complete array of cards to the director.
        _machine.Dir.SetOppCards(_playedCards);
    }

    public override void UpdateState()
    {
        // Evaluate our state timer.
        _elapsed += Time.deltaTime;
        if (_elapsed >= _duration) { 
            _machine.CurrentState = _machine.TurnPlayerState;
            return;
        }

        // Decay willingness over time and lose if it hits zero.

        _machine.Dir.DecayWillingness();

        if (_machine.Dir.GetWillingness() <= 0) {
            _machine.Dir.StopAllCoroutines();
            _machine.CurrentState = _machine.EndLossState;
        }
    }

    public override void Exit() {}

    // Misc methods ============================================================================

    private IEnumerator WaitAndGo()
    {
        yield return new WaitForSeconds(_duration);
    }
}
