using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class JournalPreferenceEntry : MonoBehaviour
{
    #region ======== [ VARIABLES ] ========

    [SerializeField] private Image opponentCardDisplay;
    [SerializeField] private Image negativeCardDisplay;
    [SerializeField] private Image positiveCardDisplay;
    [SerializeField] private PlayingCard nullCard;

    private PlayingCard[] _playingCards;
    private PlayingCard _oppCard;
    private int _posCardIndex;
    private int _negCardIndex;
    private NPC _npcData;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    public void CycleNegativeToneCard()
    {
        _negCardIndex = (_negCardIndex + 1) % _playingCards.Length;

        var card = _playingCards[_negCardIndex];
        negativeCardDisplay.sprite = card.MainSprite;
        _npcData.ChangeJournalTonePreference(_oppCard, card, BarterResponseMatrix.State.NEGATIVE);
    }

    public void CyclePositiveToneCard()
    {
        _posCardIndex = (_posCardIndex + 1) % _playingCards.Length;

        var card = _playingCards[_posCardIndex];
        positiveCardDisplay.sprite = card.MainSprite;
        _npcData.ChangeJournalTonePreference(_oppCard, card, BarterResponseMatrix.State.POSITIVE);
    }

    public void Load(NPC NPC, PlayingCard oppCard)
    {
        // Need to add null to possible cards
        var possibleCards = NPC.Cards.ToList();
        possibleCards.Add(nullCard);
        _playingCards = possibleCards.ToArray();

        _npcData = NPC;
        _oppCard = oppCard;
        var preferences = NPC.GetPreference(oppCard);

        preferences.Negative = preferences.Negative ?? nullCard;
        preferences.Positive = preferences.Positive ?? nullCard;

        // Set Index
        for (int i = 0; i < _playingCards.Length; i++)
        {
            var card = _playingCards[i];

            if (card == preferences.Negative)
            {
                _negCardIndex = i;
            }

            if (card == preferences.Positive)
            {
                _posCardIndex = i;
            }
        }


        // Set Sprites
        opponentCardDisplay.sprite = oppCard.MainSprite;
        negativeCardDisplay.sprite = preferences.Negative.MainSprite;
        positiveCardDisplay.sprite = preferences.Positive.MainSprite;
    }

    #endregion
}