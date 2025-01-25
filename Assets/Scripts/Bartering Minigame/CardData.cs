using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New CardData", menuName = "Bartering/CardData")]
public class CardData : ScriptableObject
{
    [Tooltip("An ID used for debug purposes.")]
    public string Id;

    /// <summary>
    /// Two card data 'match' if they have the same id.
    /// </summary>
    /// <param name="other">CardData - the other card to compare to this one.</param>
    /// <returns>bool - whether or not the two match.</returns>
    public bool Matches(CardData other)
    {
        return Id == other.Id;
    }
}