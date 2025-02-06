using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class which manages inputs from the new input system, via PlayerControls.
/// Modded from the input handler from the Unity FPS Microgame.
/// </summary>
public class UiInputHandler : Singleton<UiInputHandler>, UiControls.IUiInteractActions {
    // Misc Internal Variables ====================================================================

    // Object references
    UiControls _controls;
    private bool _isActive;

    // Input states: set by InputAction callbacks, read by accessors
    private bool _debugInputDown;
    private bool _settingsDown;
    private bool _progressDialogueDown;

    // Initializers and Finalizers ================================================================

    private void OnEnable() {
        if (_controls == null) {
            _controls = new UiControls();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            _controls.UiInteract.SetCallbacks(this);
        }

        _controls.UiInteract.Enable();
        _isActive = true;
    }

    private void OnDisable() {
        if (_controls != null) {
            _controls.UiInteract.Disable();
            _isActive = false;
        }
    }

    // InputAction Callbacks and Methods ==========================================================

    private void LateUpdate() {
        // LateUpdate is called at the END of every frame, after all Update() calls.
        _debugInputDown = false;
        _settingsDown = false;
        _progressDialogueDown = false;
    }

    /// <summary>
    /// Callback function used with the PlayerControls object internal to PlayerInputHandler.
    /// DO NOT CALL MANUALLY.
    /// </summary>
    /// <param name="context"></param>
    public void OnDebugKey(InputAction.CallbackContext context) {
        if (context.started) _debugInputDown = true;
        if (context.canceled) _debugInputDown = false;
    }

    /// <summary>
    /// Callback function used with the PlayerControls object internal to PlayerInputHandler.
    /// DO NOT CALL MANUALLY.
    /// </summary>
    /// <param name="context"></param>
    public void OnPauseKey(InputAction.CallbackContext context) {
        if (context.started) _settingsDown = true;
        if (context.canceled) _settingsDown = false;
    }

    /// <summary>
    /// Callback function used with the PlayerControls object internal to PlayerInputHandler.
    /// DO NOT CALL MANUALLY.
    /// </summary>
    /// <param name="context"></param>
    public void OnProgressDialogue(InputAction.CallbackContext context) {
        if (context.started) _progressDialogueDown = true;
        if (context.canceled) _progressDialogueDown = false;
    }

    // Public Accessor Methods ====================================================================

    /// <summary>
    /// Accessor for if inputs are currently accepted.
    /// </summary>
    /// <returns>bool - if PlayerInputHandler can process input.</returns>
    public bool GetCanProcessInput() {
        return _isActive;//Cursor.lockState == CursorLockMode.Locked;
    }

    /// <summary>
    /// Accessor for if the debug input was pressed on the last frame.
    /// </summary>
    /// <returns>bool - if the debug input was pressed down on the last frame.</returns>
    public bool GetDebugDown() {
        return GetCanProcessInput() && _debugInputDown;
    }

    /// <summary>
    /// Accessor for if the setting input was pressed on the last frame.
    /// </summary>
    /// <returns>bool - if the setting input was pressed down on the last frame.</returns>
    public bool GetSettingsDown() {
        return GetCanProcessInput() && _settingsDown;
    }

    /// <summary>
    /// Accessor for if the dialogue down input was pressed on the last frame.
    /// </summary>
    /// <returns>bool - if the dialogue down input was pressed down on the last frame.</returns>
    public bool GetProgressDialogueDown() {
        return GetCanProcessInput() && _progressDialogueDown;
    }


    public void SetActive(bool isActive) {
        _isActive = isActive;
    }
}