using UnityEngine;

[CreateAssetMenu(fileName = "New PlayingCard", menuName = "Bartering/PlayingCard")]
public class PlayingCard : ScriptableObject
{
    // Parameters and Publics =====================================================================

    [Tooltip("An ID used for debug purposes.")]
    public string Id;
    [Tooltip("The sprite applied to a DisplayCard with this PlayingCard as data.")]
    public Sprite MainSprite;

    // Public methods =============================================================================

    /// <summary>
    /// Two PlayingCards 'match' if they have the same id.
    /// Use this over the equality operator "==".
    /// </summary>
    /// <param name="other">PlayingCard - the other card to compare to this one.</param>
    /// <returns>bool - whether or not the two match.</returns>
    public bool Matches(PlayingCard other)
    {
        return Id == other.Id;
    }

    /// <summary>
    /// Override for the default ToString function.
    /// </summary>
    /// <returns>string - the ID of this PlayingCard.</returns>
    public override string ToString()
    {
        return Id;
    }
}