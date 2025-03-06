using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JournalNavCore : MonoBehaviour {
    // Parameters =================================================================================

    // Dialogue
    [Header("Dependencies")]
    public Canvas NPCA;
    public Canvas NPCB;
    public Canvas NPCC;
    public Canvas InfoCards;
    public Canvas Items;

    public enum UiStates {
        NPCA,
        NPCB,
        NPCC,
        InfoCards,
        Items
    }

    private UiStates _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() {
        if (NPCA == null) {
            Debug.LogError("NPC A Canvas dependency not set.");
        }

        // Swap with Accessibility Check
        MoveTo(UiStates.NPCA);
    }

    // Public Utility Methods ====================================================================

    /// <summary>
    /// Transition Start Ui to a new state.
    /// </summary>
    /// <param name="newState"> State to move to. </param>
    public void MoveTo(UiStates newState) {

        StopState(_currentCanvasState);
        StartState(newState);
    }
    public void OnEnable() {
        MoveToNPCA();
    }

    // Used for button OnClick calls as they don't let enums to be passed through :|
    public void MoveToNPCA() => MoveTo(UiStates.NPCA);
    public void MoveToNPCB() => MoveTo(UiStates.NPCB);
    public void MoveToNPCC() => MoveTo(UiStates.NPCC);
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
            case UiStates.NPCA:
                NPCA.gameObject.SetActive(false);
                break;
            case UiStates.NPCB:
                NPCB.gameObject.SetActive(false);
                break;
            case UiStates.NPCC:
                NPCC.gameObject.SetActive(false);
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
            case UiStates.NPCA:
                NPCA.gameObject.SetActive(true);
                break;
            case UiStates.NPCB:
                NPCB.gameObject.SetActive(true);
                break;
            case UiStates.NPCC:
                NPCC.gameObject.SetActive(true);
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
