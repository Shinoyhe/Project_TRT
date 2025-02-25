using UnityEngine;

[CreateAssetMenu(fileName = "New NeutralBehavior_RandomHand", menuName = "Bartering/NeutralBehavior/RandomHand")]
public class NeutralBehavior_RandomHand : BarterNeutralBehavior
{
    /// <summary>
    /// When a Neutral match is played in a slot, play a random card from the hand to that slot.
    /// </summary>
    /// <param name="dir">BarterDirector - the BarterDirector using this NeutralBehavior.</param>
    /// <param name="cardUser">CardUser - the CardUser this behavior applies to.</param>
    /// <param name="matchIndex">int - the slot index where the neutral match occurred.</param>
    /// <returns>PlayingCard - the card to play in that slot.</returns>
    public override PlayingCard GetCard(BarterDirector dir, CardUser cardUser, int matchIndex)
    {
        // Searching for a null card automatically plays a random card.
        return SearchForCard(cardUser, null);
    }
}