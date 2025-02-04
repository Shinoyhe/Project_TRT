using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// TODO: Comment this mess

public class DisplayCard : MonoBehaviour
{
    [Header("Data and References")]
    [Tooltip("The cardData we read from.")]
    public PlayingCard CardData = null;
    [SerializeField, Tooltip("TEMPORARY IMPLEMENTATION. For now, acts as our card back.")]
    private Image MainImage;
    [SerializeField, Tooltip("TEMPORARY IMPLEMENTATION. For now, acts as our card label.")]
    private TMP_Text MainText;



    public System.Action<DisplayCard> startHovering;
    public System.Action<DisplayCard> doneHovering;
    bool isDragging = false;
    public bool _interactable = true;
    PlayerCardSlot _submitSlot = null;
    public PlayerCardSlot SubmitSlot => _submitSlot;
    float dragElapsed = 0, dragUpdateRate = 0;
    float dragAlpha = 1;
    public System.Action<DisplayCard> startDragging;
    public System.Action<DisplayCard> dragging;
    public System.Action<DisplayCard> doneDragging;
    public System.Action<DisplayCard> doneTransforming;
    private RectTransform rectTransform;
    private Bounds worldBounds;
    private CanvasGroup canvasGroup;
    private Vector3 baseLocalScale;
    private Coroutine transformRoutine = null;
    private Coroutine lerpOnlySizeRoutine = null;

    [HideInInspector] public int indexInHand;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        baseLocalScale = transform.localScale;

        // Used for collision checks.
        worldBounds = new();
        
        Repaint();
    }

    private void Update()
    {
        if (isDragging) {
            dragElapsed += Time.deltaTime;
        }
    }

    public void Initialize(PlayingCard _cardData, int _indexInHand, float _dragUpdateRate, float _dragAlpha)
    {
        CardData = _cardData;
        indexInHand = _indexInHand;
        dragUpdateRate = _dragUpdateRate;
        dragAlpha = _dragAlpha;
        Repaint();
    }

    public void Repaint()
    {
        if (CardData) {
            MainImage.sprite = null;
            MainImage.color = CardData.DEBUG_COLOR;

            MainText.text = CardData.Id;
        } else {
            MainImage.sprite = null;
            MainImage.color = Color.white;

            MainText.text = "";
        }
    }

    public Rect GetWorldRect()
    {
        return RectTransformExtensions.GetWorldRect(rectTransform);
    }

    // TransformTo ====================

    public void TransformTo(Vector3 localDestination, float targetScale, float duration)
    {
        if (transformRoutine != null) StopCoroutine(transformRoutine);
        transformRoutine = StartCoroutine(TransformToRoutine(localDestination, targetScale, duration));
    }

    private IEnumerator TransformToRoutine(Vector3 localDestination, float endScaleFactor, float duration)
    {
        float elapsed = 0;
        Vector3 startPos = transform.localPosition;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = baseLocalScale * endScaleFactor;

        while (elapsed < duration) {
            transform.localPosition = Vector3.Lerp(startPos, localDestination, elapsed/duration);
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsed/duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = localDestination;
        transform.localScale = endScale;

        doneTransforming?.Invoke(this);
        transformRoutine = null;
    }

    // LerpOnlySize ===================

    public void LerpOnlySize(float endScaleFactor, float duration)
    {
        if (lerpOnlySizeRoutine != null) StopCoroutine(lerpOnlySizeRoutine);
        lerpOnlySizeRoutine = StartCoroutine(LerpOnlySizeRoutine(endScaleFactor, duration));
    }

    public IEnumerator LerpOnlySizeRoutine(float endScaleFactor, float duration)
    {
        float elapsed = 0;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = baseLocalScale * endScaleFactor;
        while (elapsed < duration) {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsed/duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
    }

    // Misc public accessors ======================================================================

    public void SetInteractable(bool value)
    {
        _interactable = value;

        if (!_interactable || isDragging) {
            if (SubmitSlot == null) {
                canvasGroup.alpha = dragAlpha;
                return;
            }
        }

        canvasGroup.alpha = 1;
    }

    public void SetSubmitted(PlayerCardSlot slot)
    {
        _submitSlot = slot;
    }

    // ================================
    // Event callbacks
    // ================================

    // Hover ==========================

    public void OnHoverStart(BaseEventData data)
    {
        // ================

        if (!_interactable) return; 

        PointerEventData pointerData = data as PointerEventData;
        // If the mouse is not moving, we can't start a new hover.
        // if (pointerData.delta != Vector2.zero) {
            startHovering?.Invoke(this);
        // }
    }

    public void OnHoverEnd(BaseEventData data)
    {
        // ================

        if (!_interactable) return; 

        PointerEventData pointerData = data as PointerEventData;
        // If the mouse is not moving, we can't end an existing hover.
        // if (pointerData.delta != Vector2.zero) {
            doneHovering?.Invoke(this);
        // }
    }

    // Drag ===========================

    public void OnDragStart(BaseEventData data)
    {   
        // ================

        if (!_interactable) return; 

        // TODO: Make it so that when the card is first picked up,
        // it lerps to the center of the cursor, instead of snapping there immediately.

        isDragging = true;
        canvasGroup.alpha = dragAlpha;
        startDragging?.Invoke(this);
    }

    public void OnDrag(BaseEventData data)
    {   
        // Moves the card that we're dragging to our pointer.
        // ================

        if (!_interactable) return; 

        PointerEventData pointerData = data as PointerEventData;

        // Move our card (global) position to our pointer position.
        transform.position = pointerData.position;
        // Set layer
        transform.SetAsLastSibling();

        // We don't need to update this every single frame.
        if (dragElapsed >= dragUpdateRate) {
            // Update our bounds.
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 min = Camera.main.ScreenToWorldPoint(corners[0]);
            Vector3 max = Camera.main.ScreenToWorldPoint(corners[2]);
            min.z = max.z = 0;
            //               (center      , size   )
            worldBounds = new((min+max)/2f, max-min);

            dragging?.Invoke(this);
            dragElapsed = 0;
        }
    }

    public void OnDragEnd(BaseEventData data)
    {      
        // ================

        isDragging = false;
        canvasGroup.alpha = 1;
        doneDragging?.Invoke(this);
    }
}
