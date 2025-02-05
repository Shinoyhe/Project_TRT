using System.Collections;
using System.Linq;
using UnityEngine;

public class BarterState_Compute : BarterBaseState
{
    // Misc Internal Variables ====================================================================

    // Whether or not, after our compute, we are waiting to transition to the opponent's turn.
    private bool _awaitingOppTurn = false;
    // The amount of time, in seconds, spent in this state so far.
    private float _elapsed = 0;
    // The amount of time, in seconds, we will spend in this state.
    private readonly float _duration = 1.5f;

    // State Methods ==============================================================================

    /// <summary>
    /// Returns a new instance of this state.
    /// </summary>
    /// <param name="stateName">string - the internal ID of this state.</param>
    /// <param name="machine">BarterStateMachine - the FSM that holds this state.</param>
    public BarterState_Compute(string stateName, BarterStateMachine machine, float duration) 
                        : base(stateName, machine) 
    {
        _duration = duration;
    }

    public override void Enter(BarterBaseState previousState)
    {
        // Initialize our timer!
        _elapsed = 0;

        // Because of how the SetCards functions work, we know for a fact these arrays have the
        // same length... or one or both are null.
        PlayingCard[] oppCards = _machine.Dir.GetOppCards();
        PlayingCard[] playerCards = _machine.Dir.GetPlayerCards();

        if (oppCards == null) {
            Debug.LogError("BarterState_Compute Error: Enter failed. oppCards was null.");
            return;
        }

        // Calculate the number of correct matches.
        int numCorrect = 0;
        bool[] matchArray = new bool[oppCards.Length];
        
        if (playerCards != null) {
            OppBarterResponses responses = _machine.Dir.BarterResponses;        

            for (int i = 0; i < oppCards.Length; i++) {
                // Match up the player cards to the NPC's preferences.
                PlayingCard desiredResponse = responses.GetResponse(oppCards[i]);

                // Store the correct matchups in the match array.
                if (desiredResponse.Matches(playerCards[i])) {
                    numCorrect++;
                    matchArray[i] = true;
                } else {
                    matchArray[i] = false;
                }
            }
        } else {
            for (int i = 0; i < oppCards.Length; i++) {
                matchArray[i] = false;
            }
        }

        // Set the match array before nudging willingness.
        _machine.Dir.SetMatchArray(matchArray);

        // Change willingness based on the result.
        float correctAmount = numCorrect*_machine.Dir.WillingnessPerMatch;
        float incorrectAmount = (oppCards.Length-numCorrect)*_machine.Dir.WillingnessPerFail;

        _machine.Dir.NudgeWillingness(correctAmount+incorrectAmount);

        DoneComputing();
    }

    public override void UpdateState()
    {
        // Evaluate our state timer.
        _elapsed += Time.deltaTime;
        if (_elapsed >= _duration) {
            // If we're waiting for the opponent's turn and we've hit our duration, go to the opp's turn.
            if (_awaitingOppTurn) {
                _machine.CurrentState = _machine.TurnOppState;
            }
            return;
        }
        
        // Decay willingness over time and lose if it hits zero.

        _machine.Dir.DecayWillingness();

        if (_machine.Dir.GetWillingness() <= 0) {
            _machine.Dir.StopAllCoroutines();
            _machine.CurrentState = _machine.EndLossState;
        }
    }

    public override void Exit() 
    {
        // Clear the submitted OppCards / PlayerCards.
        // This has the side-effect of updating our UI, animating a discard.
        _machine.Dir.SetOppCards(null);
        _machine.Dir.ClearPlayerCards();

        // Re-initialize both CardUsers before the next opp's turn!
        _machine.PlayerCardUser.DiscardHand();
        _machine.OppCardUser.DiscardHand();
        _machine.PlayerCardUser.ShuffleDiscardIntoDrawpile();
        _machine.OppCardUser.ShuffleDiscardIntoDrawpile();
        _machine.PlayerCardUser.DrawHand();
        _machine.OppCardUser.DrawHand();
    }

    // Private methods ============================================================================

    private void DoneComputing()
    {
        // Once we're done computing, prepare to take us out of the state.
        if (_machine.Dir.GetWillingness() >= 100) {
            // If we have reached 100 Willingness we win!
            _machine.CurrentState = _machine.EndWinState;
        } else {
            // Otherwise, we have not yet won and must transition to the opp's turn.
            _awaitingOppTurn = true;
        }
    }
}
