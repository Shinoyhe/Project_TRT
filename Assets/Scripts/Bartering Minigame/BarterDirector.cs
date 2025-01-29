using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BarterDirector : MonoBehaviour
{
    // Parameters =================================================================================

    [SerializeField, Range(0, 100), Tooltip("The current willingness percentage, from 0-100.")]
    private float Willingness;

    [Header("Parameters")]
    [Tooltip("The number of tone cards each player must play.\n\nDefault: 3")]
    public int CardsToPlay = 3;
    [SerializeField, Tooltip("The percentage willingness, from 0-100, lost per second.\n\nDefault: 5")]
    private float DecayPerSecond = 5;
    [SerializeField, Tooltip("How long, in seconds, the opponent's turn lasts.\n\nDefault: 1")]
    private float OppDuration = 1;
    [SerializeField, Tooltip("How long, in seconds, the opponent's turn lasts.\n\nDefault: 1.5")]
    private float ComputeDuration = 1.5f;
    [Tooltip("The amount Willingness is changed by on a successful, matching response.\n\nDefault: 5")]
    public float WillingnessPerMatch = 5;
    [Tooltip("The amount Willingness is changed by on a unsuccessful, nonmatching response.\n\nDefault: -5")]
    public float WillingnessPerFail = -5;
    [SerializeField, Tooltip("Whether or not we should print debug messages.")]
    private bool DebugMode = false;

    [Header("Object References")]
    [SerializeField, Tooltip("The tone responses the opposing NPC prefers.")]
    public OppBarterResponses BarterResponses;
    [SerializeField, Tooltip("The card user used by the opposing NPC.")]
    private CardUser OppCardUser;
    [SerializeField, Tooltip("The card user used by the player.")]
    private CardUser PlayerCardUser;
    [Tooltip("The HandController used by the player.")]
    public HandController PlayerHandController;

    // Actions for when arrays are updated.
    public System.Action<PlayingCard[]> OnOppCardsSet;
    public System.Action<PlayingCard[]> OnPlayerCardsSet;
    public System.Action<bool[]> OnMatchArraySet;
    // Action for when the full non-null set of player cards is submitted.
    public System.Action OnPlayerAllCardsSet;

    // Misc Internal Variables ====================================================================

    private PlayingCard[] _oppCards = null;
    private PlayingCard[] _playerCards = null;
    private bool[] _matchArray = null;
    private BarterStateMachine _machine = null;

    // Initializers ===============================================================================

    private void Start()
    {
        _machine = new(this, PlayerCardUser, OppCardUser, OppDuration, ComputeDuration);
        _machine.SetDebug(DebugMode);
        _machine.StartMachine();

        _playerCards = new PlayingCard[CardsToPlay];
    }

    // Update methods =============================================================================

    private void Update()
    {
        _machine.SetDebug(DebugMode);
    
        _machine.UpdateState();
    }

    // Public accessors ===========================================================================

    public float GetWillingness() { return Willingness; }

    public PlayingCard[] GetOppCards() { return _oppCards; }

    public PlayingCard[] GetPlayerCards() { return _playerCards; }

    public bool[] GetMatchArray() { return _matchArray; }

    public string GetCurrentStateName() { return _machine.CurrentState.StateName; }

    // Willingness manipulators ===================================================================

    /// <summary>
    /// Initializes Willingness to some value between 0-100. Called at the start of the bartering
    /// minigame in the Init state, based on the 'value' of the trade.
    /// </summary>
    /// <param name="startingValue">float - the value to set Willingness to.</param>
    public void InitializeWillingness(float startingValue)
    {
        if (startingValue < 0 || startingValue > 100) {
            Debug.LogError($"BarterDirector Error: InitializeWillingness failed. "
                         + $"startingValue ({startingValue}) must be between 0-100.");
            return;
        }

        Willingness = startingValue;
    }

    /// <summary>
    /// Called once per frame. Reduces our willingness by a small amount, adjusted for deltatime.
    /// </summary>
    public void DecayWillingness()
    {
        Willingness -= DecayPerSecond * Time.deltaTime;
    }

    /// <summary>
    /// Changes Willingness by some amount. Clamps the result between 0 and 100.
    /// </summary>
    /// <param name="amount">float - the value we change Willingness by.</param>
    public void NudgeWillingness(float amount)
    {
        Willingness = Mathf.Clamp(Willingness+amount, 0, 100);
    }

    // Array manipulators =========================================================================

    public void SetOppCards(PlayingCard[] oppCards) 
    {
        // Validate the array. We accept two states:
        //  * A null array, signifying 'no cards played'.
        //  * An array of length CardsToPlay, signifying 'all cards played'.
        if (oppCards != null && oppCards.Length != CardsToPlay) {
            Debug.LogError($"BarterDirector Error: SetOppCards failed. Expected {CardsToPlay} "
                         + $"cards, got {oppCards.Length} instead.");
        }
        
        _oppCards = oppCards;
        OnOppCardsSet?.Invoke(oppCards);
    }

    public void SetPlayerCard(PlayingCard playerCard, int indexInArray)
    {
        if (_playerCards != null && (indexInArray < 0 || indexInArray >= _playerCards.Length)) {
            Debug.LogError($"BarterDirector Error: SetPlayerCard failed. indexInArray "
                         + $"({indexInArray}) was beyond the bounds of the array (length "
                         + $"{_playerCards.Length})");
        }

        // print($"_playerCards is null: ({_playerCards == null})");
        // print($"Set card at index {indexInArray} to {((playerCard != null) ? playerCard.Id : "null")}");
        _playerCards[indexInArray] = playerCard;

        // Check if all slots are non-null.
        foreach (PlayingCard card in _playerCards) {
            // If we encounter any nonnull card, we shouldn't submit.
            if (card == null) return;
        }
        // If we exit the loop, all cards were non-null!
        OnPlayerAllCardsSet?.Invoke();
    }

    public void SetPlayerCards(PlayingCard[] playerCards) 
    { 
        // Validate the array. We accept two states:
        //  * A null array, signifying 'no cards played'.
        //  * An array of length CardsToPlay, signifying 'all cards played'.
        if (playerCards != null && playerCards.Length != CardsToPlay) {
            Debug.LogError($"BarterDirector Error: SetPlayerCards failed. Expected {CardsToPlay} "
                         + $"cards, got {playerCards.Length} instead.");
        }

        _playerCards = playerCards;
        OnPlayerCardsSet?.Invoke(playerCards);
    }

    public void SetMatchArray(bool[] matchArray)
    {
        // Validate the array. We accept two states:
        //  * A null array, signifying 'no matches to show'.
        //  * An array of length CardsToPlay, signifying 'all matches to show'.
        if (matchArray != null && matchArray.Length != CardsToPlay) {
            Debug.LogError($"BarterDirector Error: SetMatchArray failed. Expected {CardsToPlay} "
                         + $"cards, got {matchArray.Length} instead.");
        }

        _matchArray = matchArray;
        OnMatchArraySet?.Invoke(matchArray);
    }
}
