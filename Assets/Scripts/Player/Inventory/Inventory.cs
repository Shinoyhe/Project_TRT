using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class Inventory : MonoBehaviour
{
    [Header("Global Card Info")]
    public List<InventoryCardData> AllCardDatas;
    [SerializeField, ReadOnly] public List<InventoryCard> AllCards;

    [Space]
    [Header("Inventory")]
    public List<InventoryCardData> StartingCards;
    [SerializeField, ReadOnly] private List<InventoryCard> Cards;

    [SerializeField, ReadOnly] private HashSet<InventoryCard> KnownCards;

    [Header("Tone Cards")]
    [Tooltip("The list of tone cards that the player uses in Bartering. Because tone card "
           + "implementation is not final, neither is the implementation of this list.")]
    public List<PlayingCard> ToneCardInventory;
    
    public event Action OnInventoryUpdated;

    [HideInInspector] public float inventoryLastUpdateTime;

    // Helper Classes =============================================================================

    // Enums for Sorting
    public enum SortParameters { 
        NAME, 
        ID, 
        TYPE 
    }

    public enum SortOrder { 
        ASCENDING, 
        DESCENDING 
    }

    // Misc Internal Variables ====================================================================

    private NotificationUI _notificationUi;

    // Initializers ===============================================================================

    private void Awake()
    {
        if (AllCardDatas == null) AllCardDatas = new List<InventoryCardData>();
    }

    // Start is called before the first frame update
    void Start()
    {
        AllCards = new List<InventoryCard>();
        Cards = new List<InventoryCard>();
        KnownCards = new HashSet<InventoryCard>();
        
        // Fill the AllCards list using AllCardDatas
        foreach (InventoryCardData cardData in AllCardDatas)
        {
            InventoryCard newCard = new InventoryCard(cardData);
            AllCards.Add(newCard);
        }

        foreach (InventoryCardData card in StartingCards) {
            AddCard(card);
        }

        // Cache refs.
        if (GameManager.Instance != null && GameManager.MasterCanvas != null) {
            _notificationUi = GameManager.MasterCanvas.GetComponent<InGameUi>().Notification;
        }
        
        GameManager.FlagTracker.UpdateICFlags();
    }

    #region ---------- Public Methods ----------

    public List<InventoryCard> Get()
    {
        if (Cards == null) {
            return new List<InventoryCard>();
        }

        return new List<InventoryCard>(Cards);
    }

    public List<InventoryCardData> GetDatas()
    {
        List<InventoryCardData> returnList = new List<InventoryCardData>();
        if (Cards == null) return returnList;

        foreach (InventoryCard card in Cards) {
            returnList.Add(card.Data);
        }
        return returnList;
    }

    public List<InventoryCardData> GetKnownDatas()
    {
        List<InventoryCardData> returnList = new List<InventoryCardData>();
        if (Cards == null) return returnList;

        foreach (InventoryCard card in KnownCards) {
            returnList.Add(card.Data);
        }
        return returnList;
    }

    public void AddCard(InventoryCardData card)
    {
        if (card == null) return;


        if (IDtaken(card.ID)) {
            Debug.LogError("Card ID: " + card.ID + " already exists in inventory. Failed to add");
            return;
        }

        // Find card in AllCards and add it to the current inventory
        InventoryCard newCard = null;
        foreach (InventoryCard possibleNewCard in AllCards) {
            if (possibleNewCard.Data == card) {
                newCard = possibleNewCard;
                break;
            }
        }
        if (newCard == null) {
            Debug.LogError("Could not find, card does not exist in AllCards");
            return;
        }
        newCard.CurrentlyOwn = true;
        newCard.HaveOwned = true;

        Cards.Add(newCard);
        KnownCards.Add(newCard);
        OnInventoryUpdated?.Invoke();
        inventoryLastUpdateTime = Time.time;
        GameManager.FlagTracker.SetFlag(card, true);

        // Finally, send a ping to our notificationUI.

        if (_notificationUi) {
            _notificationUi.Notify($"Obtained {card.CardName}");
        }
    }

    public void RemoveCard(InventoryCardData card)
    {
        if (!HasCard(card)) {
            Debug.LogError("Cannot remove card. Card is not in inventory.");
            return;
        }

        InventoryCard cardToRemove = GetCardFromData(card);

        Cards.Remove(cardToRemove);
        cardToRemove.CurrentlyOwn = false;

        OnInventoryUpdated?.Invoke();
        inventoryLastUpdateTime = Time.time;
        GameManager.FlagTracker.SetFlag(card, false);

        // Finally, send a ping to our notificationUI.

        if (_notificationUi) {
            _notificationUi.Notify($"Lost {card.CardName}");
        }
    }

    /// <summary>
    /// Is the InventoryCardData in the Inventory?
    /// </summary>
    /// <returns></returns>
    public bool HasCard(InventoryCardData cardData)
    {
        if (cardData == null) {
            Debug.LogError("Inventory: HasCard: null reference exception");
            return false;
        }

        foreach (InventoryCard card in Cards)
        {
            if (card.Data.ID == cardData.ID) return true;
        }
        return false;
    }

    public void Clear()
    {
        Cards.Clear();
        OnInventoryUpdated?.Invoke();
        GameManager.FlagTracker.ResetFlags();
    }

    public void ClearExceptType(CardTypes type)
    {
        foreach (InventoryCard card in Cards) {
            if (type != card.Type) {
                RemoveCard(card.Data);
            }
        }
    }

    public void Print()
    {
        string printString = "[\n";
        foreach (InventoryCard card in Cards) {
            printString += $"[{card.CardName}, {card.ID}, {card.Type},\"{card.Description}\", {card.StartingLocation}],\n";
        }

        printString += "]";
        Debug.Log(printString);
    }

    /// <summary>
    /// Returns the card with id, null if one cannot be found
    /// </summary>
    /// <param name="id">The id to search for.</param>
    /// <returns></returns>
    public InventoryCard GetCardByID(string id)
    {
        foreach (InventoryCard card in Cards) {
            if (card.ID == id) return card;
        }

        return null;
    }

    /// <summary>
    /// Returns the first card with cardName, null if one cannot be found
    /// </summary>
    /// <param name="cardName">The cardName to search for.</param>
    /// <returns></returns>
    public InventoryCard GetCardByName(string cardName)
    {
        foreach (InventoryCard card in Cards) {
            if (card.CardName == cardName) return card;
        }

        return null;
    }

    /// <summary>
    /// Returns a List of all cards with cardName
    /// </summary>
    /// <param name="cardName">The cardName to search for.</param>
    /// <returns></returns>
    public List<InventoryCard> GetCardsByName(string cardName)
    {
        List<InventoryCard> returnList = new List<InventoryCard>();
        foreach (InventoryCard card in Cards) {
            if (card.CardName == cardName) {
                returnList.Add(card);
            }
        }

        return returnList;
    }

    /// <summary>
    /// Returns a List of all cards of a type
    /// </summary>
    /// <param name="cardName">The type to search for.</param>
    /// <returns></returns>
    public List<InventoryCard> GetCardsByType(CardTypes type)
    {
        List<InventoryCard> returnList = new List<InventoryCard>();
        foreach (InventoryCard card in Cards) {
            if (card.Type == type) {
                returnList.Add(card);
            }
        }

        return returnList;
    }

    /// <summary>
    /// Sorts the cards in the inventory by a given parameter and in a given order
    /// </summary>
    /// <returns></returns>
    public void Sort(SortParameters sortParameter, SortOrder sortOrder)
    {
        Comparison<InventoryCard> comparison = sortParameter switch {
            SortParameters.NAME => (card1, card2) => string.Compare(card1.CardName, card2.CardName, true),
            SortParameters.ID => (card1, card2) => string.Compare(card1.ID, card2.ID, true),
            SortParameters.TYPE => (card1, card2) => string.Compare(card1.Type.ToString(), card2.Type.ToString(), true),
            _ => null
        };

        // If descending order is selected, reverse the comparison
        if (sortOrder == SortOrder.DESCENDING) {
            var originalComparison = comparison;
            comparison = (card1, card2) => -originalComparison(card1, card2);
        }

        Cards.Sort(comparison);
        OnInventoryUpdated?.Invoke();
        inventoryLastUpdateTime = Time.time;
    }

    /// <summary>
    /// </summary>
    /// <returns>The InventoryCard associated with the cardData ScriptableObject in AllCards</returns>
    public InventoryCard GetCardFromData(InventoryCardData cardData)
    {
        foreach (InventoryCard card in AllCards)
        {
            if (card.Data == cardData) { return card; }
        }
        Debug.LogError("Inventory: GetCardFromData: Could not find card.");
        return null;
    }

    #endregion

    #region ---------- Private Methods ----------

    /// <summary>
    /// Is the cardID already in use?
    /// </summary>
    /// <returns></returns>
    private bool IDtaken(string cardID)
    {
        foreach (InventoryCard card in Cards) {
            if (card.ID == cardID) return true;
        }
        
        return false;
    }

    #endregion
}