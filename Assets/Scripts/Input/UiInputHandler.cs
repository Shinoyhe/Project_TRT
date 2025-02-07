using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class which manages inputs from the new input system, via PlayerControls.
/// Modded from the input handler from the Unity FPS Microgame.
/// </summary>
public class UiInputHandler : MonoBehaviour, UiControls.IUiInteractActions {
    // Misc Internal Variables ====================================================================

    // Object references
    UiControls _controls;
    public bool IsActive;

    // Input states: set by InputAction callbacks, read by accessors
    private Dictionary<string, bool> _getDown = new() {
        {"_debug", false},
        {"_settings", false},
        {"_progressDialogue", false}
    };

    // Initializers and Finalizers ================================================================

    private void OnEnable() {
        if (_controls == null) {
            _controls = new UiControls();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            _controls.UiInteract.SetCallbacks(this);
        }

        _controls.UiInteract.Enable();
        IsActive = true;
    }

    private void OnDisable() {
        if (_controls != null) {
            _controls.UiInteract.Disable();
            IsActive = false;
        }
    }

    // InputAction Callbacks and Methods ==========================================================

    private void LateUpdate() {
        // LateUpdate is called at the END of every frame, after all Update() calls.
        foreach (string key in _getDown.Keys) {
            _getDown[key] = false;
        }
    }

    private void SetDown(InputAction.CallbackContext context, string input)
    {
        if (context.started) _getDown[input] = true; 
        if (context.canceled) _getDown[input] = false;
    }

    public void OnDebugKey(InputAction.CallbackContext context) { SetDown(context, "_debug"); }
    public void OnPauseKey(InputAction.CallbackContext context) { SetDown(context, "_settings"); }
    public void OnProgressDialogue(InputAction.CallbackContext context) { SetDown(context, "_progressDialogue"); }

    // Public Accessor Methods ====================================================================

    public bool GetDebugDown() { return IsActive && _getDown["_debug"]; }
    public bool GetSettingsDown() { return IsActive && _getDown["_settings"];  }
    public bool GetProgressDialogueDown() { return IsActive && _getDown["_progressDialogue"];  }
}