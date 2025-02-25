using UnityEngine;

public abstract class BarterNeutralBehavior : ScriptableObject
{
    public abstract PlayingCard GetCard(BarterDirector dir, CardUser cardUser, int matchIndex);

    /// <summary>
    /// Search the (hand, then deck, then discard pile) of the cardUser for a card to play.
    /// 
    /// If no card matching the parameter exists, play a random card from the hand.
    /// TODO: Potentially make this introduce a card if none is found?
    /// </summary>
    /// <param name="cardUser"></param>
    /// <param name="cardToPlay"></param>
    /// <returns></returns>
    protected PlayingCard SearchForCard(CardUser cardUser, PlayingCard cardToPlay)
    {
        if (cardUser == null) {
            Debug.LogError("BarterNeutralBehavior Error: SearchForCard failed. cardUser must not "
                         + "be null.");
            return null;
        }

        if (cardToPlay != null) {
            int i = cardUser.SearchFor(CardUser.CardPile.Hand, cardToPlay);
            if (i != -1) {
                return cardUser.HandList[i];
            }

            // If not found, check the drawPile.
            i = cardUser.SearchFor(CardUser.CardPile.DrawPile, cardToPlay);
            if (i != -1) {
                int newI = cardUser.MoveToHand(CardUser.CardPile.DrawPile, i);
                return cardUser.HandList[newI];
            }

            // If not found, check the discardPile.
            i = cardUser.SearchFor(CardUser.CardPile.DiscardPile, cardToPlay);
            if (i != -1) {
                int newI = cardUser.MoveToHand(CardUser.CardPile.DiscardPile, i);
                return cardUser.HandList[newI];
            }
        }

        // If not found, get owned, I guess. Pick a random card from the hand to submit.
        int handIndex = Random.Range(0, cardUser.HandList.Count);
        return cardUser.HandList[handIndex];
    }
}