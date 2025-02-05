using UnityEngine;
using UnityEngine.UI;

public class MatchSlotUI : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    // Lightweight enum defining multiple states that a match can be.
    public enum MatchType { Null, Right, Wrong }
    [SerializeField, Tooltip("The sprite we show when the match array is null.")]
    private Sprite emptySprite;
    [SerializeField, Tooltip("The sprite we show on a match.")]
    private Sprite rightSprite;
    [SerializeField, Tooltip("The sprite we show on a nonmatch.")]
    private Sprite wrongSprite;

    // Misc Internal Variables ====================================================================

    private Image icon;

    // Initializers ===============================================================================

    private void Awake()
    {
        icon = GetComponent<Image>();
    }

    // Public manipulators ========================================================================

    /// <summary>
    /// Set the state of this Match: Right, Wrong, or Null.
    /// </summary>
    /// <param name="type">MatchType - this match's value (Right, Wrong, or Null).</param>
    public void SetState(MatchType type)
    {
        // Add fancy animations here!

        icon.sprite = type switch {
            MatchType.Right => rightSprite,
            MatchType.Wrong => wrongSprite,
            _ => emptySprite
        };
    }
}