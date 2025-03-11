using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class StartUi : MonoBehaviour {
    // Parameters =================================================================================

    [Header("Dependencies")]
    public Canvas TitleScreen;
    public Canvas Credits;
    public Canvas Options;
    public Canvas AccessibilityCheck;
    public Canvas Controls;

    public enum UiState {
        Title = 0,
        Credits = 1,
        Options = 2,
        NewGame = 3,
        ContinueGame = 4,
        Quit = 5,
        AccessibilityCheck = 6,
        Controls = 7
    }

    private UiState _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() 
    {
        if (TitleScreen == null) {
            Debug.LogError("TitleScreen dependency not set.");
        }

        // Swap with Accessibility Check
        MoveTo(UiState.Title);
    }

    // Public Utility Methods ====================================================================

    /// <summary>
    /// Transition Start Ui to a new state.
    /// </summary>
    /// <param name="newState"> State to move to. </param>
    public void MoveTo(UiState newState) {

        StopState(_currentCanvasState);
        StartState(newState);
    }

    // Used for button OnClick calls as they don't let enums to be passed through :|
    public void MoveToTitle() => MoveTo(UiState.Title);
    public void MoveToCredits() => MoveTo(UiState.Credits);
    public void MoveToOptions() => MoveTo(UiState.Options);
    public void MoveToAccessibilityCheck() => MoveTo(UiState.AccessibilityCheck);
    public void MoveToNewGame() => MoveTo(UiState.NewGame);
    public void MoveToContinueGame() => MoveTo(UiState.ContinueGame);
    public void MoveToQuit() => MoveTo(UiState.Quit);
    public void MoveToControls() => MoveTo(UiState.Controls);


    // Private Helper Methods ====================================================================


    /// <summary>
    /// Stop a currently running Ui state.
    /// </summary>
    /// <param name="stateToStop"> State that will stop. </param>
    void StopState(UiState stateToStop) {

        // Can't stop transition states
        // (NewGame, ContinueGame, Quit)

        switch (stateToStop) {
            case UiState.Title:
                // Insert animation!
                TitleScreen.gameObject.SetActive(false);
                break;
            case UiState.Credits:
                // Insert animation!
                Credits.gameObject.SetActive(false);
                break;
            case UiState.Options:
                // Insert animation!
                Options.gameObject.SetActive(false);
                break;
            case UiState.AccessibilityCheck:
                // Insert animation!
                AccessibilityCheck.gameObject.SetActive(false);
                break;
            case UiState.Controls:
                // Insert animation!
                Controls.gameObject.SetActive(false);
                break;
        }

    }

    /// <summary>
    /// Start a new state.
    /// </summary>
    /// <param name="stateToStart">State that will start.</param>
    void StartState(UiState stateToStart) {

        // Set our new state
        _currentCanvasState = stateToStart;

        switch (stateToStart) {
            case UiState.Title:
                // Insert animation!
                TitleScreen.gameObject.SetActive(true);
                break;
            case UiState.Credits:
                // Insert animation!
                Credits.gameObject.SetActive(true);
                break;
            case UiState.Options:
                // Insert animation!
                Options.gameObject.SetActive(true);
                break;
            case UiState.NewGame:
                // Start new game
                // -- TEMP --
                SceneManager.LoadScene("Tutorial");
                // -- TEMP --
                break;
            case UiState.ContinueGame:
                // Continue game
                // -- TEMP --
                SceneManager.LoadScene(2);
                // -- TEMP --
                break;
            case UiState.Quit:
                // Save then quit
                // -- TEMP -- 
                Application.Quit();
                // -- TEMP --
                break;
            case UiState.AccessibilityCheck:
                // Insert animation!
                AccessibilityCheck.gameObject.SetActive(true);
                break;
            case UiState.Controls:
                // Insert animation!
                Controls.gameObject.SetActive(true);
                break;
        }

    }
}
