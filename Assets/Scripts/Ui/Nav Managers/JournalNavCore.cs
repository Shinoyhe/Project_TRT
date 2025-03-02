using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditorInternal.ReorderableList;

public class JournalNavCore : MonoBehaviour {
    // Parameters =================================================================================

    // Dialogue
    [Header("Dependencies")]
    public Canvas NPC;
    public Canvas InfoCards;
    public Canvas Items;

    public enum UiStates {
        NPC,
        InfoCards,
        Items
    }

    private UiStates _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() {
        if (NPC == null) 
        {
            Debug.LogError("NPC A Canvas dependency not set.");
        }

        // Swap with Accessibility Check
        MoveTo(UiStates.NPC);
    }

    // Public Utility Methods ====================================================================

    /// <summary>
    /// Transition Start Ui to a new state.
    /// </summary>
    /// <param name="newState"> State to move to. </param>
    public void MoveTo(UiStates newState) 
    {

        StopState(_currentCanvasState);
        StartState(newState);
    }

    public void OnEnable() 
    {
        MoveToNPC();
    }

    // Used for button OnClick calls as they don't let enums to be passed through :|
    public void MoveToNPC() => MoveTo(UiStates.NPC);
    public void MoveToInfoCards() => MoveTo(UiStates.InfoCards);
    public void MoveToItems() => MoveTo(UiStates.Items);

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Stop a currently running Ui state.
    /// </summary>
    /// <param name="stateToStop"> State that will stop. </param>
    void StopState(UiStates stateToStop) {

        // Can't stop transition states
        // (MoveToTitle)

        switch (stateToStop) {
            case UiStates.NPC:
                NPC.gameObject.SetActive(false);
                break;
            case UiStates.InfoCards:
                InfoCards.gameObject.SetActive(false);
                break;
            case UiStates.Items:
                Items.gameObject.SetActive(false);
                break;
        }

    }

    /// <summary>
    /// Start a new state.
    /// </summary>
    /// <param name="stateToStart">State that will start.</param>
    void StartState(UiStates stateToStart) {

        // Set our new state
        _currentCanvasState = stateToStart;

        switch (stateToStart) {
            case UiStates.NPC:
                NPC.gameObject.SetActive(true);
                break;
            case UiStates.InfoCards:
                InfoCards.gameObject.SetActive(true);
                break;
            case UiStates.Items:
                Items.gameObject.SetActive(true);
                break;
        }
    }


}
