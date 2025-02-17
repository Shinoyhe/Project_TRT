using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BarterState_EndWin : BarterBaseState
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
    public BarterState_EndWin(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
        Debug.Log("Player wins!");
        _machine.Dir.TriggerWin();
        //_machine.Dir.StartCoroutine(DEBUG_Reload());
    }

    public override void UpdateState() {}

    public override void Exit() {}

    // Debug Methods ==============================================================================

    private IEnumerator DEBUG_Reload()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}