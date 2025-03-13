using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PlayerCardSlot : MonoBehaviour 
{
    // Parameters and Publics =====================================================================

    [SerializeField, Tooltip("The color this is when not selected.\n\nDefault: FFFFFF-80")]
    private Color baseColor = new(1,1,1,0.5f);
    [SerializeField, Tooltip("The color this is when selected.\n\nDefault: 00AFFF-FF")]
    private Color selectedColor = new(0/256f, 175/256f, 255/256f, 1);
    public DisplayCard Card => _card;
    public System.Action<PlayerCardSlot, DisplayCard> OnSetCard;

    // Misc Internal Variables ====================================================================

    private RectTransform _rectTransform;
    private Image _image;
    private DisplayCard _card = null;
    private bool _selected = false;

    // Initializers ===============================================================================

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    // Update methods =============================================================================

    private void Update()
    {
        // We use update to set our color to selected or not.
        // ================
        
        // TODO: Make this visualization nicer and not placeholder.

        if (_image != null) {
            // Only show the selection visuals if we're not actively using the mouse.
            if (_selected && !GameManager.PlayerInput.MouseLastUsed) {
                _image.color = selectedColor;
            } else {
                _image.color = baseColor;
            }
        }
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

    /// <summary>
    /// Whether or not this card slot is 'selected' by the Gamepad/Keyboard inputs.
    /// </summary>
    /// <param name="selected"></param>
    public void SetSelected(bool selected)
    {
        _selected = selected;
    }
}
