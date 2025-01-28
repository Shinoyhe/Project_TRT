using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlayerCardSlotUI : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [SerializeField, Tooltip("TEMPORARY IMPLEMENTATION. For now, acts as our card back.")]
    private Image MainImage;
    [SerializeField, Tooltip("TEMPORARY IMPLEMENTATION. For now, acts as our card label.")]
    private TMP_Text MainText; 
    [SerializeField, Tooltip("The sprite on this slot when our state is set to null.")]
    private Sprite NullSprite;

    // Initializers ===============================================================================

    private void Awake()
    {
        SetState(null);
    }

    // Public manipulators ========================================================================

    public void SetState(PlayingCard card)
    {
        if (card == null) {
            MainImage.sprite = NullSprite;
            MainImage.color = Color.white;
            MainText.text = "";
        } else {
            MainImage.sprite = null;
            MainImage.color = card.DEBUG_COLOR;
            MainText.text = card.Id;
        }
    }
}