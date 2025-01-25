using UnityEngine;

public class BarterStateMachine
{
    // Parameters and Publics =====================================================================

    // The turn director holding us.
    public BarterDirector Dir;
    // The card user used by the player.
    public CardUser PlayerCardUser;
    // The card user used by the opposing NPC.
    public CardUser OppCardUser;
    // An action called when the machine's state changes.
    public System.Action<BarterBaseState> OnStateChanged;

    // The current state of the state machine.
    private BarterBaseState _currentState = null;
    public BarterBaseState CurrentState {
        get {
            return _currentState; 
        }
        set {
            // TODO: Don't let us exit terminal states.
            /*
                if (state is terminal) {
                    return;
                }
            */

            // Exit the old state if it exists.
            _currentState?.Exit();
            
            // Cache the old state.
            BarterBaseState lastState = _currentState;
            _currentState = value;

            // Debug announce that we are changing state.
            if (_debugMode) {
                AnnounceState(_currentState, lastState);
            }

            // Actually change state.
            _currentState.Enter(lastState);
            OnStateChanged?.Invoke(_currentState);
        }
    }

    // Define our states.
    public BarterState_Init InitState;
    public BarterState_TurnOpp TurnOppState;
    // public BarterState_TurnPlayer TurnPlayerState;
    public BarterState_TurnAutoPlayer DEBUG_TurnAutoPlayerState;
    public BarterState_Compute ComputeState;
    // public BarterState_CheckInfo CheckInfoState;
    public BarterState_EndWin EndWinState;
    public BarterState_EndLoss EndLossState;
    

    // Misc Internal Variables ====================================================================

    private bool _debugMode = false;

    // Constructors and Initializers ==============================================================

    public BarterStateMachine(BarterDirector dir, CardUser playerCardUser, CardUser oppCardUser)
    {
        // Treat this as an "Awake" method. 
        // Finish any initialization elsewhere before calling Start().
        // ================

        Dir = dir;
        PlayerCardUser = playerCardUser;
        OppCardUser = oppCardUser;

        // Create our states.

        InitState = new("Init", this);
        TurnOppState = new("Opponent Turn", this);
        DEBUG_TurnAutoPlayerState = new("Auto Player Turn", this);
        ComputeState = new("Compute", this);
        // CheckInfoState = new("Check Info", this);
        EndWinState = new("Win", this);
        EndLossState = new("Loss", this);
    }

    /// <summary>
    /// Initialize the CurrentState and start the state machine.
    /// MUST be called from some controller script, *after* all other initialization.
    /// </summary>
    public void StartMachine()
    {
        CurrentState = InitState;
    }

    // Update Methods =============================================================================

    /// <summary>
    /// Update the current state.
    /// </summary>
    public void UpdateState()
    {
        CurrentState.UpdateState();
    }

    // Debug Methods ==============================================================================

    /// <summary>
    /// Enables/disables debug mode, which, when enabled, prints when we change state.
    /// </summary>
    /// <param name="value">bool - if debug mode should be enabled.</param>
    public void SetDebug(bool value)
    {
        _debugMode = value;
    }

    /// <summary>
    /// Wrapper for a debug message, called when we transition state.
    /// </summary>
    /// <param name="newState">BarterBaseState - the state we have just entered.</param>
    /// <param name="previousState">BarterBaseState - the state we have just left.</param>
    public void AnnounceState(BarterBaseState newState, BarterBaseState previousState)
    {
        string newName = (newState == null) ? "nothing" : $"{newState.StateName}";
        string previousName = (previousState == null) ? "nothing." : $"{previousState.StateName}";

        string message = $"BarterStateMachine Message: Entered {newName} from {previousName}";
        
        Debug.Log(message, Dir);
    }
}
