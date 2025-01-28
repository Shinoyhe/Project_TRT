using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New PlayingCard", menuName = "Bartering/PlayingCard")]
public class PlayingCard : ScriptableObject
{
    [Tooltip("An ID used for debug purposes.")]
    public string Id;

    public Color DEBUG_COLOR = Color.white;

    /// <summary>
    /// Two card data 'match' if they have the same id.
    /// </summary>
    /// <param name="other">PlayingCard - the other card to compare to this one.</param>
    /// <returns>bool - whether or not the two match.</returns>
    public bool Matches(PlayingCard other)
    {
        return Id == other.Id;
    }
}