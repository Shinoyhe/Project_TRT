using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PlayerCardSlot : MonoBehaviour 
{
    // Parameters and Publics =====================================================================

    public DisplayCard Card => _card;
    public System.Action<PlayerCardSlot, DisplayCard> OnSetCard;

    // Misc Internal Variables ====================================================================

    private RectTransform _rectTransform;
    private DisplayCard _card = null;

    // Initializers ===============================================================================

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // Public accessors ===========================================================================

    /// <summary>
    /// Returns the worldspace rect of this slot. 
    /// By default, Rects are in local space, which makes it difficult to overlap Rects on objects
    /// with different parents. Converting both to worldspace allows for direct comparisons.
    /// </summary>
    /// <returns>Rect - the worldspace rect on this slot.</returns>
    public Rect GetWorldRect() 
    {
        return RectTransformExtensions.GetWorldRect(_rectTransform);
    }

    // Public manipulators ========================================================================

    /// <summary>
    /// Store in this slot a new card (or no card).
    /// </summary>
    /// <param name="card">PlayingCard - the card data that is now held in this slot.</param>
    public void SetCard(DisplayCard card)
    {
        // NOTE: This function is called from HandController, when the DisplayCard is dragged over
        //       this slot's rect, or when a DisplayCard is dragged *off* of it.
        // NOTE: OnSetCard is subscribed to by a function in BarterCardSubmissionUI. This, in turn,
        //       submits the PlayingCard (or null) to our BarterDirector.
        // ================

        _card = card;
        OnSetCard?.Invoke(this, card);
    }
}
