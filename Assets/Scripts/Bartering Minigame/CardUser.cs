using UnityEngine;
using System.Collections.Generic;

public class CardUser : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [Header("Stats")]
    [SerializeField, Tooltip("The number of cards we draw each turn by default.\n\nDefault: 3")]
    private int BaseDrawSize = 3;
    [Header("Card Piles")]
    [SerializeField, Tooltip("A list of CardData we have in our deck. Populates our DrawPile.")]
    private List<CardData> DebugStartingDeck = new();

    // A public enum used as shorthand to identify the three places cards can be- the Draw Pile,
    // the Hand, and the Discard Pile.
    public enum CardPile { DrawPile, Hand, DiscardPile }

    // An action called when we draw cards. Takes as argument the number of cards drawn.
    // Useful for things like SFX.
    public System.Action<int> OnDraw;
    // An action called when the state of our hand changes in any way. 
    // Takes as argument a HandDelta object, containing relevant info about what changes occurred.
    // Useful for things like hand animations.
    public System.Action<HandDelta> OnHandUpdated;
    // An action called when the
    public System.Action OnShuffle;

    // Misc Internal Variables ====================================================================

    // The list representing our draw pile- where UNUSED and inaccessible cards go.
    private List<CardData> _drawPile = null;
    // The list representing our hand- where cards that can be played go.
    private List<CardData> _hand = new();
    // The list representing our discard pile- where USED and inaccessible cards go.
    private List<CardData> _discardPile = new();
    // Used as a quick converter between our CardPile enum and our actual card lists.
    private static Dictionary<CardPile, List<CardData>> _pileToList = null;

    // Initializers ===============================================================================

    private void Awake()
    {
        // Initialize _drawPile and _pileToList.
        // ================

        _drawPile = new List<CardData>();
        foreach (CardData data in DebugStartingDeck) {

            /*
                TODO: 
                If cards end up being really simple and don't need to hold instance data, DON'T
                CLONE! It's expensive and pointless if we can just add reference copies instead.
            */

            // Clone cards from our startingDeck into our drawpile.
            CardData dataInstance = Instantiate(data);
            dataInstance.name = data.name;

            _drawPile.Add(dataInstance);
        }

        _pileToList = new() {
            {CardPile.DrawPile, _drawPile},
            {CardPile.Hand, _hand},
            {CardPile.DiscardPile, _discardPile},
        };
    }

    // Public interface methods ===================================================================

    /// <summary>
    /// Plays a specified card from our hand. CURRENTLY DOES NOTHING BUT DISCARD.
    /// </summary>
    /// <param name="indexInHand">int - the index in our Hand for the played card.</param>
    public void PlayCard(int indexInHand)
    {
        // Play the card at indexInHand to the GameManager.
        // ================

        if (indexInHand < 0 || indexInHand > _hand.Count) {
            Debug.LogError($"CardUser Error. UseCard failed. indexInHand {indexInHand} was outside "
                         + $"the bounds of the list.", this);
            return;
        }

        // Make the card do something.
        /*
            ... hand[indexInHand] ...
        */

        Discard(indexInHand);
    }

    /// <summary>
    /// Shuffles the DiscardPile and then pops it to the DrawPile. This way, if we're shuffling
    /// into a non-empty DrawPile, only the new cards get shuffled, and pre-existing ones keep
    /// their existing order.
    /// </summary>
    public void ShuffleDiscardIntoDrawpile()
    {
        Shuffle(CardPile.DiscardPile);
        PopFromPushTo(CardPile.DiscardPile, CardPile.DrawPile, _discardPile.Count);

        OnShuffle?.Invoke();
    }

    /// <summary>
    /// Draws one or more cards from the DrawPile to the Hand.
    /// Pushes as many cards as possible if it can't push the specified number of cards.
    /// </summary>
    /// <param name="n">int - the number of cards to push. Default 1.</param>
    public void Draw(int n=1)
    {
        List<CardData> fromList = _pileToList[CardPile.DrawPile];
        List<CardData> toList = _pileToList[CardPile.Hand];

        if (_drawPile.Count < n) {
            ShuffleDiscardIntoDrawpile();
        }

        PopFromPushTo(CardPile.DrawPile, CardPile.Hand, Mathf.Min(n, _drawPile.Count), addToBack:true);

        OnDraw?.Invoke(n);
    }

    /// <summary>
    /// Draws cards equal to the CardUser's BaseDrawSize from the DrawPile to the Hand.
    /// </summary>
    public void DrawHand()
    {
        Draw(BaseDrawSize);
    }

    /// <summary>
    /// Discards a specified card.
    /// </summary>
    /// <param name="indexToDiscard">The index of the card to discard.</param>
    public void Discard(int indexToDiscard)
    {
        RemoveAtFromPushTo(indexToDiscard, CardPile.Hand, CardPile.DiscardPile);
    }

    /// <summary>
    /// Discards your entire hand.
    /// </summary>
    public void DiscardHand()
    {
        for (int i = _hand.Count-1; i >= 0; i--) {
            RemoveAtFromPushTo(i, CardPile.Hand, CardPile.DiscardPile);
        }
    }

    /// <summary>
    /// Shuffles-in-place the list tied to a CardPile, using the Fisher-Yates algorithm.
    /// </summary>
    /// <param name="pile">CardPile - the pile to shuffle.</param>
    public void Shuffle(CardPile pile)
    {
        List<CardData> pileList = _pileToList[pile];

        // Fisher-Yates shuffle
        for (int i = pileList.Count-1; i > 0; i--) {
            SwapItems(pileList, i, Random.Range(0,i));
        }

        // If the hand got updated, make a new HandDelta and note that cards got moved around...
        if (pileList == _pileToList[CardPile.Hand]) {
            HandDelta delta = new();
            delta.addSource = delta.removedDestination = CardPile.Hand;
            // But since nothing is added or removed, don't fill out those fields.
            OnHandUpdated?.Invoke(delta);
            // Elsewhere, if you need, identify a delta with empty Added and Removed lists as
            // representing a shuffle.
        }
    }

    /*
        TODO:
        Add a method to return readonly copies of these lists.
        ...Or make some properties.
    */

    // Pile-editing methods =======================================================================

    private void SwapItems<T>(List<T> pile, int indexA, int indexB)
    {
        // In-place swaps items in a list.
        // ================

        (pile[indexB], pile[indexA]) = (pile[indexA], pile[indexB]);
    }

    private void PopFromPushTo(CardPile fromPile, CardPile toPile, int n=1, bool addToBack=false)
    {
        // Simulates a queuelike pop-push operation on two cardlists. Removes 
        // n cards from the end of fromPile, and adds them to the start of toPile.
        // ================

        List<CardData> fromList = _pileToList[fromPile];
        List<CardData> toList = _pileToList[toPile];

        if (fromList.Count < n) {
            Debug.LogError($"CardUser Error. PopFromPushTo failed. There are fewer cards in "
                         + $"fromPile ({fromList.Count}) than the number requested ({n}).", this);
            return;
        }

        // Moving the card ================

        // Prepare to potentially change the state of the hand.
        HandDelta delta = null;

        for (int i = 0; i < n; i++) {
            CardData card = fromList[^1];
            int sourceIndex = fromList.Count-1;
            fromList.RemoveAt(sourceIndex);

            // If adding to the back, insert at the final index, otherwise use the first.
            toList.Insert(addToBack ? toList.Count : 0, card);

            if (fromPile == CardPile.Hand) {
                // Lazily initialize delta if it's null...
                delta ??= new();
                // And note that a card was removed from our hand.
                delta.removed.Add(new HandDelta.Removed(card, sourceIndex));
            }

            if (toPile == CardPile.Hand) {
                // Lazily initialize delta if it's null...
                delta ??= new();
                // And note that a card was added to our hand.
                delta.added.Add(new HandDelta.Added(card, addToBack));
            }
        }

        // Ending HandDelta stuff ================

        // If we changed our hand at all, announce it.
        if (delta != null) {
            delta.addSource = fromPile;
            delta.removedDestination = toPile;

            OnHandUpdated?.Invoke(delta);
        }
    }

    private void RemoveAtFromPushTo(int indexInFromPile, CardPile fromPile, CardPile toPile, bool addToBack=false)
    {
        // Attempts to remove the card at indexInFromPile from fromPile and
        // pushes it to the start of toPile. Raises an error if index is too
        // big or too small.
        // ================

        List<CardData> fromList = _pileToList[fromPile];
        List<CardData> toList = _pileToList[toPile];

        if (indexInFromPile < 0 || indexInFromPile > fromList.Count) {
            Debug.LogError($"CardUser Error. RemoveAtPushTo failed. indexInFromPile "
                         + $"{indexInFromPile} was outside the bounds of the list.", this);
            return;
        }

        // Moving the card ================

        CardData card = fromList[indexInFromPile];
        fromList.RemoveAt(indexInFromPile);
        // If adding to the back, insert at the final index, otherwise use the first.
        toList.Insert(addToBack ? toList.Count : 0, card);

        // HandDelta stuff ================

        HandDelta delta = null;

        if (fromPile == CardPile.Hand) {
            // Lazily initialize delta if it's null...
            delta ??= new();
            // And note that a card was removed from our hand.
            delta.removed.Add(new HandDelta.Removed(card, indexInFromPile));
        }

        if (toPile == CardPile.Hand) {
            // Lazily initialize delta if it's null...
            delta ??= new();
            // And note that a card was added to our hand.
            delta.added.Add(new HandDelta.Added(card, addToBack));
        }

        // If we changed our hand at all, announce it.
        if (delta != null) {
            delta.addSource = fromPile;
            delta.removedDestination = toPile;

            OnHandUpdated?.Invoke(delta);
        }
    }

    // Helper classes =============================================================================

    /// <summary>
    /// A helper class for tracking cards that move in and out of the visible hand.
    /// Useful for animating card movement.
    /// </summary>
    public class HandDelta
    {
        // A list of cards that were ADDED in this delta...
        public List<Added> added = new();
        // And what pile they came from.
        public CardPile addSource = CardPile.DrawPile;
        // A list of cards that were REMOVED in this delta...
        public List<Removed> removed = new();
        // And what pile they were sent to.
        public CardPile removedDestination  = CardPile.DiscardPile;



        /// <summary>
        /// Struct representing a single added card, including its CardData and whether it was
        /// added to the front or back of our hand.
        /// </summary>
        public struct Added
        {
            public CardData data;
            public bool toBack;

            public Added(CardData _data, bool _toBack=true) 
            {
                data = _data;
                toBack = _toBack;
            }

            public override readonly string ToString()
            {
                string s = toBack ? "back" : "front";
                return $"{data.name} at {s}";
            }
        }

        /// <summary>
        /// Struct representing a single removed card, including its CardData and its former
        /// position in our hand.
        /// </summary>
        public struct Removed
        {
            public CardData _data;
            public int _formerIndex;

            public Removed(CardData data, int formerIndex) 
            {
                _data = data;
                _formerIndex = formerIndex;
            }

            public override readonly string ToString()
            {
                return $"{_data.name} at {_formerIndex}";
            }
        }
    }
}

