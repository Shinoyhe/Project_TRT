using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartUi : MonoBehaviour {
    // Parameters =================================================================================

    public Canvas TitleScreenCanvas;
    public Canvas OptionsCanvas;
    public Canvas CreditsCanvas;

    private Canvas _currentCanvas;
    private enum _canvasState {
        TitleScreen,
        OptionScreen,
        CreditsScreen
    }
    private _canvasState _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() {
        if(TitleScreenCanvas == null || OptionsCanvas == null || CreditsCanvas == null) {
            Debug.LogError("Start Ui not setup!");
        }

        _currentCanvasState = _canvasState.TitleScreen;
        _currentCanvas = TitleScreenCanvas;
    }

    // Public Utility Methods ====================================================================

    public void SwitchToTitleScreen() {
        SwitchCanvas(TitleScreenCanvas);
        _currentCanvasState = _canvasState.TitleScreen;
    }
    public void SwitchToOptionsMenu() {
        SwitchCanvas(OptionsCanvas);
        _currentCanvasState = _canvasState.OptionScreen;
    }
    public void SwitchToCreditsMenu() {
        SwitchCanvas(CreditsCanvas);
        _currentCanvasState = _canvasState.CreditsScreen;
    }

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Switch from current canvas to a new canvas.
    /// </summary>
    /// <param name="canvas">New canvas to show.</param>
    void SwitchCanvas(Canvas canvas) {

        if (canvas == null) return;

        if (_currentCanvas != null) {
            _currentCanvas.gameObject.SetActive(false);
        }

        canvas.gameObject.SetActive(true);
        _currentCanvas = canvas;
    }

    // Update is called once per frame
    void Update() {
        if (UiInputHandler.Instance == null) { return; }

        // Keyboard input checking
        if (_currentCanvasState == _canvasState.TitleScreen) {

            if (UiInputHandler.Instance.GetSettingsDown()) {
                SwitchToOptionsMenu();
                return;
            }
        }

        if (_currentCanvasState != _canvasState.TitleScreen) {

            if (UiInputHandler.Instance.GetSettingsDown()) {
                SwitchToTitleScreen();
            }
        }
    }
}
