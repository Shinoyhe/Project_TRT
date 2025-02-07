using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class InventoryCard
{
    [SerializeField, ReadOnly]
    private InventoryCardData Data;

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

}
