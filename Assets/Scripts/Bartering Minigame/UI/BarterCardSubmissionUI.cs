using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarterCardSubmissionUI : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [SerializeField, Tooltip("The BarterDirector in this scene.")]
    private BarterDirector Director;

    [Header("Player Card Region")]
    [SerializeField, Tooltip("The UI prefab consisting of a single player card slot.")]
    private GameObject PlayerCardSlotPrefab;
    [SerializeField, Tooltip("A RectTransform used as the area where we can spawn player card slots.")]
    private RectTransform PlayerCardSlotZone;

    [Header("Opp Card Region")]
    [SerializeField, Tooltip("The UI prefab consisting of a single opp card slot.")]
    private GameObject OppCardSlotPrefab;
    [SerializeField, Tooltip("A RectTransform used as the area where we can spawn opp card slots.")]
    private RectTransform OppCardSlotZone;
    
    [Header("MatchSlots")]
    [SerializeField, Tooltip("The UI prefab consisting of a single match slot.")]
    private GameObject MatchSlotPrefab;
    [SerializeField, Tooltip("A RectTransform used as the area where we can spawn MatchSlotUI.")]
    private RectTransform MatchSlotZone;
    
    // Misc Internal Variables ====================================================================

    // Player CardSlots =======================
    // Array of AutoPlayerCardSlotUI components, one for each Player card submitted.
    private AutoPlayerCardSlotUI[] _playerCardSlots = null;
    // The horizontal range of our PlayerCardSlotZone, used to position the AutoPlayerCardSlotUI objects.
    private Vector2 _playerCardSlotBounds;
    private float _playerCardSlotY;

    // Opp CardSlots =======================
    // Array of AutoPlayerCardSlotUI components, one for each Opp card submitted.
    private AutoPlayerCardSlotUI[] _oppCardSlots = null;
    // The horizontal range of our OppCardSlotZone, used to position the AutoPlayerCardSlotUI objects.
    private Vector2 _oppCardSlotBounds;
    private float _oppCardSlotY;

    // MatchSlots =====================
    // Array of MatchSlotUI components, one for each Opp-Player pair of cards submitted.
    private MatchSlotUI[] _matchSlots = null;
    // The horizontal range of our MatchSlotZone, used to position the MatchSlotUI objects.
    private Vector2 _matchSlotBounds;
    private float _matchSlotY;

    // Initializers ===============================================================================

    private void Awake()
    {
        // TODO: Test all this at different resolutions.
        _playerCardSlotBounds = new(PlayerCardSlotZone.offsetMin.x, PlayerCardSlotZone.offsetMax.x);
        _playerCardSlotY = PlayerCardSlotZone.anchoredPosition.y;

        _oppCardSlotBounds = new(OppCardSlotZone.offsetMin.x, OppCardSlotZone.offsetMax.x);
        _oppCardSlotY = OppCardSlotZone.anchoredPosition.y;

        _matchSlotBounds = new(MatchSlotZone.offsetMin.x, MatchSlotZone.offsetMax.x);
        _matchSlotY = MatchSlotZone.anchoredPosition.y;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Array inits ================

        // Initialize our arrays!
        _playerCardSlots = new AutoPlayerCardSlotUI[Director.CardsToPlay];
        _oppCardSlots = new AutoPlayerCardSlotUI[Director.CardsToPlay];
        _matchSlots = new MatchSlotUI[Director.CardsToPlay];

        // Instantiate our objects in our arrays.
        for (int i=0; i<Director.CardsToPlay; i++)
        {
            // Initialize our Player CardSlots!
            GameObject playerSlot = PlaceSlot(i, PlayerCardSlotPrefab, _playerCardSlotBounds, 
                                              _playerCardSlotY);
            _playerCardSlots[i] = playerSlot.GetComponent<AutoPlayerCardSlotUI>();
            // Initialize our Opp CardSlots!
            GameObject oppSlot = PlaceSlot(i, OppCardSlotPrefab, _oppCardSlotBounds, 
                                           _oppCardSlotY);
            _oppCardSlots[i] = oppSlot.GetComponent<AutoPlayerCardSlotUI>();
            // Initialize our MatchSlots!
            GameObject matchSlot = PlaceSlot(i, MatchSlotPrefab, _matchSlotBounds, _matchSlotY);
            _matchSlots[i] = matchSlot.GetComponent<MatchSlotUI>();
        }

        // Action subs ================

        Director.OnPlayerCardsSet -= UpdatePlayerCards;
        Director.OnPlayerCardsSet += UpdatePlayerCards;

        Director.OnOppCardsSet -= UpdateOppCards;
        Director.OnOppCardsSet += UpdateOppCards;

        Director.OnMatchArraySet -= UpdateMatchIcons;
        Director.OnMatchArraySet += UpdateMatchIcons;

        // Locals =====================

        // Local method to simplify slot initialization.
        GameObject PlaceSlot(int i, GameObject prefab, Vector2 xBounds, float y)
        {
            // For later, center our indices in the range.
            float normalizedI = (i + 1) / (float)(Director.CardsToPlay + 1);

            GameObject slotObject = Instantiate(prefab, transform);
            float x = Mathf.Lerp(xBounds.x, xBounds.y, normalizedI);
            ((RectTransform)slotObject.transform).anchoredPosition = new(x, y);
            return slotObject;
        }
    }

    private void UpdatePlayerCards(PlayingCard[] results)
    {
        UpdateCards(results, _playerCardSlots);
    }

    private void UpdateOppCards(PlayingCard[] results)
    {
        UpdateCards(results, _oppCardSlots);
    }

    private void UpdateCards(PlayingCard[] results, AutoPlayerCardSlotUI[] slots)
    {
        // Case 1, null array- set all slots to the null sprite.
        if (results == null) {
            foreach (AutoPlayerCardSlotUI slot in slots) {
                slot.SetState(null);
            }
            return;
        }

        // Case 2, nonnull array but wrong size- raise an error.
        if (results.Length != _matchSlots.Length) {
            Debug.LogError("BarterCardSubmissionUI Error: UpdateCards failed. results array "
                        + $"length ({results.Length}) and slots array length ({slots.Length}) "
                        + $"do not match.");
            return;
        }

        // Case 3, nonnull array and right size- set sprites.
        for (int i=0; i<results.Length; i++) {
            slots[i].SetState(results[i]);
        }
    }

    private void UpdateMatchIcons(bool[] results)
    {
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
            MatchSlotUI.MatchType matchType = results[i] ? MatchSlotUI.MatchType.Right 
                                                         : MatchSlotUI.MatchType.Wrong;
            _matchSlots[i].SetState(matchType);
        }
    }
}
