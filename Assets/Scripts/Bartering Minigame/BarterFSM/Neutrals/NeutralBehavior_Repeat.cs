using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New NeutralBehavior_Repeat", menuName = "Bartering/NeutralBehavior/Repeat")]
public class NeutralBehavior_Repeat : BarterNeutralBehavior
{
    public enum RepeatTarget {
        OPPONENT,
        PLAYER
    }

    [HorizontalLine]
    [SerializeField, Tooltip("Whether we should repeat the card from the opponent's or the "
                           + "player's last hand.")]
    private RepeatTarget repeatTarget;

    /// <summary>
    /// When a Neutral match is played in a slot, search the (hand, then deck, then discard pile)
    /// for a card
    /// 
    /// If no card matching the parameter exists, play a random card from the hand.
    /// TODO: Potentially make this introduce a card if none is found?
    /// </summary>
    /// <param name="dir">BarterDirector - the BarterDirector using this NeutralBehavior.</param>
    /// <param name="cardUser">CardUser - the CardUser this behavior applies to.</param>
    /// <param name="matchIndex">int - the slot index where the neutral match occurred.</param>
    /// <returns>PlayingCard - the card to play in that slot.</returns>
    public override PlayingCard GetCard(BarterDirector dir, CardUser cardUser, int matchIndex)
    {
        // The first MatchHistory is the most recent one.
        BarterDirector.MatchHistory history = dir.MatchHistories[0];
        PlayingCard cardToPlay = repeatTarget switch {
            RepeatTarget.OPPONENT => history.OppCards[matchIndex],
            RepeatTarget.PLAYER => history.PlayerCards[matchIndex],
            _ => null
        };

        return SearchForCard(cardUser, cardToPlay);
    }
}