using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayCard : MonoBehaviour
{
    [Tooltip("The PlayingCard data we read from.")]
    public PlayingCard PlayingCard = null;
    [SerializeField, Tooltip("TEMPORARY IMPLEMENTATION. For now, acts as our card back.")]
    private Image mainImage;
    [SerializeField, Tooltip("TEMPORARY IMPLEMENTATION. For now, acts as our card label.")]
    private TMP_Text mainText;

    // The PlayerCardSlot, if any, that this DisplayCard is held by.
    public PlayerCardSlot SubmitSlot => _submitSlot;
    [HideInInspector] // The index of this DisplayCard in the HandController.
    // Alternatively, the index of this card's PlayingCard data in the CardUser hand.
    public int IndexInHand;

    // Actions that are called when this DisplayCard is interacted with in a certain way.
    public System.Action<DisplayCard> StartHovering;
    public System.Action<DisplayCard> DoneHovering;
    public System.Action<DisplayCard> OnStartDrag;
    public System.Action<DisplayCard> OnEndDrag;
    public System.Action<DisplayCard> OnEndTransformTo;

    // Misc Internal Variables ====================================================================

    // Objects ================
    // The RectTransform on this DisplayCard.
    private RectTransform _rectTransform;
    // The CanvasGroup on this DisplayCard object.
    private CanvasGroup _canvasGroup;
    // The PlayerCardSlot, or none, that holds this DisplayCard.
    PlayerCardSlot _submitSlot = null;

    // Parameters ================    
    private float _dragAlpha = 1;
    private Vector3 _baseLocalScale;

    // State ================
    // Whether or not the card can be interacted with right now.
    private bool _interactable = true;
    // Whether or not we are actively dragging this card.
    private bool _isDragging = false;
    // The Coroutine currently holding our TransformTo routine.
    private Coroutine _transformToRoutine = null;
    // The Coroutine currently holding our LerpOnlySize routine.
    private Coroutine _lerpOnlySizeRoutine = null;

    // Initializers ===============================================================================

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _baseLocalScale = transform.localScale;
        
        RepaintCard();
    }

    /// <summary>
    /// (Re)initializes and repaints our card, given a PlayingCard and some other parameters.
    /// </summary>
    /// <param name="cardData">PlayingCard - the card data that we're reading from.</param>
    /// <param name="indexInHand">int - where this card is in the HandController.</param>
    /// <param name="dragAlpha">float - the alpha of the DisplayCard when being dragged.</param>
    public void Initialize(PlayingCard cardData, int indexInHand, float dragAlpha)
    {
        PlayingCard = cardData;
        IndexInHand = indexInHand;
        _dragAlpha = dragAlpha;
        RepaintCard();
    }

    private void RepaintCard()
    {
        // Read from the PlayingCard field and populate the display objects.

        if (PlayingCard) {
            mainImage.sprite = null;
            mainImage.color = PlayingCard.DEBUG_COLOR;

            mainText.text = PlayingCard.Id;
        } else {
            mainImage.sprite = null;
            mainImage.color = Color.white;

            mainText.text = "";
        }
    }

    // Public transform methods ===================================================================

    /// <summary>
    /// Transforms this DisplayCard from one point and size to another point and size.
    /// </summary>
    /// <param name="localDestination">Vector3 - where to transform to, in this card's LOCAL space.</param>
    /// <param name="endScaleFactor">float - the final scale of the card, as a fraction of its default scale.</param>
    /// <param name="duration">float - how long to animate for.</param>
    public void TransformTo(Vector3 localDestination, float endScaleFactor, float duration)
    {
        // Acts as a wrapper function for TransformToRoutine.

        if (_transformToRoutine != null) StopCoroutine(_transformToRoutine);
        _transformToRoutine = StartCoroutine(TransformToRoutine(localDestination, endScaleFactor, duration));
    }

    private IEnumerator TransformToRoutine(Vector3 localDestination, float endScaleFactor, float duration)
    {
        float elapsed = 0;
        Vector3 startPos = transform.localPosition;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = _baseLocalScale * endScaleFactor;

        while (elapsed < duration) {
            transform.localPosition = Vector3.Lerp(startPos, localDestination, elapsed/duration);
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsed/duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set at the end to smooth over floating point errors.
        transform.localPosition = localDestination;
        transform.localScale = endScale;

        OnEndTransformTo?.Invoke(this);
        _transformToRoutine = null;
    }

    /// <summary>
    /// Transforms this DisplayCard from one size to another size.
    /// </summary>
    /// <param name="endScaleFactor">float - the final scale of the card, as a fraction of its default scale.</param>
    /// <param name="duration">float - how long to animate for.</param>
    public void LerpOnlySize(float endScaleFactor, float duration)
    {
        // Acts as a wrapper function for LerpOnlySizeRoutine.

        if (_lerpOnlySizeRoutine != null) StopCoroutine(_lerpOnlySizeRoutine);
        _lerpOnlySizeRoutine = StartCoroutine(LerpOnlySizeRoutine(endScaleFactor, duration));
    }

    private IEnumerator LerpOnlySizeRoutine(float endScaleFactor, float duration)
    {
        float elapsed = 0;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = _baseLocalScale * endScaleFactor;
        while (elapsed < duration) {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsed/duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set at the end to smooth over floating point errors.
        transform.localScale = endScale;
    }

    // Misc public manipulators and accessors =====================================================

    /// <summary>
    /// Returns the worldspace rect of this DisplayCard. 
    /// By default, Rects are in local space, which makes it difficult to overlap Rects on objects
    /// with different parents. Converting both to worldspace allows for direct comparisons.
    /// </summary>
    /// <returns>Rect - the worldspace rect on this slot.</returns>
    public Rect GetWorldRect()
    {
        return RectTransformExtensions.GetWorldRect(_rectTransform);
    }

    /// <summary>
    /// Set whether this DisplayCard accepts pointer interactions.
    /// </summary>
    /// <param name="value">bool - whether this should accept pointer interactions</param>
    public void SetInteractable(bool value)
    {
        _interactable = value;

        if (!_interactable || _isDragging) {
            if (SubmitSlot == null) {
                _canvasGroup.alpha = _dragAlpha;
                return;
            }
        }

        _canvasGroup.alpha = 1;
    }

    /// <summary>
    /// Sets the slot, or none, that this DisplayCard is held in.
    /// </summary>
    /// <param name="slot">PlayerCardSlot - the slot, or none, to be held in.</param>
    public void SetSubmitted(PlayerCardSlot slot)
    {
        _submitSlot = slot;
    }

    // Callback methods ===========================================================================

    // ============================================================================================
    // NOTE: All of these are callbacks that are called from an EventTrigger component on the
    //       GameObject that has the DisplayCard on it.
    // ============================================================================================
    
    public void OnHoverStart()
    {
        if (!_interactable) return; 

        StartHovering?.Invoke(this);
    }

    public void OnHoverEnd()
    {
        if (!_interactable) return; 
        
        DoneHovering?.Invoke(this);
    }

    public void OnDragStart()
    {   
        if (!_interactable) return; 

        // TODO: Make it so that when the card is first picked up,
        // it lerps to the center of the cursor, instead of snapping there immediately.

        _isDragging = true;
        _canvasGroup.alpha = _dragAlpha;
        OnStartDrag?.Invoke(this);
    }

    public void OnDrag(BaseEventData data)
    {   
        if (!_interactable) return; 

        PointerEventData pointerData = data as PointerEventData;

        // Move our card (global) position to our pointer position.
        transform.position = pointerData.position;
        // Set layer to front!
        transform.SetAsLastSibling();
    }

    public void OnDragEnd()
    {      
        _isDragging = false;
        _canvasGroup.alpha = 1;
        OnEndDrag?.Invoke(this);
    }
}
