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

    private Image _icon;
    private static Color _nullColor = new(1,1,1,0.5f);

    // Initializers ===============================================================================

    private void Awake()
    {
        _icon = GetComponent<Image>();
    }

    // Public manipulators ========================================================================

    /// <summary>
    /// Set the state of this Match: Right, Wrong, or Null.
    /// </summary>
    /// <param name="type">MatchType - this match's value (Right, Wrong, or Null).</param>
    public void SetState(MatchType type)
    {
        // Add fancy animations here!

        switch (type) {
            case MatchType.Right:
                _icon.sprite = rightSprite;
                break;
            case MatchType.Wrong:
                _icon.sprite = wrongSprite;
                break;
            default:
                _icon.sprite = emptySprite;
                _icon.color = _nullColor;
                break;
        };
    }
}