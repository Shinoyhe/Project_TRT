using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BarterState_EndWin : BarterBaseState
{
    public BarterState_EndWin(string stateName, BarterStateMachine machine) : base(stateName, machine) {}

    public override void Enter(BarterBaseState previousState)
    {
        Debug.Log("Player wins!");

        _machine.Dir.StartCoroutine(DEBUG_Reload());
    }

    public override void UpdateState()
    {
        
    }

    public override void Exit() {}

    // Debug Methods ==============================================================================

    private IEnumerator DEBUG_Reload()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}