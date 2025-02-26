using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class NPCData : ScriptableObject
{
    public class Preference
    {
        public PlayingCard Positive;
        public PlayingCard Negative;
    }

    public Sprite Icon { get; }
    public string Bio { get; }
    public BarterResponseMatrix Matrix { get; }

    [ReadOnly] [SerializeField] private Dictionary<PlayingCard, Preference> journalPreferences = new Dictionary<PlayingCard, Preference>();


    /// <summary>
    /// Changes the Player's Assumed Preferences in the journal
    /// </summary>
    /// <param name="opponentCard">The opponent card that the player is logging about.</param>
    /// <param name="playerCard">The card the player thinks is either positive or negative.</param>
    /// <param name="state">Whether the player believes the player card is POSITIVE or NEGATIVE</param>
    public void ChangeJournalPreference(PlayingCard opponentCard, PlayingCard playerCard, BarterResponseMatrix.State state)
    {
        journalPreferences.TryAdd(opponentCard, new Preference());
        
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
    public Preference GetPreference(PlayingCard opponentCard)
    {
        return journalPreferences[opponentCard];
    }


    /// <summary>
    /// Resets the Journal Preferences
    /// </summary>
    public void ResetJournalPreferences()
    {
        journalPreferences = new Dictionary<PlayingCard, Preference>();
    }
}
