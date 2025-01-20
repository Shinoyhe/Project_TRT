using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardData : ScriptableObject
{
    public string cardName;

    [Tooltip("Valid Types: info, item")]
    public string type;

    public string id;
    public string description;
    public Sprite sprite;
}
