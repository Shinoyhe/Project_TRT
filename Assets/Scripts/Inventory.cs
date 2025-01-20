using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class Inventory : MonoBehaviour
{
    public List<CardData> startingCards;
    private List<CardData> cards;

    // Start is called before the first frame update
    void Start()
    {
        Clear();
        cards.AddRange(startingCards);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region ---------- Public Methods ----------

    public void AddCard(CardData card)
    {
        
        if (IDtaken(card.id))
        {
            Debug.LogError("Card ID: " + card.id + " already exists in inventory. Failed to add");
            return;
        }

        cards.Add(card);
    }

    public void RemoveCard(CardData card)
    {
        cards.Remove(card);
    }

    public void Clear()
    {
        cards.Clear();
    }

    /// <summary>
    /// Returns the card with id, null if one cannot be found
    /// </summary>
    /// <param name="id">The id to search for.</param>
    /// <returns></returns>
    public CardData GetCardByID(string id)
    {
        foreach (CardData card in cards)
        {
            if (card.id == id) return card;
        }
        return null;
    }

    /// <summary>
    /// Returns the first card with cardName, null if one cannot be found
    /// </summary>
    /// <param name="cardName">The cardName to search for.</param>
    /// <returns></returns>
    public CardData GetCardByName(string cardName)
    {
        foreach (CardData card in cards)
        {
            if (card.name == cardName) return card;
        }
        return null;
    }

    /// <summary>
    /// Returns a List of all cards with cardName
    /// </summary>
    /// <param name="cardName">The cardName to search for.</param>
    /// <returns></returns>
    public List<CardData> GetCardsByName(string cardName)
    {
        List<CardData> returnList = new List<CardData>();
        foreach (CardData card in cards)
        {
            if (card.name == cardName)
            {
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
    public List<CardData> GetCardsByType(string type)
    {
        List<CardData> returnList = new List<CardData>();
        foreach (CardData card in cards)
        {
            if (card.type == type)
            {
                returnList.Add(card);
            }
        }
        return returnList;
    }

    /// <summary>
    /// Sorts the cards in the inventory.
    /// 
    /// Sort by a given parameter and in a given order
    /// ex. sort("name", "descending")
    /// </summary>
    /// <param name="sortParameter">possible sortParameters: "name", "id", "type"</param>
    /// <param name="sortOrder">possible sortOrders: "ascending", "descending"</param>
    /// <returns></returns>
    public void Sort(string sortParameter, string sortOrder)
    {
        List<string> possibleParameters = new List<string> { "name", "id" };
        List<string> possibleOrders = new List<string> { "ascending", "descending" };

        if (!possibleParameters.Contains(sortParameter))
        {
            Debug.LogError("Cannot sort by: " + sortParameter);
            return;
        }
        if (!possibleOrders.Contains(sortOrder)) {
            Debug.LogError("Cannot sort by: " + sortOrder);
            return;
        }

        Comparison<CardData> comparison = null;

        switch (sortParameter)
        {
            case "name":
                comparison = (card1, card2) => string.Compare(card1.cardName, card2.cardName, true);
                break;
            case "id":
                comparison = (card1, card2) => string.Compare(card1.id, card2.id, true);
                break;
            case "type":
                comparison = (card1, card2) => string.Compare(card1.type, card2.type, true);
                break;
        }

        // If descending order is selected, reverse the comparison
        if (sortOrder == "descending")
        {
            var originalComparison = comparison;
            comparison = (card1, card2) => -originalComparison(card1, card2);
        }

        cards.Sort(comparison);
    }

    #endregion

    #region ---------- Private Methods ----------

    /// <summary>
    /// Is the cardID already in use?
    /// </summary>
    /// <returns></returns>
    private bool IDtaken(string cardID)
    {
        foreach (CardData card in cards)
        {
            if (card.id == cardID) return true;
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