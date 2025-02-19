using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

[Serializable]
public class InventoryCard
{
    [SerializeField, ReadOnly]
    public InventoryCardData Data;

    [ReadOnly] public List<ContextOrigins> ContextsLearned;
    [ReadOnly] public bool HaveOwned = false;
    [ReadOnly] public bool CurrentlyOwn = false;

    #region ---------- InventoryCardData Accessors ----------

    public string CardName
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard CardName that has not been set"); }
            return Data.CardName;
        }
    }

    public CardTypes Type
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard Type that has not been set"); }
            return Data.Type;
        }
    }

    public string ID
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard ID that has not been set"); }
            return Data.ID;
        }
    }

    public string Description
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard Description that has not been set"); }
            return Data.Description;
        }
    }

    public Sprite Sprite
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard Sprite that has not been set"); }
            return Data.Sprite;
        }
    }

    public string StartingLocation
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard StartingLocation that has not been set"); }
            return Data.StartingLocation;
        }
    }

    public List<ContextOriginPair> ContextData
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard ContextData that has not been set"); }
            return Data.ContextData;
        }
    }

    #endregion


    public InventoryCard()
    {
        ContextsLearned = new List<ContextOrigins>();
    }

    public InventoryCard(InventoryCardData data)
    {
        Data = data;
        ContextsLearned = new List<ContextOrigins>();
        if (!hasValidContextSetup())
        {
            Debug.LogError("Invalid Contexts, duplicate origins");
        }
    }

    /// <summary>
    /// Learns a new piece of context, if it is the last relevant context, it will learn all contexts
    /// </summary>
    /// <returns></returns>
    public void LearnContext(ContextOrigins origin)
    {
        if (ContextsLearned.Contains(origin)) {
            Debug.LogError($"Context Already Learned: {origin}");
            return;
        }
        ContextsLearned.Add(origin);
    }

    /// <summary>
    /// Checks the ContextData and returns if it is valid. Looks for duplicate ContextOrigins and 
    /// </summary>
    /// <returns></returns>
    private bool hasValidContextSetup()
    {
        // Items should not have any context
        if (Type == CardTypes.ITEM) { return true; }

        // Checks for duplicate origins
        List<ContextOrigins> originsUsed = new List<ContextOrigins>();
        foreach (ContextOriginPair originPair in ContextData) { 
            if (originsUsed.Contains(originPair.origin)) { return false; }
            originsUsed.Add(originPair.origin);
        }

        return true;
    }

    /// <summary>
    /// </summary>
    /// <returns>Whether or not the player has learned a given context</returns>
    public bool KnowsContext(ContextOrigins origin)
    {
        return false;
    }

}
