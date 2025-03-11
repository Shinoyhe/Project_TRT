using UnityEngine;
using UnityEngine.UI;

// ================================================================================================
// This is a fully functional class up until the point where we start playing more than 3 cards. It
// will not display more than 3, and will bug out if displaying less than 3.
//
// This is far from clean or healthy implementation, but facing the deadline it's most important 
// that this gets done in a functional form.
// ================================================================================================

public class NaiveBarterHistoryUI : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [Header("Main")]
    [SerializeField, Tooltip("The BarterDirector we monitor.")]
    private BarterDirector director;
    [SerializeField, Tooltip("The index in the BarterDirector's history that we display.")]
    private int indexToMonitor;
    [SerializeField, Tooltip("A helper class containing all of our image refs.")]
    private MatchTriplet matchTriplet;

    [Header("Sprite References")]
    [SerializeField, Tooltip("The sprite we display in a match slot for a POSITIVE match.")]
    private Sprite posMatchSprite;
    [SerializeField, Tooltip("The sprite we display in a match slot for a NEUTRAL match.")]
    private Sprite neuMatchSprite;
    [SerializeField, Tooltip("The sprite we display in a match slot for a NEGATIVE match.")]
    private Sprite negMatchSprite;
    [SerializeField, Tooltip("The sprite we display whenever a card or match is null.")]
    private Sprite nullSprite;

    // Helper classes =====================================================================

    [System.Serializable]    
    public class MatchTriplet
    {
        public Image[] OppCards = new Image[3];
        public Image[] PlayerCards = new Image[3];
        public Image[] Match = new Image[3];
    }

    // Misc Internal Variables ====================================================================

    private BarterDirector.MatchHistory lastHistory = null;

    // Methods ====================================================================================

    private void Update()
    {
        // If the history hasn't changed, return.
        if (director.MatchHistories.Count <= indexToMonitor) return;
        // If the history has changed, change our display.
        BarterDirector.MatchHistory history = director.MatchHistories[indexToMonitor];
        if (lastHistory != history) {
            for (int i = 0; i < 3; i++) {
                matchTriplet.OppCards[i].sprite = history.OppCards[i].MainSprite;
                matchTriplet.PlayerCards[i].sprite = history.PlayerCards[i].MainSprite;
                matchTriplet.Match[i].sprite = GetMatchSprite(history.Matches[i]);

                if (matchTriplet.Match[i].sprite != null) {
                    matchTriplet.Match[i].color = Color.black;
                } else {
                    matchTriplet.Match[i].color = Color.clear;
                }
            }

            lastHistory = history;
        }
    }

    private Sprite GetMatchSprite(BarterResponseMatrix.State match)
    {
        return match switch {
            BarterResponseMatrix.State.POSITIVE => posMatchSprite,
            BarterResponseMatrix.State.NEUTRAL => neuMatchSprite,
            BarterResponseMatrix.State.NEGATIVE => negMatchSprite,
            _ => null
        };
    }
}