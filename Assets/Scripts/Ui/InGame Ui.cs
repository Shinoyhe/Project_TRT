using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUi : MonoBehaviour
{
    // Parameters =================================================================================

    public Canvas HudCanvas;
    public Canvas InventoryCanvas;

    private Canvas _currentCanvas;
    private enum _canvasState {
        HudCanvas,
        InventoryCanvas
    }
    private _canvasState _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() {
        if (HudCanvas == null || InventoryCanvas == null) {
            Debug.LogError("Start Ui not setup!");
        }

        _currentCanvasState = _canvasState.HudCanvas;
        _currentCanvas = HudCanvas;
    }

    // Public Utility Methods ====================================================================

    public void SwitchToHudScreen() {
        SwitchCanvas(HudCanvas);
        _currentCanvasState = _canvasState.HudCanvas;
    }
    public void SwitchToInventory() {
        SwitchCanvas(InventoryCanvas);
        _currentCanvasState = _canvasState.InventoryCanvas;
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

        // Keyboard input checking
        if (_currentCanvasState == _canvasState.HudCanvas) {

            if (GameManager.UiInput.GetSettingsDown()) {
                SwitchToInventory();
                return;
            }
        }

        if (_currentCanvasState != _canvasState.HudCanvas) {

            if (GameManager.UiInput.GetSettingsDown()) {
                SwitchToHudScreen();
                return;
            }
        }


    }
}
