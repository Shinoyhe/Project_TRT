using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    // Parameters and Publics =====================================================================
    
    [Header("References")]
    [SerializeField, Tooltip("The cardUser which owns the hand we're displaying.\n\nDefault: 5")]
    private CardUser cardUser;
    [SerializeField, Tooltip("The displayCard prefab, with images and text preconfigured.")]
    private GameObject displayCardPrefab;
    [SerializeField, Tooltip("The cardUser which owns the hand we're displaying.\n\nDefault: 5")]
    private BarterCardSubmissionUI submissionUI;

    [Header("Positioning")]
    [SerializeField, Tooltip("The leftmost anchor object.")]
    private Transform anchorLeft;
    [SerializeField, Tooltip("The rightmost anchor object.")]
    private Transform anchorRight;
    [SerializeField, Tooltip("The anchor object determining where drawn cards are spawned from.")]
    private Transform drawAnchor;
    [SerializeField, Tooltip("The anchor object determining where discarded cards animate to.")]
    private Transform discardAnchor;
    [SerializeField, Range(0,1), Tooltip("The amount we multiply (handsize - 1) by to get our usableRange.\n\nDefault: 0.2.")]
    private float usableRangePerCard = 0.2f;

    [Header("Animation")]
    [SerializeField, Tooltip("The amount of time, in seconds, a card takes to move back into place.\n\nDefault: 0.25")]
    private float moveTime = 0.25f;
    [SerializeField, Tooltip("The amount we multiply a card's scale by on a drag.\n\nDefault: 0.9f.")]
    private float dragScaleFactor = 0.9f;
    [SerializeField, Tooltip("The amount of time, in seconds, a card takes to animate a hover/unhover.\n\nDefault: 0.15")]
    private float hoverTime = 0.15f;
    [SerializeField, Tooltip("The alpha value of a card when it is being dragged.\n\nDefault: 0.5")]
    private float dragAlpha = 0.5f;
    [SerializeField, Tooltip("The amount of time, in seconds, a card takes to snap to a submit slot.\n\nDefault: 0.15")]
    private float snapTime = 0.15f;

    [Header("SFX")]
    [SerializeField]
    private AudioEvent cardPlaceSFX;
    [SerializeField]
    private AudioEvent cardShuffleSFX;

    // Misc Internal Variables ====================================================================

    // A list of all DisplayCards in our hand.
    private List<DisplayCard> _hand = null;
    // A dictionary of the positions of our hand in base formation.
    private readonly Dictionary<DisplayCard, Vector3> _baseCardPositions = new();
    // The amount of our anchored line we're allowed to place cards in.
    private float _usableRange;
    // Whether or not we are actively dragging a displayCard.
    private bool _alreadyDragging = false;

    // Initializers ===============================================================================

    private void Awake()
    {
        // Awake is called before Start.
        // ================

        cardUser.OnHandUpdated += UpdateDisplayCards;
        // TODO: cardUser.OnDraw += play draw SFX;
        // cardUser.OnShuffle += PlayShuffleSFX;

        _hand = new();
    }

    // Repainting Methods =========================================================================

    public void PlayShuffleSFX()
    {
        cardShuffleSFX.Play(gameObject);
    }

    /// <summary>
    /// Deletes or adds DisplayCards in our HandController, based on the contents of handDelta.
    /// </summary>
    /// <param name="handDelta">CardUser.HandDelta - the change in our hand to repaint.</param>
    public void UpdateDisplayCards(CardUser.HandDelta handDelta)
    {
        // TODO: If it becomes a problem, use an object pool rather than creating and destroying 
        //       cards all the time. This implementation was not made with performance in mind. 😅
        // ================

        // Remove cards ================
        foreach (CardUser.HandDelta.Removed removed in handDelta.removed) {
            DisplayCard display = _hand[removed._formerIndex];
            _hand[removed._formerIndex] = null;

            display.StartHovering -= OnCardHoverStart;
            display.OnStartDrag -= OnCardDragStart;
            display.OnEndDrag -= OnCardDragEnd;  

            display.OnEndTransformTo = (display) => {
                Destroy(display.gameObject);
            };

            float leaveTime = moveTime;
            Vector3 pos = handDelta.removedDestination switch {
                CardUser.CardPile.DrawPile => drawAnchor.position,
                CardUser.CardPile.Hand => Vector3.Lerp(anchorRight.position, anchorLeft.position, 0.5f),
                CardUser.CardPile.DiscardPile => discardAnchor.position,
                _ => Vector3.zero
            };

            display.SetInteractable(false);

            Vector3 localAnchorPosition = pos-transform.position;
            display.TransformTo(localAnchorPosition, 0.5f, leaveTime);
        }

        // Add cards ================
        foreach (CardUser.HandDelta.Added added in handDelta.added) {
            PlayingCard data = added.data;

            GameObject displayCardObj = Instantiate(displayCardPrefab, transform);

            displayCardObj.transform.position = handDelta.addSource switch {
                CardUser.CardPile.DrawPile => drawAnchor.position,
                CardUser.CardPile.Hand => Vector3.Lerp(anchorRight.position, anchorLeft.position, 0.5f),
                CardUser.CardPile.DiscardPile => discardAnchor.position,
                _ => Vector3.zero
            };

            DisplayCard display = displayCardObj.GetComponent<DisplayCard>();

            int destinationIndex = added.toBack ? _hand.Count : 0;
            _hand.Insert(destinationIndex, display);

            display.StartHovering += OnCardHoverStart;
            display.OnStartDrag += OnCardDragStart;
            display.OnEndDrag += OnCardDragEnd;

            display.Initialize(data, destinationIndex, dragAlpha);
            display.name = $"DisplayCard({data.name})";
        }

        // Remove all null displayCards from our list.
        _hand.RemoveAll(displayCard => displayCard == null);
        // Update our usableRange.
        _usableRange = Mathf.Min(1,usableRangePerCard*(_hand.Count-1));
        // Update all our indices-in-hand.
        for (int i=0; i<_hand.Count; i++) {
            _hand[i].IndexInHand = i;
        }

        GoToViewPosition();
    }

    /// <summary>
    /// Calculate the default position for all DisplayCards, and move them there.
    /// </summary>
    /// <param name="overrideAnim">bool - whether to skip the animation. Default false.</param>
    public void GoToViewPosition(bool overrideAnim=false)
    {
        // Before setting the new positions, clear the old ones.
        _baseCardPositions.Clear();

        for (int i = 0; i < _hand.Count; i++) {
            DisplayCard displayCard = _hand[i];

            // Calculate the position we need to put the card at!
            float lerpIndex = (_hand.Count == 1) ? 0.5f : i/(float)(_hand.Count-1);

            // The amount, from 0-1, we need to shift forward along our arc to have our cards
            // be centered after accounting for usableRange.
            float usableRange_arcOffset = 0.5f*(1-_usableRange);
            // How far, from 0-1, this card should be placed on our arc. 
            float distanceOnArc = _usableRange*lerpIndex + usableRange_arcOffset;
            // Get the position!
            Vector3 position = Vector3.Lerp(anchorLeft.localPosition, anchorRight.localPosition,
                                            distanceOnArc);

            // Log our base position in our dictionary.
            _baseCardPositions[displayCard] = position;

            // If we the current card isn't submitted...
            if (displayCard.SubmitSlot == null) {
                // Figure out how long we should animate for and animate!
                float duration = overrideAnim ? 0 : moveTime;
                displayCard.TransformTo(position, 1, duration);
            }
        }
    }

    // Misc public manipulators ===================================================================

    /// <summary>
    /// Lock input on our display cards and reset to the view position.
    /// </summary>
    public void Lock()
    {
        foreach (DisplayCard card in _hand) {
            card.SetInteractable(false);
        }
    }

    /// <summary>
    /// Unlock input on our display cards.
    /// </summary>
    public void Unlock()
    {
        foreach (DisplayCard card in _hand) {
            card.SetInteractable(true);
        }

        _hand[0].Selectable.Select();
    }

    // DisplayCard Callbacks ======================================================================

    private void OnCardHoverStart(DisplayCard card)
    {
        // Don't parse hovers if we're dragging a card OR the card is submitted.
        if (_alreadyDragging || card.SubmitSlot != null) return;

        // While under a Canvas, this makes this DisplayCard render on top of its siblings.
        card.transform.SetAsLastSibling();

        // TODO: Play hover SFX
    }

    private void OnCardDragStart(DisplayCard card)
    {
        // Begin animating the card drag on the card!
        if (!_alreadyDragging) {
            card.LerpOnlySize(dragScaleFactor, hoverTime);
            _alreadyDragging = true;

            // If we were in a slot, unhook ourselves from it.
            if (card.SubmitSlot != null) {
                card.SubmitSlot.SetCard(null);
                card.SetSubmitted(null);
            }
        }
    }

    private void OnCardDragEnd(DisplayCard card)
    {
        // There is one valid scenario in which a drag ending will result in a valid play:
        // the card must be played over a PlayerCardSlot.
        // ================

        if (_alreadyDragging) {
            PlayerCardSlot targetSlot = null;

            // Check each of the slots and see if it's overlapping any of them.
            // NOTE: Looping through each slot isn't ideal, but at our scale it shouldn't matter.
            foreach (PlayerCardSlot slot in submissionUI.GetPlayerCardSlots()) {
                if (card.GetWorldRect().Overlaps(slot.GetWorldRect())) {
                    // TODO: Animate something on the slot? 
                    // TODO: Animate something on the card?

                    // If the slot is already filled, kick the old card out.
                    if (slot.Card != null) {
                        slot.Card.SetSubmitted(null);
                        slot.SetCard(null);
                    }

                    targetSlot = slot;
                    break;
                }
            }

            // After we've checked everything, if we have a target slot...
            if (targetSlot != null) {
                // Play all submitting card SFX
                
                // GARRETT: Commented following line out because it was causing errors
                // cardPlaceSFX.Play(this.gameObject);

                // Find the position of the slot in OUR localspace.
                Vector3 localSlotPosition = card.transform.parent.InverseTransformPoint(targetSlot.transform.position);
                // Snap the display card to the slot's position.
                card.TransformTo(localSlotPosition, 1, snapTime);
                // Note that the display card is submitted (don't animate it back into the hand).
                card.SetSubmitted(targetSlot);
                // Tell the slot that it's holding a new PlayingCard.
                targetSlot.SetCard(card);
            }

            // Move cards back into position.
            GoToViewPosition();
            
            _alreadyDragging = false;
        }
    }
}