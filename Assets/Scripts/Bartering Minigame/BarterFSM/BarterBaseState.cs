/// <summary>
/// Base class that BarterStateMachine states inherit from. 
/// Includes methods for Enter, Update, and Exit.
/// </summary>
public abstract class BarterBaseState
{
    // Used for debug/monitoring purposes.
    public string StateName = "";
    // Used to compare to states in the machine on entry into a new state...
    // ...or to reach out into the game scene via references on the machine.
    protected BarterStateMachine _machine = null;

    /// <summary>
    /// Returns a new instance of this state.
    /// </summary>
    /// <param name="stateName">string - the internal ID of this state.</param>
    /// <param name="machine">BarterStateMachine - the FSM that holds this state.</param>
    public BarterBaseState(string stateName, BarterStateMachine machine)
    {
        StateName = stateName;
        _machine = machine;
    }

    /// <summary>
    /// Called by the state machine when we move from another state to this state.
    /// </summary>
    /// <param name="previousState">BarterBaseState - the state preceding this one or null.</param>
    public abstract void Enter(BarterBaseState previousState);

    /// <summary>
    /// Called by the state machine once per frame.
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Called by the state machine when we move from this state to another state.
    /// </summary>
    public abstract void Exit();
}
