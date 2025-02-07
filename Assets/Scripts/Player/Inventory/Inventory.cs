using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class Inventory : MonoBehaviour
{
    public List<InventoryCardData> StartingCards;
    public event Action OnInventoryUpdated;

    private List<InventoryCardData> _cards;

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

    // Start is called before the first frame update
    void Start()
    {
        _cards = new List<InventoryCardData>();

        Clear();

        foreach (InventoryCardData card in StartingCards) {
            AddCard(card);
        }
    }

    #region ---------- Public Methods ----------

    public List<InventoryCardData> Get()
    {
        if (_cards == null) {
            return new List<InventoryCardData>();
        }

        return new List<InventoryCardData>(_cards);
    }

    public void AddCard(InventoryCardData card)
    {
        if (card == null) return;


        if (IDtaken(card.ID)) {
            Debug.LogError("Card ID: " + card.ID + " already exists in inventory. Failed to add");
            return;
        }

        _cards.Add(card);
        OnInventoryUpdated?.Invoke();
    }

    public void RemoveCard(InventoryCardData card)
    {
        if (!_cards.Contains(card)) {
            Debug.LogError("Cannot remove card. Card does not exist.");
            return;
        }

        _cards.Remove(card);
        OnInventoryUpdated?.Invoke();
    }

    public void Clear()
    {
        _cards.Clear();
        OnInventoryUpdated?.Invoke();
    }

    public void ClearExceptType(CardTypes type)
    {
        foreach (InventoryCardData card in _cards) {
            if (type != card.Type) {
                RemoveCard(card);
            }
        }
    }

    public void Print()
    {
        string printString = "[\n";
        foreach (InventoryCardData card in _cards) {
            printString += $"[{card.CardName}, {card.ID}, {card.Type},\"{card.Description}\"],\n";
        }

        printString += "]";
        Debug.Log(printString);
    }

    /// <summary>
    /// Returns the card with id, null if one cannot be found
    /// </summary>
    /// <param name="id">The id to search for.</param>
    /// <returns></returns>
    public InventoryCardData GetCardByID(string id)
    {
        foreach (InventoryCardData card in _cards) {
            if (card.ID == id) return card;
        }

        return null;
    }

    /// <summary>
    /// Returns the first card with cardName, null if one cannot be found
    /// </summary>
    /// <param name="cardName">The cardName to search for.</param>
    /// <returns></returns>
    public InventoryCardData GetCardByName(string cardName)
    {
        foreach (InventoryCardData card in _cards) {
            if (card.name == cardName) return card;
        }

        return null;
    }

    /// <summary>
    /// Returns a List of all cards with cardName
    /// </summary>
    /// <param name="cardName">The cardName to search for.</param>
    /// <returns></returns>
    public List<InventoryCardData> GetCardsByName(string cardName)
    {
        List<InventoryCardData> returnList = new List<InventoryCardData>();
        foreach (InventoryCardData card in _cards) {
            if (card.name == cardName) {
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
    public List<InventoryCardData> GetCardsByType(CardTypes type)
    {
        List<InventoryCardData> returnList = new List<InventoryCardData>();
        foreach (InventoryCardData card in _cards) {
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
        Comparison<InventoryCardData> comparison = sortParameter switch {
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

        _cards.Sort(comparison);
        OnInventoryUpdated?.Invoke();
    }

    #endregion

    #region ---------- Private Methods ----------

    /// <summary>
    /// Is the cardID already in use?
    /// </summary>
    /// <returns></returns>
    private bool IDtaken(string cardID)
    {
        foreach (InventoryCardData card in _cards) {
            if (card.ID == cardID) return true;
        }
        
        return false;
    }

    #endregion
}

#region ---------- Ideas ----------

/* 
cards are scriptable objects

addCard
removeCard
clear
sort?

getByName


*/

#endregion