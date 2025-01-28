using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The cardUser which owns the hand we're displaying.\n\nDefault: 5")]
    private CardUser cardUser;
    [SerializeField, Tooltip("The displayCard prefab, with images and text preconfigured.")]
    private GameObject displayCardPrefab;


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

    // ================================================================
    // Repainting methods
    // ================================================================

    public void UpdateDisplayCards(CardUser.HandDelta handDelta)
    {
        // Deletes or adds cards in our HandController, based on an updated state
        // from our CardUser.
        // ================

        // print($"Added cards: {string.Join(",", handDelta.added)}\nRemoved cards: {string.Join(",", handDelta.removed)}");

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

            PointsOnLine.GetPosition(anchorLeft.localPosition, anchorRight.localPosition, i,
                                        hand.Count, usableRange, out Vector3 arcPosition);

            float duration = overrideAnimation ? 0 : moveTime;
            displayCard.TransformTo(arcPosition, 1, duration);

            baseCardPositions[displayCard] = arcPosition;
        }
    }

    // ================================================================
    // DisplayCard callbacks
    // ================================================================

    private void OnCardHoverStart(DisplayCard card)
    {
        // Also, moves and reorders all other cards in the sibling hierarchy.
        // Called when this card is hovered over.
        // ================

        if (alreadyDragging) return;

        // Play hover SFX
    }

    private void OnCardHoverEnd(DisplayCard card)
    {
        if (!alreadyDragging) {
            card.TransformTo(transform.localPosition, 1, hoverTime);
            ResetToViewPosition();
        }
    }

    private void OnCardDragStart(DisplayCard card)
    {
        if (!alreadyDragging) {
            card.LerpOnlySize(dragScaleFactor, hoverTime);
            alreadyDragging = true;
        }
    }

    private void OnCardDrag(DisplayCard card)
    {      
        // foreach (CardSubmitSlot slot in Director.GetCardSubmitSlots()) {
        //     if (card.GetWorldRect().Overlaps(slot.GetWorldRect())) {
        //         // TODO: Animate something on the slot? Something on the card?
        //     }
        // }
    }

    private void OnCardDragEnd(DisplayCard card)
    {
        // There is one valid scenario in which a drag ending will result in a valid play:
        // the card must be played over a CardSubmitSlot.
        // ================

        if (alreadyDragging) {
            /*CardSubmitSlot*/ Transform targetSlot = null;

            // foreach (CardSubmitSlot slot in Director.GetCardSubmitSlots()) {
            //     if (card.GetWorldRect().Overlaps(slot.GetWorldRect())) {
            //         // Animate something on the slot? Something on the card?
            //         targetSlot = slot;
            //         break;
            //     }
            // }

            // If we have a target/targets...
            if (targetSlot != null) {
                // TODO: Submit the card to the director in that slot!
                // TODO: Play a submit SFX
                // TODO: Note the display card as 'submitted'- this discounts it
                //       from hand rendering, until it's dragged out again
            } else {
                // If we don't have targets, return to our hand.
                card.TransformTo(baseCardPositions[card], 1, moveTime);
                ResetToViewPosition();
            }
            
            alreadyDragging = false;
        }
    }
}