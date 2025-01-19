using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New CardData", menuName = "Bartering/CardData")]
public class CardData : ScriptableObject
{
    [Tooltip("An ID used for debug purposes.")]
    public string Id;
}