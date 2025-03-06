using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUi : MonoBehaviour
{
    // Parameters =================================================================================

    [Header("Shared Dependency")]
    public Canvas NavBar;

    // Dialogue
    [Header("Dependencies")]
    public Canvas Default;
    public Canvas Pause;
    public Canvas Options;
    public Canvas Controls;
    public Canvas Journal;
    public Canvas Inventory;
    public Canvas Bartering;

    public enum UiStates {
        Default,
        Pause,
        Options, 
        MoveToTitle,
        Controls,
        Journal, 
        Inventory,
        Bartering 
    }

    private UiStates _currentCanvasState;
    private NavBarController _navBarController;

    // Initializers and Update ================================================================
    void Start() {
        if (Default == null) {
            Debug.LogError("Default Canvas dependency not set.");
        }

        _navBarController = NavBar.gameObject.GetComponent<NavBarController>();

        // Swap with Accessibility Check
        MoveTo(UiStates.Default);
    }
    
    /// <summary>
    /// Check for player input and update UI.
    /// </summary>
    public void Update() {

        if(GameManager.PlayerInput.GetMenu1Down()) {
            if (_currentCanvasState == UiStates.Inventory) {
                MoveToDefault();
            } else {
                MoveToInventory();
            }
        }

        if (GameManager.PlayerInput.GetMenu2Down()) {
            if (_currentCanvasState == UiStates.Journal) {
                MoveToDefault();
            } else {
                MoveToJournal();
            }
        }

        if (GameManager.PlayerInput.GetStartDown()) {
            if (_currentCanvasState == UiStates.Pause) {
                MoveToDefault();
            } else {
                MoveToPause();
            }
        }
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
    public void MoveToJournal() => MoveTo(UiStates.Journal);
    public void MoveToInventory() => MoveTo(UiStates.Inventory);
    public void MoveToBartering() => MoveTo(UiStates.Bartering);

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Show or hide the Nav Bar based on the state.
    /// </summary>
    /// <param name="currentState"></param>
    void ToggleNavBar(UiStates currentState) {

        bool usingNavBar = false;

        if(currentState == UiStates.Pause || currentState == UiStates.Journal || currentState == UiStates.Inventory) {
            usingNavBar = true;
        }

        // Add Animation here!
        NavBar.gameObject.SetActive(usingNavBar);

        switch (currentState) {
            case UiStates.Pause:
                _navBarController.InitNavBar(2);
                break;
            case UiStates.Journal:
                _navBarController.InitNavBar(0);
                break;
            case UiStates.Inventory:
                _navBarController.InitNavBar(1);
                break;
        }
    }


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
                GameManager.Player.Movement.TogglePlayerMovement(false);
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

        // Setup Nav Bar if needed!
        ToggleNavBar(_currentCanvasState);

        switch (stateToStart) {
            case UiStates.Default:
                // Insert animation!
                GameManager.Player.Movement.TogglePlayerMovement(true);
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
