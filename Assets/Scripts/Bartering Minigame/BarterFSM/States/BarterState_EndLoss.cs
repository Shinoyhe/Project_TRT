public class BarterState_EndLoss : BarterBaseState
{
    // ============================================================================================
    //
    //                   TODO: THIS CLASS HAS ONLY DEBUG FUNCTIONALITY RIGHT NOW!
    //
    // ============================================================================================

    // State Methods ==============================================================================

    /// <summary>
    /// Returns a new instance of this state.
    /// </summary>
    /// <param name="stateName">string - the internal ID of this state.</param>
    /// <param name="machine">BarterStateMachine - the FSM that holds this state.</param>
    public BarterState_EndLoss(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
        _machine.Dir.TriggerLose();

        // Triggers music manager to return to previously playing music
        MusicActionsManager.ChangeToPreviousMusicState();
        
        //_machine.Dir.StartCoroutine(DEBUG_Reload());
    }

    public override void UpdateState() {}

    public override void Exit() {}
}
