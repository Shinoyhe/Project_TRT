using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New NPCData", menuName = "ScriptableObjects/NPCData")]
public class NPCData : ScriptableObject
{
    #region ======== [ CLASSES ] ========

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

    #endregion

    #region ======== [ VARIABLES ] ========

    public string Name;
    public Sprite Icon;
    [TextArea] public string Bio;

    // I'm thinking that the BarterResponseMatrix could be referenced and called from here instead. Maybe this script could combine with it?
    public BarterResponseMatrix Matrix;

    [ReadOnly] [SerializeField] private Dictionary<PlayingCard, CardPreference> journalTonePreferences;
    [ReadOnly] [SerializeField] private Dictionary<InventoryCardData, InventoryCardData> journalKnownTrades;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Changes the Player's Assumed Preferences in the journal
    /// </summary>
    /// <param name="opponentCard">The opponent card that the player is logging about.</param>
    /// <param name="playerCard">The card the player thinks is either positive or negative.</param>
    /// <param name="state">Whether the player believes the player card is POSITIVE or NEGATIVE</param>
    public void ChangeJournalTonePreference(PlayingCard opponentCard, PlayingCard playerCard, BarterResponseMatrix.State state)
    {
        journalTonePreferences.TryAdd(opponentCard, new CardPreference());
        
        switch (state)
        {
            case BarterResponseMatrix.State.POSITIVE:
                journalTonePreferences[opponentCard].Positive = playerCard;
                break;
            case BarterResponseMatrix.State.NEGATIVE:
                journalTonePreferences[opponentCard].Negative = playerCard;
                break;
        }
    }


    /// <summary>
    /// Adds known trade to the journal
    /// </summary>
    /// <param name="playerCard">Inventory Card that the player provides the NPC</param>
    /// <param name="oppCard">Inventory Card that the NPC trades for the playerCard</param>
    public void AddJournalKnownTrade(InventoryCardData playerCard, InventoryCardData oppCard)
    {
        if (journalKnownTrades == null)
        {
            journalKnownTrades = new Dictionary<InventoryCardData, InventoryCardData>();
        }

        journalKnownTrades.TryAdd(playerCard, oppCard);
    }


    /// <summary>
    /// Returns the Preference class linked to the opponent card.
    /// </summary>
    /// <param name="opponentCard">The opponent card</param>
    /// <returns>Prefence class includes the PlayingCards marked as positive and negative.</returns>
    public CardPreference GetPreference(PlayingCard opponentCard)
    {
        if (journalTonePreferences == null)
        {
            InitalizePreferences();
        }

        return journalTonePreferences[opponentCard];
    }


    /// <param name="playerCard">Card the player trades in</param>
    /// <returns>Card the NPC is know to give in exchange for playerCard</returns>
    public InventoryCardData GetKnownTrade(InventoryCardData playerCard)
    {
        if (journalKnownTrades == null)
        {
            journalKnownTrades = new Dictionary<InventoryCardData, InventoryCardData>();
        }

        if (playerCard == null || !journalKnownTrades.ContainsKey(playerCard)) 
            return null;

        return journalKnownTrades[playerCard];
    }


    /// <summary>
    /// Resets the Journal Preferences
    /// </summary>
    public void ResetJournalPreferences()
    {
        journalTonePreferences = new Dictionary<PlayingCard, CardPreference>();
    }

    /// <summary>
    /// Returns the possible Cards that the NPC may play
    /// </summary>
    public PlayingCard[] Cards => Matrix.OppCards;

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    /// <summary>
    /// Builds a default preference dictionary
    /// </summary>
    private void InitalizePreferences()
    {
        journalTonePreferences = new Dictionary<PlayingCard, CardPreference>();
        foreach (var card in Matrix.OppCards)
        {
            journalTonePreferences.Add(card, new CardPreference());
        }
    }

    #endregion
}
