using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardTypes { INFO, ITEM }


[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardData : ScriptableObject
{
    public string CardName;

    public CardTypes Type;

    public string ID;
    public string Description;
    public Sprite Sprite;
}