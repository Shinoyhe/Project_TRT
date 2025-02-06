using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardTypes { INFO, ITEM }


[CreateAssetMenu(fileName = "InventoryCard", menuName = "ScriptableObjects/InventoryCard", order = 1)]
public class InventoryCard : ScriptableObject
{
    public string CardName;

    public CardTypes Type;

    public string ID;
    public string Description;
    public Sprite Sprite;
}