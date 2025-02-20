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

    public enum StartUiState {
        Title = 0,
        Credits = 1,
        Options = 2,
        NewGame = 3,
        ContinueGame = 4,
        Quit = 5,
        AccessibilityCheck = 6
    }

    private StartUiState _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() 
    {
        if (TitleScreen == null) {
            Debug.LogError("TitleScreen dependency not set.");
        }

        // Swap with Accessibility Check
        MoveTo(StartUiState.Title);
    }

    // Public Utility Methods ====================================================================

    /// <summary>
    /// Transition Start Ui to a new state.
    /// </summary>
    /// <param name="newState"> State to move to. </param>
    public void MoveTo(StartUiState newState) {

        StopState(_currentCanvasState);
        StartState(newState);
    }

    // Used for button OnClick calls as they don't let enums to be passed through :|
    public void MoveToTitle() => MoveTo(StartUiState.Title);
    public void MoveToCredits() => MoveTo(StartUiState.Credits);
    public void MoveToOptions() => MoveTo(StartUiState.Options);
    public void MoveToAccessibilityCheck() => MoveTo(StartUiState.AccessibilityCheck);
    public void MoveToNewGame() => MoveTo(StartUiState.NewGame);
    public void MoveToContinueGame() => MoveTo(StartUiState.ContinueGame);
    public void MoveToQuit() => MoveTo(StartUiState.Quit);


    // Private Helper Methods ====================================================================


    /// <summary>
    /// Stop a currently running Ui state.
    /// </summary>
    /// <param name="stateToStop"> State that will stop. </param>
    void StopState(StartUiState stateToStop) {

        // Can't stop transition states
        // (NewGame, ContinueGame, Quit)

        switch (stateToStop) {
            case StartUiState.Title:
                // Insert animation!
                TitleScreen.gameObject.SetActive(false);
                break;
            case StartUiState.Credits:
                // Insert animation!
                Credits.gameObject.SetActive(false);
                break;
            case StartUiState.Options:
                // Insert animation!
                Options.gameObject.SetActive(false);
                break;
            case StartUiState.AccessibilityCheck:
                // Insert animation!
                AccessibilityCheck.gameObject.SetActive(false);
                break;
        }

    }

    /// <summary>
    /// Start a new state.
    /// </summary>
    /// <param name="stateToStart">State that will start.</param>
    void StartState(StartUiState stateToStart) {

        // Set our new state
        _currentCanvasState = stateToStart;

        switch (stateToStart) {
            case StartUiState.Title:
                // Insert animation!
                TitleScreen.gameObject.SetActive(true);
                break;
            case StartUiState.Credits:
                // Insert animation!
                Credits.gameObject.SetActive(true);
                break;
            case StartUiState.Options:
                // Insert animation!
                Options.gameObject.SetActive(true);
                break;
            case StartUiState.NewGame:
                // Start new game
                // -- TEMP --
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                // -- TEMP --
                break;
            case StartUiState.ContinueGame:
                // Continue game
                // -- TEMP --
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                // -- TEMP --
                break;
            case StartUiState.Quit:
                // Save then quit
                // -- TEMP -- 
                Application.Quit();
                // -- TEMP --
                break;
            case StartUiState.AccessibilityCheck:
                // Insert animation!
                AccessibilityCheck.gameObject.SetActive(true);
                break;
        }

    }
}
