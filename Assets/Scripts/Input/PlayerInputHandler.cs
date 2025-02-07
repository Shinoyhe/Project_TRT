using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class which manages inputs from the new input system, via PlayerControls.
/// Modded from the input handler from the Unity FPS Microgame.
/// </summary>
public class PlayerInputHandler : MonoBehaviour, PlayerControls.IMainControlsActions 
{
    // Misc Internal Variables ====================================================================

    // Object references
    PlayerControls _controls;
    public bool IsActive;

    // Input states: set by InputAction callbacks, read by accessors
    private Vector2 _controlAxisVector;
    private Dictionary<string, bool> _getDown = new() {
        {"_primaryTrigger", false},
        {"_secondaryTrigger", false},
        {"_start", false},
        {"_affirm", false},
        {"_reject", false},
        {"_menu1", false},
        {"_menu2", false}
    };
   
    // Initializers and Finalizers ================================================================

    private void OnEnable() {
        if (_controls == null) {
            _controls = new PlayerControls();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            _controls.MainControls.SetCallbacks(this);
        }

        _controls.MainControls.Enable();
        IsActive = true;
    }

    private void OnDisable() {
        if (_controls != null) {
            _controls.MainControls.Disable();
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

    public void OnControlAxis(InputAction.CallbackContext context) 
    {
        _controlAxisVector = context.ReadValue<Vector2>();
    }

    private void SetDown(InputAction.CallbackContext context, string input)
    {
        if (context.started) _getDown[input] = true; 
        if (context.canceled) _getDown[input] = false;
    }

    public void OnPrimaryTrigger(InputAction.CallbackContext context) { SetDown(context, "_primaryTrigger"); }
    public void OnSecondaryTrigger(InputAction.CallbackContext context) { SetDown(context, "_secondaryTrigger"); }
    public void OnAffirmButton(InputAction.CallbackContext context) { SetDown(context, "_affirm"); }
    public void OnStartButton(InputAction.CallbackContext context) { SetDown(context, "_start"); }
    public void OnRejectButton(InputAction.CallbackContext context) { SetDown(context, "_reject"); }
    public void OnMenuButton1(InputAction.CallbackContext context) { SetDown(context, "_menu1"); }
    public void OnMenuButton2(InputAction.CallbackContext context) { SetDown(context, "_menu2"); }

    // Public Accessor Methods ====================================================================

    /// <summary>
    /// Accessor for the last held values of the lateral move inputs.
    /// </summary>
    /// <returns>Vector3 - last known move input.</returns>
    public Vector3 GetControlInput() 
    {
        if (!IsActive) {
            return Vector3.zero;
        }

        Vector3 move = new(_controlAxisVector.x, 0f, _controlAxisVector.y);

        // 'Normalize' move vector but allow for sub-one values.
        return Vector3.ClampMagnitude(move, 1);
    }

    public bool GetPrimaryTriggerDown() { return IsActive && _getDown["_primaryTrigger"]; }
    public bool GetSecondaryTriggerDown() { return IsActive && _getDown["_secondaryTrigger"]; }
    public bool GetStartDown() { return IsActive && _getDown["_start"]; }
    public bool GetAffirmDown() { return IsActive && _getDown["_affirm"]; }
    public bool GetRejectDown() { return IsActive && _getDown["_reject"]; }
    public bool GetMenu1Down() { return IsActive && _getDown["_menu1"]; }
    public bool GetMenu2Down() { return IsActive && _getDown["_menu2"]; }
}