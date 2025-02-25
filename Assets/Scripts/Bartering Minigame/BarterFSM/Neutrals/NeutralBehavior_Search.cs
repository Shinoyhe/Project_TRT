using UnityEngine;

[CreateAssetMenu(fileName = "New NeutralBehavior_Search", menuName = "Bartering/NeutralBehavior/Search")]
public class NeutralBehavior_Search : BarterNeutralBehavior
{   
    [SerializeField, Tooltip("The card we want to play in the neutral slot.")]
    private PlayingCard cardToPlay;

    /// <summary>
    /// When a Neutral match is played in a slot, search the (hand, then deck, then discard pile)
    /// for a specific, constant card type to play. 
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
        return SearchForCard(cardUser, cardToPlay);
    }
}