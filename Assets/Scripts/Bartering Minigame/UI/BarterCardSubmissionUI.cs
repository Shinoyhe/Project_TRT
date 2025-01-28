using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarterCardSubmissionUI : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [SerializeField, Tooltip("The BarterDirector in this scene.")]
    private BarterDirector Director;

    [SerializeField, Tooltip("The UI prefab consisting of a single match slot.")]
    private GameObject MatchSlotPrefab;
    [SerializeField, Tooltip("A RectTransform used as the area where we can spawn MatchSlotUI.")]
    private RectTransform MatchSlotZone;
    
    // Misc Internal Variables ====================================================================

    // AutoPlayerCardslots =======================
    

    // MatchSlots =====================
    // Array of MatchSlotUI components, one for each Opp-Player pair of cards submitted.
    private MatchSlotUI[] _matchSlots = null;
    // The horizontal range of our MatchSlotZone, used to position the MatchSlotUI objects.
    private Vector2 MatchSlotBounds;
    private float MatchSlotY;

    // Initializers ===============================================================================

    private void Awake()
    {
        // TODO: Test at different resolutions.
        MatchSlotBounds = new(MatchSlotZone.offsetMin.x, MatchSlotZone.offsetMax.x);
        MatchSlotY = MatchSlotZone.anchoredPosition.y;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Instantiate our match slot objects.
        _opp
        _matchSlots = new MatchSlotUI[Director.CardsToPlay];

        for (int i=0; i<Director.CardsToPlay; i++) {
            GameObject slotObject = Instantiate(MatchSlotPrefab, transform);

            // Set the position!

            // For position purposes, we want our i to be 'normalized'- ie, centered.
            float normalizedI = (i+1)/(float)(_matchSlots.Length+1);
            Vector2 position = new(Mathf.Lerp(MatchSlotBounds.x, MatchSlotBounds.y, normalizedI), 
                                   MatchSlotY);
            ((RectTransform)slotObject.transform).anchoredPosition = position;

            _matchSlots[i] = slotObject.GetComponent<MatchSlotUI>();
        }

        // Subscribe!
        Director.OnMatchArraySet -= UpdateMatchIcons;
        Director.OnMatchArraySet += UpdateMatchIcons;
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
