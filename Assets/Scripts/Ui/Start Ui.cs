using UnityEngine;

public class StartUi : MonoBehaviour {
    // Parameters =================================================================================

    public Canvas TitleScreenCanvas;
    public Canvas CreditsCanvas;

    private Canvas _currentCanvas;
    private enum CanvasState {
        TitleScreen,
        Credits
    }
    private CanvasState _currentCanvasState;

    // Initializers and Update ================================================================
    void Start() 
    {
        if (TitleScreenCanvas == null) {
            Debug.LogError("Start UI not setup!");
        }

        _currentCanvasState = CanvasState.TitleScreen;
        _currentCanvas = TitleScreenCanvas;
    }

    // Public Utility Methods ====================================================================

    public void SwitchToTitleScreen() 
    {
        SwitchCanvas(TitleScreenCanvas);
        _currentCanvasState = CanvasState.TitleScreen;
    }

    public void SwitchToCreditsScreen() 
    {
        SwitchCanvas(CreditsCanvas);
        _currentCanvasState = CanvasState.Credits;
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

        if (_currentCanvasState != CanvasState.TitleScreen) {
            if (GameManager.UiInput.GetSettingsDown()) {
                SwitchToTitleScreen();
            }
        }
    }
}
