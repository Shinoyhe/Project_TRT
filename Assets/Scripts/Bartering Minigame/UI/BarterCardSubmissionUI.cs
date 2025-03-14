using UnityEngine;
using TMPro;

public class BarterCardSubmissionUI : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [SerializeField, Tooltip("The BarterDirector in this scene.")]
    private BarterDirector director;
    [SerializeField, Tooltip("DEBUG- DISPLAY CURRENT STATE.")]
    private TMP_Text DEBUG_StateDisplay;

    [Header("Player Card Region")]
    [SerializeField, Tooltip("The UI prefab consisting of a single player card slot.")]
    private GameObject playerCardSlotPrefab;
    [SerializeField, Tooltip("A RectTransform used as the area where we can spawn player card slots.")]
    private RectTransform playerCardSlotZone;
    public PlayerCardSlot SelectedPlayerSlot => _playerCardSlots?[_selectedSlotIndex];

    [Header("Opp Card Region")]
    [SerializeField, Tooltip("The UI prefab consisting of a single opp card slot.")]
    private GameObject oppCardSlotPrefab;
    [SerializeField, Tooltip("A RectTransform used as the area where we can spawn opp card slots.")]
    private RectTransform oppCardSlotZone;
    
    [Header("MatchSlots")]
    [SerializeField, Tooltip("The UI prefab consisting of a single match slot.")]
    private GameObject matchSlotPrefab;
    [SerializeField, Tooltip("A RectTransform used as the area where we can spawn MatchSlotUI.")]
    private RectTransform matchSlotZone;
    
    // Misc Internal Variables ====================================================================

    // Whether or not these slots can be selected.
    private bool _locked;

    // Player CardSlots =======================
    // Array of AutoPlayerCardSlotUI components, one for each Player card submitted.
    private PlayerCardSlot[] _playerCardSlots = null;
    // The horizontal range of our PlayerCardSlotZone, used to position the AutoPlayerCardSlotUI objects.
    private Vector2 _playerCardSlotBounds;
    // The slot that's currently 'selected' by the player, when not using mouse.
    private int _selectedSlotIndex = 0;

    // Opp CardSlots =======================
    // Array of AutoPlayerCardSlotUI components, one for each Opp card submitted.
    private AutoPlayerCardSlotUI[] _oppCardSlots = null;
    // The horizontal range of our OppCardSlotZone, used to position the AutoPlayerCardSlotUI objects.
    private Vector2 _oppCardSlotBounds;

    // MatchSlots =====================
    // Array of MatchSlotUI components, one for each Opp-Player pair of cards submitted.
    private MatchSlotUI[] _matchSlots = null;
    // The horizontal range of our MatchSlotZone, used to position the MatchSlotUI objects.
    private Vector2 _matchSlotBounds;

    // Initializers ===============================================================================

    private void Awake()
    {
        // TODO: Test all this at different resolutions.
        _playerCardSlotBounds = new(playerCardSlotZone.offsetMin.x, playerCardSlotZone.offsetMax.x);
        _oppCardSlotBounds = new(oppCardSlotZone.offsetMin.x, oppCardSlotZone.offsetMax.x);
        _matchSlotBounds = new(matchSlotZone.offsetMin.x, matchSlotZone.offsetMax.x);

        // Array inits ================

        // Initialize our slot arrays!
        _playerCardSlots = new PlayerCardSlot[director.CardsToPlay];
        _oppCardSlots = new AutoPlayerCardSlotUI[director.CardsToPlay];
        _matchSlots = new MatchSlotUI[director.CardsToPlay];

        // Slot inits ================

        // Instantiate our objects in our arrays.
        for (int i=0; i<director.CardsToPlay; i++)
        {
            // Initialize our Player CardSlots!
            GameObject playerSlot = PlaceSlot(i, playerCardSlotPrefab, _playerCardSlotBounds, 
                                              playerCardSlotZone);
            _playerCardSlots[i] = playerSlot.GetComponent<PlayerCardSlot>();

            // Sub to PlayerCardSlot actions
            _playerCardSlots[i].OnSetCard -= OnSlotSetCard;
            _playerCardSlots[i].OnSetCard += OnSlotSetCard;

            // Initialize our Opp CardSlots!
            GameObject oppSlot = PlaceSlot(i, oppCardSlotPrefab, _oppCardSlotBounds, 
                                           oppCardSlotZone);
            _oppCardSlots[i] = oppSlot.GetComponent<AutoPlayerCardSlotUI>();
            // Initialize our MatchSlots!
            GameObject matchSlot = PlaceSlot(i, matchSlotPrefab, _matchSlotBounds, matchSlotZone);
            _matchSlots[i] = matchSlot.GetComponent<MatchSlotUI>();
        }

        // Action subs ================

        director.OnOppCardsSet -= UpdateOppCards;
        director.OnOppCardsSet += UpdateOppCards;

        director.OnMatchArraySet -= UpdateMatchIcons;
        director.OnMatchArraySet += UpdateMatchIcons;

        // Locals =====================

        // Local method to simplify slot initialization.
        GameObject PlaceSlot(int i, GameObject prefab, Vector2 xBounds, Transform parent)
        {
            // For later, center our indices in the range.
            float normalizedI = (i + 1) / (float)(director.CardsToPlay + 1);

            GameObject slotObject = Instantiate(prefab, parent);
            float x = Mathf.Lerp(xBounds.x, xBounds.y, normalizedI);
            ((RectTransform)slotObject.transform).anchoredPosition = new(x, 0);
            return slotObject;
        }
    }

    // Update functions ===========================================================================

    private void Update()
    {
        // TODO: Replace this with an action-based implementation, that only switches the string
        // when we enter a new state.
        DEBUG_StateDisplay.text = "Current State:\n" + director.GetCurrentStateName();

        // Detect slot index inputs. ============

        if (!_locked) {
            bool dirty = false;
            int newIndex = -1;

            if (GameManager.PlayerInput.GetPrimaryTriggerDown()) {
                newIndex = (_selectedSlotIndex+1) % _playerCardSlots.Length;
                dirty = true;
            } else if (GameManager.PlayerInput.GetSecondaryTriggerDown()) {
                newIndex = _selectedSlotIndex-1;
                if (newIndex < 0) newIndex += _playerCardSlots.Length;
                dirty = true;
            }

            if (dirty) {
                _playerCardSlots[_selectedSlotIndex].SetSelected(false);
                _playerCardSlots[newIndex].SetSelected(true);

                _selectedSlotIndex = newIndex;
            }
        }
    }

    // Public accessors ===========================================================================

    public PlayerCardSlot[] GetPlayerCardSlots() { return _playerCardSlots; }

    // Public manipulators ========================================================================

    /// <summary>
    /// Sets the value of _locked. If _locked, deselects all slots, if not _locked, selects the 
    /// 0th slot.
    /// </summary>
    /// <param name="value">bool - whether or not we're locked.</param>
    public void SetLocked(bool value)
    {
        _locked = value;

        if (_locked) {
            DeselectAllPlayerSlots();
        } else {
            SelectPlayerSlot(0);
        }
    }

    /// <summary>
    /// Deselects all player slots.
    /// </summary>
    public void DeselectAllPlayerSlots()
    {
        foreach (PlayerCardSlot slot in _playerCardSlots) {
            slot.SetSelected(false);
        }
    }

    /// <summary>
    /// Sets _selectedSlotIndex to some value.
    /// </summary>
    /// <param name="index">int - the value to set _selectedSlotIndex to.</param>
    public void SelectPlayerSlot(int index)
    {
        if (index < 0 || index >= _playerCardSlots.Length) {
            Debug.Log($"BarterCardSubmissionUI Error: SelectPlayerSlot failed. index ({index}) was "
                    + $"outside the range of _playerCardSlots (length {_playerCardSlots.Length})");
        }

        _selectedSlotIndex = index;
        _playerCardSlots[_selectedSlotIndex].SetSelected(true);
    }

    // Callback functions =========================================================================

    private void OnSlotSetCard(PlayerCardSlot slot, DisplayCard card)
    {
        // Submits the PlayingCard, or null, to our director in the right slot.
        // Called via an action from PlayerCardSlot when a slot has a DisplayCard dragged over it,
        // or dragged off of it.
        //
        // NOTE: For the player, the UI is set first (via user input), and then the game state 
        //       changes to match.
        // ================

        // NOTE: Looping through the whole thing isn't a great solution, but it's simple, and 
        //       performance is negligible on the single-digit scales we're gonna be using it on.
        //       Potentially revisit this if we end up having a hand size of like, 1000, lol.
        
        for (int i=0; i<_playerCardSlots.Length; i++) {
            if (_playerCardSlots[i] == slot) {
                PlayingCard playingCard = (card != null) ? card.PlayingCard : null;
                director.SetPlayerCard(playingCard, i);
                return;
            }
        }
    }

    private void UpdateOppCards(PlayingCard[] oppCards)
    {
        // Update the displayed opponent cards. Called via an action from the BarterDirector.
        //
        // NOTE: For the opponent, the game state changes first (via the BarterDirector), and then
        //       the UI changes to match.
        // ================

        // Case 1, null array- set all slots to the null sprite.
        if (oppCards == null) {
            foreach (AutoPlayerCardSlotUI slot in _oppCardSlots) {
                slot.DisplayCard(null);
            }
            return;
        }

        // Case 2, nonnull array but wrong size- raise an error.
        if (oppCards.Length != _matchSlots.Length) {
            Debug.LogError("BarterCardSubmissionUI Error: UpdateCards failed. results array "
                        + $"length ({oppCards.Length}) and slots array length "
                        + $"({_oppCardSlots.Length}) do not match.");
            return;
        }

        // Case 3, nonnull array and right size- set sprites.
        for (int i=0; i<oppCards.Length; i++) {
            _oppCardSlots[i].DisplayCard(oppCards[i]);
        }
    }

    private void UpdateMatchIcons(BarterResponseMatrix.State[] results)
    {
        // Update the displayed opponent cards. Called via an action from the BarterDirector.
        //
        // NOTE: For the matches, the game state changes first (via the BarterDirector), and then
        //       the UI changes to match.
        // ================

        // Case 1, null array- set all slots to the null sprite.
        if (results == null) {
            foreach (MatchSlotUI slot in _matchSlots) {
                slot.SetState(MatchSlotUI.MatchType.Null);
            }
            return;
        }

        // Case 2, nonnull array but wrong size- raise an error.
        if (results.Length != _matchSlots.Length) {
            Debug.LogError("BarterCardSubmissionUI Error: UpdateMatchIcons failed. results array "
                        + $"length ({results.Length}) and _matchSlots array length "
                        + $"({_matchSlots.Length}) do not match.");
            return;
        }

        // Case 3, nonnull array and right size- set sprites.
        for (int i=0; i<results.Length; i++) {
            // Convert from state types to UI display types.
            var matchType = results[i] switch {
                BarterResponseMatrix.State.POSITIVE => MatchSlotUI.MatchType.Right,
                BarterResponseMatrix.State.NEGATIVE => MatchSlotUI.MatchType.Wrong,
                _ => MatchSlotUI.MatchType.Neutral
            };

            _matchSlots[i].SetState(matchType);
        }
    }
}