using UnityEngine;

public class InGameUi : MonoBehaviour
{
    // Parameters =================================================================================

    public Canvas HudCanvas;
    public Canvas InventoryCanvas;

    private Canvas _currentCanvas;
    private enum CanvasState {
        HudCanvas,
        InventoryCanvas
    }
    private CanvasState _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() 
    {
        if (HudCanvas == null || InventoryCanvas == null) {
            Debug.LogError("Start UI not setup!");
        }

        _currentCanvasState = CanvasState.HudCanvas;
        _currentCanvas = HudCanvas;
    }

    // Public Utility Methods ====================================================================

    public void SwitchToHudScreen() 
    {
        SwitchCanvas(HudCanvas);
        TimeLoopManager.SetLoopPaused(false);
        _currentCanvasState = CanvasState.HudCanvas;
    }

    public void SwitchToInventory() 
    {
        SwitchCanvas(InventoryCanvas);
        TimeLoopManager.SetLoopPaused(true);
        _currentCanvasState = CanvasState.InventoryCanvas;
    }

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Switch from current canvas to a new canvas.
    /// </summary>
    /// <param name="canvas">New canvas to show.</param>
    void SwitchCanvas(Canvas canvas) 
    {
        if (canvas == null) return;

        if (_currentCanvas != null) {
            _currentCanvas.gameObject.SetActive(false);
        }

        canvas.gameObject.SetActive(true);
        _currentCanvas = canvas;
    }

    // Update is called once per frame
    void Update() 
    {
        if (GameManager.UiInput == null) return;

        // Keyboard input checking
        if (_currentCanvasState == CanvasState.HudCanvas) {
            if (GameManager.UiInput.GetSettingsDown()) {
                SwitchToInventory();
                return;
            }
        }

        if (_currentCanvasState != CanvasState.HudCanvas) {
            if (GameManager.UiInput.GetSettingsDown()) {
                SwitchToHudScreen();
                return;
            }
        }
    }
}
