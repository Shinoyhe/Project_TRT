using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

[Serializable]
public class NPC
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

    [SerializeField, ReadOnly]
    public NPCData Data;

    [ReadOnly][SerializeField] public Dictionary<PlayingCard, CardPreference> journalTonePreferences;
    [ReadOnly][SerializeField] public Dictionary<InventoryCardData, InventoryCardData> journalKnownTrades;

    #endregion

    #region ======== [ NPCData Accessors ] ========

    public string Name
    {
        get
        {
            if (Data == null) { Debug.LogError("NPC has not been set"); throw new System.Exception("Accessing NPC Name that has not been set"); }
            return Data.Name;
        }
    }

    public Sprite Icon
    {
        get
        {
            if (Data == null) { Debug.LogError("NPC has not been set"); throw new System.Exception("Accessing NPC Icon that has not been set"); }
            return Data.Icon;
        }
    }

    public string Bio
    {
        get
        {
            if (Data == null) { Debug.LogError("NPC has not been set"); throw new System.Exception("Accessing NPC Bio that has not been set"); }
            return Data.Bio;
        }
    }

    public BarterResponseMatrix Matrix
    {
        get
        {
            if (Data == null) { Debug.LogError("NPC has not been set"); throw new System.Exception("Accessing NPC Matrix that has not been set"); }
            return Data.Matrix;
        }
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    public NPC(NPCData data)
    {
        Data = data;

        journalTonePreferences = new Dictionary<PlayingCard, CardPreference>();
        journalKnownTrades = new Dictionary<InventoryCardData, InventoryCardData>();
    }

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

    public void LoadTonePrefs(Dictionary<PlayingCard, CardPreference> newPrefs)
    {
        journalTonePreferences = newPrefs;
    }

    public void LoadKnownTrades(Dictionary<InventoryCardData, InventoryCardData> newTrades)
    {
        journalKnownTrades = newTrades;
    }

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
