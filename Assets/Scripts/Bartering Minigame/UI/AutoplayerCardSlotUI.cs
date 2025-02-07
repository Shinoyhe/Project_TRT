using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlayerCardSlotUI : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [SerializeField, Tooltip("TEMPORARY IMPLEMENTATION. For now, acts as our card back.")]
    private Image mainImage;
    [SerializeField, Tooltip("TEMPORARY IMPLEMENTATION. For now, acts as our card label.")]
    private TMP_Text mainText; 
    [SerializeField, Tooltip("The sprite on this slot when our state is set to null.")]
    private Sprite nullSprite;

    // Initializers ===============================================================================

    private void Awake()
    {
        DisplayCard(null);
    }

    // Public manipulators ========================================================================

    /// <summary>
    /// Display in this slot a new card (or no card).
    /// </summary>
    /// <param name="card">PlayingCard - the card data we are to display in this slot.</param>
    public void DisplayCard(PlayingCard card)
    {
        if (card == null) {
            mainImage.sprite = nullSprite;
            mainImage.color = Color.white;
            mainText.text = "";
        } else {
            mainImage.sprite = null;
            mainImage.color = card.DEBUG_COLOR;
            mainText.text = card.Id;
        }
    }
}