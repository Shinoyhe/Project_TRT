using System.Collections;
using System.Linq;
using UnityEngine;

public class BarterState_TurnOpp : BarterBaseState
{
    // Misc Internal Variables ====================================================================

    private CardData[] playedCards = null;

    // State Methods ==============================================================================

    public BarterState_TurnOpp(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
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
        if (playedCards == null || playedCards.Length != cardsToPlay) {
            playedCards = new CardData[cardsToPlay];   
        }
        // Enemy picks cards randomly and stores them in an array!
        for (int i = 0; i < cardsToPlay; i++) {
            int handIndex = Random.Range(0, handList.Count);
            playedCards[i] = handList[handIndex];
        }

        // Send the complete array of cards to the director.
        _machine.Dir.SetOppCards(playedCards);

        Debug.Log($"Opp submitted: {string.Join(", ", playedCards.Select(x => x.Id))}");

        // _machine.CurrentState = _machine.DEBUG_TurnAutoPlayerState;
        _machine.Dir.StartCoroutine(WaitAndGo());
    }

    public override void UpdateState()
    {
        _machine.Dir.DecayWillingness();

        if (_machine.Dir.GetWillingness() <= 0) {
            _machine.Dir.StopAllCoroutines();
            _machine.CurrentState = _machine.EndLossState;
        }
    }

    public override void Exit() {}

    // Private methods ============================================================================

    private IEnumerator WaitAndGo()
    {
        yield return new WaitForSeconds(1);
        _machine.CurrentState = _machine.DEBUG_TurnAutoPlayerState;
    }
}
