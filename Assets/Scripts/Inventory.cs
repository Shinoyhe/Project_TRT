using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<CardData> startingCards;
    private List<CardData> cards;

    // Start is called before the first frame update
    void Start()
    {
        clear();
        cards.AddRange(startingCards);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Public Methods

    public void addCard(CardData card)
    {
        
        if (IDtaken(card.id))
        {
            Debug.LogError("Card ID: " + card.id + " already exists in inventory. Failed to add");
            return;
        }

        cards.Add(card);
    }

    public void removeCard(CardData card)
    {
        cards.Remove(card);
    }

    public void clear()
    {
        cards.Clear();
    }

    // Returns the card with the id, null if card cannot be found
    public CardData getCardByID(string id)
    {
        foreach (CardData card in cards)
        {
            if (card.id == id) return card;
        }
        return null;
    }

    // Returns the first card with the name, null if card cannot be found
    public CardData getCardByName(string cardName)
    {
        foreach (CardData card in cards)
        {
            if (card.name == cardName) return card;
        }
        return null;
    }

    // Returns a list of all cards with a given name
    public List<CardData> getCardsByName(string cardName)
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

    #endregion

    #region Private Methods

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