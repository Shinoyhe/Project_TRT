using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUi : MonoBehaviour
{
    // Parameters =================================================================================

    public Canvas HudCanvas;
    public Canvas InventoryCanvas;
    public Canvas OptionsCanvas;
    public Canvas CreditsCanvas;

    private Canvas _currentCanvas;
    private enum _canvasState {
        HudCanvas,
        OptionScreen,
        CreditsScreen,
        InventoryCanvas
    }
    private _canvasState _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() {
        if (HudCanvas == null || OptionsCanvas == null || CreditsCanvas == null || InventoryCanvas == null) {
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
    public void SwitchToOptionsMenu() {
        SwitchCanvas(OptionsCanvas);
        _currentCanvasState = _canvasState.OptionScreen;
    }
    public void SwitchToCreditsMenu() {
        SwitchCanvas(CreditsCanvas);
        _currentCanvasState = _canvasState.CreditsScreen;
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
        if (PlayerInputHandler.Instance == null) { return; }

        // Keyboard input checking
        if (_currentCanvasState == _canvasState.HudCanvas) {

            if (PlayerInputHandler.Instance.GetSettingsDown()) {
                SwitchToInventory();
                return;
            }
            if (PlayerInputHandler.Instance.GetInteractDown()) {
                //SwitchToInventory();
                return;
            }
        }

        if (_currentCanvasState != _canvasState.HudCanvas) {

            if (PlayerInputHandler.Instance.GetSettingsDown()) {
                SwitchToHudScreen();
                return;
            }
        }


    }
}
