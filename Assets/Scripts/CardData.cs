using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardData : ScriptableObject
{
    public string cardName;
    public string id;
    public string description;
    public Sprite sprite;
}
