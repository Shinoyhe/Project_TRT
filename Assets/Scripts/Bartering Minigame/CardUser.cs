using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class CardUser : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [Header("Stats")]
    [SerializeField, Tooltip("The number of cards we draw each turn by default.\n\nDefault: 3")]
    private int baseDrawSize = 3;
    public int BaseDrawSize => baseDrawSize;

    [Header("Card Piles")]
    [SerializeField, Tooltip("A list of PlayingCard we have in our deck. Populates our DrawPile.")]
    private PlayingCard[] startingDeck;
    // The draw pile as a read-only list.
    public ReadOnlyCollection<PlayingCard> DrawPileList { get { return _drawPile.AsReadOnly(); }}
    // The hand as a read-only list.
    public ReadOnlyCollection<PlayingCard> HandList { get { return _hand.AsReadOnly(); }}
    // The discard pile as a read-only list.
    public ReadOnlyCollection<PlayingCard> DiscardPileList { get { return _discardPile.AsReadOnly(); }}

    // Enums ================

    // A public enum used as shorthand to identify the three places cards can be- the Draw Pile,
    // the Hand, and the Discard Pile.
    public enum CardPile { 
        DrawPile, 
        Hand, 
        DiscardPile 
    }

    // Actions ================

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
    private List<PlayingCard> _drawPile = new();
    // The list representing our hand- where cards that can be played go.")]
    private List<PlayingCard> _hand = new();
    // The list representing our discard pile- where USED and inaccessible cards go.
    private List<PlayingCard> _discardPile = new();
    // Used as a quick converter between our CardPile enum and our actual card lists.
    private Dictionary<CardPile, List<PlayingCard>> _pileToList = null;

    // Initializers ===============================================================================

    /// <summary>
    /// Initializes the draw pile and the pile lookup table. MUST be called before anything else.
    /// Sets the deck to be deckSize cards big, containing only cards in matrix.OppCards.
    /// </summary>
    /// <param name="matrix">BarterResponseMatrix - the matrix from which we use OppCards.</param>
    /// <param name="deckSize">int - the size of our deck.</param>
    public void Initialize(BarterResponseMatrix matrix, int deckSize)
    {
        Initialize(matrix.OppCards.ToList(), deckSize);
    }

    /// <summary>
    /// Initializes the draw pile and the pile lookup table. MUST be called before anything else.
    /// Sets the deck to be deckSize cards big, containing only cards in source.
    /// </summary>
    /// <param name="matrix">BarterResponseMatrix - the matrix from which we use OppCards.</param>
    /// <param name="deckSize">int - the size of our deck.</param>
    public void Initialize(List<PlayingCard> source, int deckSize)
    {
        source = source.ToHashSet().ToList();
        
        int numEach = deckSize / source.Count;
        int remainder = deckSize % source.Count;

        List<PlayingCard> tempDeck = new();

        // Equally distribute the cards between different types.
        for (int i = 0; i < source.Count; i++) {
            for (int j = 0; j < numEach; j++) {
                tempDeck.Add(source[i]);
            }

            if (i < remainder) {
                tempDeck.Add(source[i]);   
            }
        }

        startingDeck = tempDeck.ToArray();

        Initialize();
    }

    /// <summary>
    /// Initializes the draw pile and the pile lookup table. MUST be called before anything else.
    /// Sets the deck to be a clone of source.
    /// </summary>
    /// <param name="source">PlayingCard list - the list of cards we're cloning.</param>
    public void Initialize(List<PlayingCard> source)
    {
        startingDeck = source.ToArray();

        Initialize();
    }

    /// <summary>
    /// Initializes the draw pile and the pile lookup table. MUST be called before anything else.
    /// </summary>
    public void Initialize()
    {
        // Initialize _drawPile and _pileToList.
        // ================

        foreach (PlayingCard data in startingDeck) {
            // Clone cards from our startingDeck into our drawpile.
            PlayingCard dataInstance = Instantiate(data);
            dataInstance.name = data.name;
            dataInstance.PlayerSubmitted = false;

            _drawPile.Add(dataInstance);
        }

        // Initialize _pileToList.
        _pileToList = new() {
            {CardPile.DrawPile, _drawPile},
            {CardPile.Hand, _hand},
            {CardPile.DiscardPile, _discardPile},
        };
    }

    // Public interface methods ===================================================================

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
        List<PlayingCard> fromList = _drawPile;
        List<PlayingCard> toList = _hand;

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
        Draw(baseDrawSize);
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
    /// Discards all cards in your hand that have been marked as PlayerSubmitted.
    /// </summary>
    public void DiscardSubmitted()
    {
        // Iterate backwards to avoid disrupting other cards.
        for (int i = _hand.Count-1; i >= 0; i--) {
            if (_hand[i].PlayerSubmitted) {
                RemoveAtFromPushTo(i, CardPile.Hand, CardPile.DiscardPile);
            }
        }
    }

    /// <summary>
    /// Shuffles-in-place the list tied to a CardPile, using the Fisher-Yates algorithm.
    /// </summary>
    /// <param name="pile">CardPile - the pile to shuffle.</param>
    public void Shuffle(CardPile pile)
    {
        List<PlayingCard> pileList = _pileToList[pile];
        int count = pileList.Count;

        // Fisher-Yates shuffle
        for (int i = count-1; i > 0; i--) {
            SwapItems(pileList, i, Random.Range(0,i+1));
        }

        // If the hand got updated, make a new HandDelta and note that cards got moved around...
        if (pileList == _hand) {
            HandDelta delta = new();
            delta.addSource = delta.removedDestination = CardPile.Hand;
            // But since nothing is added or removed, don't fill out those fields.
            OnHandUpdated?.Invoke(delta);
            // Elsewhere, if you need, identify a delta with empty Added and Removed lists as
            // representing a shuffle.
        }
    }

    /// <summary>
    /// Searches the pile for a card that matches the supplied card, and returns its index.
    /// </summary>
    /// <param name="pile">CardPile - the pile to search.</param>
    /// <param name="match">PlayingCard - the card we're comparing our cards against.</param>
    /// <returns>Returns the index the match is at in the list, if found, and -1 if not.</returns>
    public int SearchFor(CardPile pile, PlayingCard match)
    {
        var pileList = _pileToList[pile];
        for (int i = 0; i < pileList.Count; i++) {
            if (pileList[i] != null && pileList[i].Matches(match)) {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Moves a card from a pile to the hand.
    /// </summary>
    /// <param name="pile">CardPile - the pile that the card to move is in.</param>
    /// <param name="sourceIndex">int - the index in the original pile of the card to move.</param>
    /// <returns>int - the index in the hand of the moved card.</returns>
    public int MoveToHand(CardPile pile, int sourceIndex)
    {
        return RemoveAtFromPushTo(sourceIndex, pile, CardPile.Hand);
    }

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

        List<PlayingCard> fromList = _pileToList[fromPile];
        List<PlayingCard> toList = _pileToList[toPile];

        if (fromList.Count < n) {
            Debug.LogError($"CardUser Error. PopFromPushTo failed. There are fewer cards in "
                         + $"{fromPile} ({fromList.Count}) than the number requested ({n}).", this);
            return;
        }

        // Moving the card ================

        // Prepare to potentially change the state of the hand.
        HandDelta delta = null;

        for (int i = 0; i < n; i++) {
            PlayingCard card = fromList[^1];
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

    private int RemoveAtFromPushTo(int indexInFromPile, CardPile fromPile, CardPile toPile, bool addToBack=false)
    {
        // Attempts to remove the card at indexInFromPile from fromPile and
        // pushes it to the start of toPile. Raises an error if index is too
        // big or too small.
        // ================

        List<PlayingCard> fromList = _pileToList[fromPile];
        List<PlayingCard> toList = _pileToList[toPile];

        if (indexInFromPile < 0 || indexInFromPile >= fromList.Count) {
            Debug.LogError($"CardUser Error. RemoveAtPushTo failed. indexInFromPile "
                         + $"{indexInFromPile} was outside the bounds of the list.", this);
            return -1;
        }

        // Moving the card ================

        PlayingCard card = fromList[indexInFromPile];
        fromList.RemoveAt(indexInFromPile);
        // If adding to the back, insert at the final index, otherwise use the first.
        int insertIndex = addToBack ? toList.Count : 0;
        toList.Insert(insertIndex, card);

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

        // Return! ========================
        return insertIndex;
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
        /// Struct representing a single added card, including its PlayingCard and whether it was
        /// added to the front or back of our hand.
        /// </summary>
        public struct Added
        {
            public PlayingCard data;
            public bool toBack;

            public Added(PlayingCard _data, bool _toBack=true) 
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
        /// Struct representing a single removed card, including its PlayingCard and its former
        /// position in our hand.
        /// </summary>
        public struct Removed
        {
            public PlayingCard _data;
            public int _formerIndex;

            public Removed(PlayingCard data, int formerIndex) 
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

