using System.Collections;
using System.Linq;
using UnityEngine;

public class BarterState_Compute : BarterBaseState
{
    // State Methods ==============================================================================

    public BarterState_Compute(string stateName, BarterStateMachine machine) : base(stateName, machine) {} 

    public override void Enter(BarterBaseState previousState)
    {
        // Because of how the SetCards functions work, we know for a fact these arrays have the
        // same length... or one or both are null.
        PlayingCard[] oppCards = _machine.Dir.GetOppCards();
        PlayingCard[] playerCards = _machine.Dir.GetPlayerCards();

        if (oppCards == null) {
            Debug.LogError("BarterState_Compute Error: Enter failed. oppCards was null.");
            return;
        } else if (playerCards == null) {
            Debug.LogError("BarterState_Compute Error: Enter failed. playerCards was null.");
            return;
        }

        OppBarterResponses responses = _machine.Dir.BarterResponses;

        int numCorrect = 0;
        bool[] matchArray = new bool[oppCards.Length];

        for (int i = 0; i < oppCards.Length; i++) {
            // Match up the player cards to the NPC's preferences.
            PlayingCard desiredResponse = responses.GetResponse(oppCards[i]);

            // Store the number of correct matchups.
            if (desiredResponse.Matches(playerCards[i])) {
                numCorrect++;
                matchArray[i] = true;
            } else {
                matchArray[i] = false;
            }
        }

        // Change willingness based on the result.
        float correctAmount = numCorrect*_machine.Dir.WillingnessPerMatch;
        float incorrectAmount = (oppCards.Length-numCorrect)*_machine.Dir.WillingnessPerFail;
        _machine.Dir.NudgeWillingness(correctAmount+incorrectAmount);

        Debug.Log($"Results: {string.Join(", ", matchArray.Select(b => b ? "Match" : "No Match"))}\n"
                + $"Willingness changed by {correctAmount+incorrectAmount}!");
        _machine.Dir.SetMatchArray(matchArray);

        DoneComputing();
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
        // Clear the submitted OppCards / PlayerCards.
        // This has the side-effect of updating our UI, animating a discard.
        _machine.Dir.SetOppCards(null);
        _machine.Dir.SetPlayerCards(null);
        // Clear the submitted match array.
        // TODO: Consider moving this elsewhere, if it makes animating hard.
        _machine.Dir.SetMatchArray(null);

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
        // if (_machine.Dir.GetWillingness() >= 100) {
        //     _machine.CurrentState = _machine.EndWinState;
        // } else /*if (_machine.Director.InfoThresholds > 0) {
        //     _machine.CurrentState = _machine.CheckInfoState;
        // } else*/ {
        //     _machine.CurrentState = _machine.TurnOppState;
        // }

        if (_machine.Dir.GetWillingness() >= 100) {
            _machine.CurrentState = _machine.EndWinState;
        }

        // _machine.CurrentState = _machine.TurnOppState;
        _machine.Dir.StartCoroutine(WaitAndGo());
    }

    private IEnumerator WaitAndGo()
    {
        yield return new WaitForSeconds(2);
        _machine.CurrentState = _machine.TurnOppState;
    }
}
