using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;


[CreateAssetMenu(fileName = "InventoryCard", menuName = "ScriptableObjects/InventoryCard", order = 1)]
public class InventoryCardData : ScriptableObject
{
    public string CardName;

    public CardTypes Type;

    public string ID;
    public string Description;
    public Sprite Sprite;
    public string StartingLocation;

    public List<ContextOriginPair> contextData = new List<ContextOriginPair>();
}