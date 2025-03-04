using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New NPCData", menuName = "ScriptableObjects/NPCData")]
public class NPCData : ScriptableObject
{
    public class CardPreference
    {
        public PlayingCard Positive;
        public PlayingCard Negative;

        public CardPreference()
        {
            Positive = null;
            Negative = null;
        }

        public CardPreference(PlayingCard positive, PlayingCard negative)
        {
            Positive = positive;
            Negative = negative;
        }
    }

    public string Name;
    public Sprite Icon;
    [TextArea] public string Bio;

    // I'm thinking that the BarterResponseMatrix could be referenced and called from here instead. Maybe this script could combine with it?
    public BarterResponseMatrix Matrix;

    [ReadOnly] [SerializeField] private Dictionary<PlayingCard, CardPreference> journalPreferences;


    /// <summary>
    /// Changes the Player's Assumed Preferences in the journal
    /// </summary>
    /// <param name="opponentCard">The opponent card that the player is logging about.</param>
    /// <param name="playerCard">The card the player thinks is either positive or negative.</param>
    /// <param name="state">Whether the player believes the player card is POSITIVE or NEGATIVE</param>
    public void ChangeJournalPreference(PlayingCard opponentCard, PlayingCard playerCard, BarterResponseMatrix.State state)
    {
        journalPreferences.TryAdd(opponentCard, new CardPreference());
        
        switch (state)
        {
            case BarterResponseMatrix.State.POSITIVE:
                journalPreferences[opponentCard].Positive = playerCard;
                break;
            case BarterResponseMatrix.State.NEGATIVE:
                journalPreferences[opponentCard].Negative = playerCard;
                break;
        }
    }


    /// <summary>
    /// Returns the Preference class linked to the opponent card.
    /// </summary>
    /// <param name="opponentCard">The opponent card</param>
    /// <returns>Prefence class includes the PlayingCards marked as positive and negative.</returns>
    public CardPreference GetPreference(PlayingCard opponentCard)
    {
        if (journalPreferences == null)
        {
            InitalizePreferences();
        }

        return journalPreferences[opponentCard];
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>Dictionary of the PlayingCard</returns>
    public Dictionary<PlayingCard, CardPreference> GetPreferences()
    {
        if (journalPreferences == null)
        {
            InitalizePreferences();
        }

        return journalPreferences;
    }


    /// <summary>
    /// Resets the Journal Preferences
    /// </summary>
    public void ResetJournalPreferences()
    {
        journalPreferences = new Dictionary<PlayingCard, CardPreference>();
    }

    /// <summary>
    /// Returns the possible Cards that the NPC may play
    /// </summary>
    public PlayingCard[] Cards => Matrix.OppCards;

    /// <summary>
    /// Builds a default preference dictionary
    /// </summary>
    private void InitalizePreferences()
    {
        journalPreferences = new Dictionary<PlayingCard, CardPreference>();
        foreach (var card in Matrix.OppCards)
        {
            journalPreferences.Add(card, new CardPreference());
        }
    }
}
