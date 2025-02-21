using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUi : MonoBehaviour
{
    // Parameters =================================================================================

    // Dialogue
    [Header("Dependencies")]
    public Canvas Default;
    public Canvas Pause;
    public Canvas Options;
    public Canvas Controls;
    public Canvas RebindControls;
    public Canvas Journal;
    public Canvas Inventory;
    public Canvas Bartering;

    public enum UiStates {
        Default,
        Pause,
        Options, 
        MoveToTitle,
        Controls,
        RebindControls,
        Journal, 
        Inventory,
        Bartering 
    }

    private UiStates _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() {
        if (Default == null) {
            Debug.LogError("Default Canvas dependency not set.");
        }

        // Swap with Accessibility Check
        MoveTo(UiStates.Default);
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

    // Used for button OnClick calls as they don't let enums to be passed through :|
    public void MoveToDefault() => MoveTo(UiStates.Default);
    public void MoveToPause() => MoveTo(UiStates.Pause);
    public void MoveToOptions() => MoveTo(UiStates.Options);
    public void MoveToTitle() => MoveTo(UiStates.MoveToTitle);
    public void MoveToControls() => MoveTo(UiStates.Controls);
    public void MoveToControlsRebind() => MoveTo(UiStates.RebindControls);
    public void MoveToJournal() => MoveTo(UiStates.Journal);
    public void MoveToInventory() => MoveTo(UiStates.Inventory);
    public void MoveToBartering() => MoveTo(UiStates.Bartering);

    public void Update() {
    
    }

    // Private Helper Methods ====================================================================


    /// <summary>
    /// Stop a currently running Ui state.
    /// </summary>
    /// <param name="stateToStop"> State that will stop. </param>
    void StopState(UiStates stateToStop) {

        // Can't stop transition states
        // (MoveToTitle)

        switch (stateToStop) {
            case UiStates.Default:
                // Insert animation!
                Default.gameObject.SetActive(false);
                break;
            case UiStates.Pause:
                // Insert animation!
                Pause.gameObject.SetActive(false);
                break;
            case UiStates.Options:
                // Insert animation!
                Options.gameObject.SetActive(false);
                break;
            case UiStates.Controls:
                // Insert animation!
                Controls.gameObject.SetActive(false);
                break;
            case UiStates.RebindControls:
                // Insert animation!
                RebindControls.gameObject.SetActive(false);
                break;
            case UiStates.Journal:
                // Insert animation!
                Journal.gameObject.SetActive(false);
                break;
            case UiStates.Inventory:
                // Insert animation!
                Inventory.gameObject.SetActive(false);
                break;
            case UiStates.Bartering:
                // Insert animation!
                Bartering.gameObject.SetActive(false);
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
            case UiStates.Default:
                // Insert animation!
                Default.gameObject.SetActive(true);
                break;
            case UiStates.Pause:
                // Insert animation!
                Pause.gameObject.SetActive(true);
                break;
            case UiStates.Options:
                // Insert animation!
                Options.gameObject.SetActive(true);
                break;
            case UiStates.MoveToTitle:
                // Insert animation!
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                break;
            case UiStates.Controls:
                // Insert animation!
                Controls.gameObject.SetActive(true);
                break;
            case UiStates.RebindControls:
                // Insert animation!
                RebindControls.gameObject.SetActive(true);
                break;
            case UiStates.Journal:
                // Insert animation!
                Journal.gameObject.SetActive(true);
                break;
            case UiStates.Inventory:
                // Insert animation!
                Inventory.gameObject.SetActive(true);
                break;
            case UiStates.Bartering:
                // Insert animation!
                Bartering.gameObject.SetActive(true);
                break;
        }
    }
}
