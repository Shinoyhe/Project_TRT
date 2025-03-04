using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class JournalPreferenceEntry : MonoBehaviour
{
    [SerializeField] private Image opponentCardDisplay;
    [SerializeField] private Image negativeCardDisplay;
    [SerializeField] private Image positiveCardDisplay;


    private PlayingCard[] _playingCards;
    private PlayingCard _oppCard;
    private int _posCardIndex;
    private int _negCardIndex;
    private NPCData _npcData;

    [SerializeField] private NPCData debugNPCData;

    public void CycleNegativeToneCard()
    {
        _negCardIndex = (_negCardIndex + 1) % _playingCards.Length;

        var card = _playingCards[_negCardIndex];
        negativeCardDisplay.sprite = card.MainSprite;
        _npcData.ChangeJournalPreference(_oppCard, card, BarterResponseMatrix.State.NEGATIVE);
    }

    public void CyclePositiveToneCard()
    {
        _posCardIndex = (_posCardIndex + 1) % _playingCards.Length;

        var card = _playingCards[_posCardIndex];
        positiveCardDisplay.sprite = card.MainSprite;
        _npcData.ChangeJournalPreference(_oppCard, card, BarterResponseMatrix.State.POSITIVE);
    }

    public void Load(NPCData NPC, PlayingCard oppCard)
    {
        // Need to add null to possible cards
        var possibleCards = NPC.Matrix.OppCards.ToList<PlayingCard>();
        possibleCards.Add(null);
        _playingCards = possibleCards.ToArray();

        _npcData = NPC;
        var preferences = NPC.GetPreference(oppCard);
    }
}