using UnityEngine;
using UnityEngine.UI;

public class MatchSlotUI : MonoBehaviour
{
    // Parameters and Publics =====================================================================

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

    private void Start()
    {
        icon = GetComponent<Image>();
    }

    // Public manipulators ========================================================================

    public void SetState(MatchType type)
    {
        icon.sprite = type switch {
            MatchType.Right => rightSprite,
            MatchType.Wrong => wrongSprite,
            _ => emptySprite
        };
    }
}