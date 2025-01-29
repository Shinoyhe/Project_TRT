using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class HandController : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The cardUser which owns the hand we're displaying.\n\nDefault: 5")]
    private CardUser cardUser;
    [SerializeField, Tooltip("The displayCard prefab, with images and text preconfigured.")]
    private GameObject displayCardPrefab;
    [SerializeField, Tooltip("The cardUser which owns the hand we're displaying.\n\nDefault: 5")]
    private BarterCardSubmissionUI SubmissionUI;


    [Header("Positioning")]
    [SerializeField, Tooltip("The leftmost anchor object.")]
    private Transform anchorLeft;
    [SerializeField, Tooltip("The rightmost anchor object.")]
    private Transform anchorRight;
    [SerializeField, Tooltip("The anchor object determining where drawn cards are spawned from.")]
    private Transform drawAnchor;
    [SerializeField, Tooltip("The anchor object determining where discarded cards animate to.")]
    private Transform discardAnchor;
    [SerializeField, Range(0,1), Tooltip("The amount we multiply (handsize - 1) by to get our usableRange\n\nDefault: 0.2.")]
    private float usableRangePerCard = 0.2f;
    [SerializeField, /*ReadOnly,*/ Range(0,1), Tooltip("The amount of our anchored arc we're allowed to place cards in.")]
    private float usableRange;


    [Header("Animation")]
    [SerializeField, Tooltip("The amount of time, in seconds, a card takes to move back into place.\n\nDefault: 0.25")]
    private float moveTime = 0.25f;
    [SerializeField, Tooltip("The amount we multiply a card's scale by on a hover.\n\nDefault: 1.1f.")]
    private float hoverScaleFactor = 1.1f;
    [SerializeField, Tooltip("The amount we multiply a card's scale by on a drag.\n\nDefault: 0.9f.")]
    private float dragScaleFactor = 0.9f;
    [SerializeField, Tooltip("The amount of time, in seconds, a card takes to animate a hover/unhover.\n\nDefault: 0.15")]
    private float hoverTime = 0.15f;
    [SerializeField, Tooltip("The alpha value of a card when it is being dragged.\n\nDefault: 0.5")]
    private float dragAlpha = 0.5f;
    [SerializeField/*, ReadOnly*/]
    private bool alreadyDragging = false;
    [SerializeField, Tooltip("The amount of time, in seconds, a card takes to snap to a submit slot.\n\nDefault: 0.15")]
    private float snapTime = 0.15f;

    [Header("Other")]
    [SerializeField, Tooltip("The amount of time, in seconds, between dragUpdate events.\n\nDefault: 0.1")]
    private float dragUpdateRate = 0.1f;


    [SerializeField/*, ReadOnly*/] // A list of all DisplayCards in our hand.
    private List<DisplayCard> hand = null;
    // A dictionary of the positions of our hand in base formation.
    private Dictionary<DisplayCard, Vector3> baseCardPositions = new();



    private void Awake()
    {
        // Awake is called before Start.
        // ================

        cardUser.OnHandUpdated += UpdateDisplayCards;
        // TODO: cardUser.OnDraw += play draw SFX;
        // TODO: cardUser.OnShuffle += play shuffle sfx;

        hand = new();
    }

    // Repainting Methods =========================================================================

    public void UpdateDisplayCards(CardUser.HandDelta handDelta)
    {
        // Deletes or adds cards in our HandController, based on an updated state
        // from our CardUser.
        // ================

        // Remove cards.
        foreach (CardUser.HandDelta.Removed removed in handDelta.removed) {
            DisplayCard display = hand[removed._formerIndex];
            hand[removed._formerIndex] = null;

            display.startHovering -= OnCardHoverStart;
            display.doneHovering -= OnCardHoverEnd;
            display.startDragging -= OnCardDragStart;
            display.dragging -= OnCardDrag;
            display.doneDragging -= OnCardDragEnd;  

            display.doneTransforming = (display) => {
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

        // Add cards.
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

            int destinationIndex = added.toBack ? hand.Count : 0;
            hand.Insert(destinationIndex, display);

            display.startHovering += OnCardHoverStart;
            display.doneHovering += OnCardHoverEnd;
            display.startDragging += OnCardDragStart;
            display.dragging += OnCardDrag;
            display.doneDragging += OnCardDragEnd;

            display.Initialize(data, destinationIndex, dragUpdateRate, dragAlpha);
            display.name = $"DisplayCard({data.name})";
        }

        // Remove all null displayCards from our list.
        hand.RemoveAll(displayCard => displayCard == null);
        // Update our usableRange.
        usableRange = Mathf.Min(1,usableRangePerCard*(hand.Count-1));
        // Update all our indices-in-hand.
        for (int i=0; i<hand.Count; i++) {
            hand[i].indexInHand = i;
        }

        ResetToViewPosition();
    }

    public void ResetToViewPosition() { ResetToViewPosition(false); }

    public void ResetToViewPosition(bool overrideAnimation=false)
    {
        baseCardPositions.Clear();

        for (int i = 0; i < hand.Count; i++) {
            DisplayCard displayCard = hand[i];

            // Calculate the position we need to put the card at!
            float lerpIndex = (hand.Count == 1) ? 0.5f : i/(float)(hand.Count-1);

            // The amount, from 0-1, we need to shift forward along our arc to have our cards
            // be centered after accounting for usableRange.
            float usableRange_arcOffset = 0.5f*(1-usableRange);
            // How far, from 0-1, this card should be placed on our arc. 
            float distanceOnArc = usableRange*lerpIndex + usableRange_arcOffset;
            // Get the position!
            Vector3 position = Vector3.Lerp(anchorLeft.localPosition, anchorRight.localPosition,
                                            distanceOnArc);

            // Log our base position in our dictionary.
            baseCardPositions[displayCard] = position;

            // If we the current card isn't submitted...
            if (displayCard.SubmitSlot == null) {
                // Figure out how long we should animate for and animate!
                float duration = overrideAnimation ? 0 : moveTime;
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
        foreach (DisplayCard card in hand) {
            card.SetInteractable(false);
        }
    }

    /// <summary>
    /// Unlock input on our display cards.
    /// </summary>
    public void Unlock()
    {
        foreach (DisplayCard card in hand) {
            card.SetInteractable(true);
        }
    }

    // DisplayCard Callbacks ======================================================================

    private void OnCardHoverStart(DisplayCard card)
    {
        // Also, moves and reorders all other cards in the sibling hierarchy.
        // Called when this card is hovered over.
        // ================

        if (alreadyDragging) return;

        card.transform.SetAsLastSibling();

        // Play hover SFX
    }

    private void OnCardHoverEnd(DisplayCard card)
    {
        if (alreadyDragging || card.SubmitSlot != null) return;

        card.LerpOnlySize(1, hoverTime);
        ResetToViewPosition();
    }

    private void OnCardDragStart(DisplayCard card)
    {
        if (!alreadyDragging) {
            card.LerpOnlySize(dragScaleFactor, hoverTime);
            alreadyDragging = true;

            // If we were in a slot, unhook ourselves from it.
            if (card.SubmitSlot != null) {
                card.SubmitSlot.SetCard(null);
                card.SetSubmitted(null);
            }
        }
    }

    private void OnCardDrag(DisplayCard card)
    {      
        // Doesn't do anything right now.
        return;
    }

    private void OnCardDragEnd(DisplayCard card)
    {
        // There is one valid scenario in which a drag ending will result in a valid play:
        // the card must be played over a CardSubmitSlot.
        // ================

        if (alreadyDragging) {
            PlayerCardSlot targetSlot = null;

            foreach (PlayerCardSlot slot in SubmissionUI.GetPlayerCardSlots()) {
                if (card.GetWorldRect().Overlaps(slot.GetWorldRect())) {
                    // Animate something on the slot? Something on the card?

                    // If the slot is already filled, kick the old card out.
                    if (slot.Card != null) {
                        slot.Card.SetSubmitted(null);
                        slot.SetCard(null);
                    }

                    targetSlot = slot;
                    break;
                }
            }

            // If we have a target/targets...
            if (targetSlot != null) {
                // TODO: Submit the card to the director in that slot!
                // TODO: Play a submit SFX
                // TODO: Note the display card as 'submitted'- this discounts it
                //       from hand rendering, until it's dragged out again

                // Find the position of the slot in OUR localspace.
                Vector3 localSlotPosition = card.transform.localPosition + 
                                            (targetSlot.transform.position-card.transform.position);
                card.TransformTo(localSlotPosition, 1, snapTime);
                card.SetSubmitted(targetSlot);
                targetSlot.SetCard(card);

                ResetToViewPosition();
            } else {
                // If we don't have targets, return to our hand.
                card.TransformTo(baseCardPositions[card], 1, moveTime);
                card.SetSubmitted(null);
                card.SubmitSlot.SetCard(null);

                ResetToViewPosition();
            }
            
            alreadyDragging = false;
        }
    }
}