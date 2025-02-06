using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartUi : MonoBehaviour {
    // Parameters =================================================================================

    public Canvas TitleScreenCanvas;
    public Canvas CreditsCanvas;

    private Canvas _currentCanvas;
    private enum _canvasState {
        TitleScreen,
        Credits
    }
    private _canvasState _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() {
        if(TitleScreenCanvas == null) {
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

    public void SwitchToCreditsScreen() {
        SwitchCanvas(CreditsCanvas);
        _currentCanvasState = _canvasState.Credits;
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
        if (GameManager.UiInput == null) { return; }

        if (_currentCanvasState != _canvasState.TitleScreen) {

            if (GameManager.UiInput.GetSettingsDown()) {
                SwitchToTitleScreen();
            }
        }
    }
}
