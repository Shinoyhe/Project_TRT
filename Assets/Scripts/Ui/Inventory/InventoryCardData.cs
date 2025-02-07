using System;
using System.Collections.Generic;
using UnityEngine;

public enum CardTypes { 
    INFO, 
    ITEM 
}

public enum Merchants
{
    MAILBOT,
    SHADY,
    PRIESTESS
}

[CreateAssetMenu(fileName = "InventoryCard", menuName = "ScriptableObjects/InventoryCard", order = 1)]
public class InventoryCardData : ScriptableObject
{
    public readonly string CardName;

    public CardTypes Type;

    public string ID;
    public string Description;
    public Sprite Sprite;
    public string StartingLocation;

    [SerializeField]
    public Dictionary<string, Array> Contexts = new Dictionary<string, Array>() 
    {
        
    };
}

[Serializable]
public class KeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}