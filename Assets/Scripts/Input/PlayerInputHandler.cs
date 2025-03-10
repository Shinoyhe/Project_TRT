using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class which manages inputs from the new input system, via PlayerControls.
/// Modded from the input handler from the Unity FPS Microgame.
/// </summary>
public class PlayerInputHandler : MonoBehaviour, PlayerControls.IMainControlsActions, PlayerControls.IDebugActions
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
        {"_menu2", false},
        {"_debug0", false},
        {"_debug1", false},
        {"_debug2", false}
    };

    private Dictionary<string, bool> _get = new() {};
   
    // Initializers and Finalizers ================================================================

    private void OnEnable() {
        if (_controls == null) {
            _controls = new PlayerControls();
            // Tell the "MainControls" action map that we want to get told about
            // when actions get triggered.
            _controls.MainControls.SetCallbacks(this);
            // Likewise for the "Debug" action map.
            _controls.Debug.SetCallbacks(this);
        }

        // Initialize the _get dict from the _getDown dict
        foreach (string key in _getDown.Keys) {
            _get[key] = false;
        }

        _controls.MainControls.Enable();
        _controls.Debug.Enable();
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
        
        // ==============================================================================
        // NOTE:
        // Not happy that we're calling ToList every frame, but that's the cleanest way
        // to make this work without having 7 boolean assignments in a row.
        //
        // The performance hit should be minimal for us. If it isn't, change this.
        // ==============================================================================

        foreach (string key in _getDown.Keys.ToList()) {
            _getDown[key] = false;
        }
    }

    public void OnControlAxis(InputAction.CallbackContext context) 
    {
        _controlAxisVector = context.ReadValue<Vector2>();
    }

    private void SetDown(InputAction.CallbackContext context, string input)
    {
        if (context.started) _getDown[input] = _get[input] = true; 
        if (context.canceled) _getDown[input] = _get[input] = false;

        if (context.started) {
            Debug.Log($"Input: {input} was just pressed down.");
        }
    }

    public void OnPrimaryTrigger(InputAction.CallbackContext context) { SetDown(context, "_primaryTrigger"); }
    public void OnSecondaryTrigger(InputAction.CallbackContext context) { SetDown(context, "_secondaryTrigger"); }
    public void OnAffirmButton(InputAction.CallbackContext context) { SetDown(context, "_affirm"); }
    public void OnStartButton(InputAction.CallbackContext context) { SetDown(context, "_start"); }
    public void OnRejectButton(InputAction.CallbackContext context) { SetDown(context, "_reject"); }
    public void OnMenuButton1(InputAction.CallbackContext context) { SetDown(context, "_menu1"); }
    public void OnMenuButton2(InputAction.CallbackContext context) { SetDown(context, "_menu2"); }
    public void OnDebug0(InputAction.CallbackContext context) { SetDown(context, "_debug0"); }
    public void OnDebug1(InputAction.CallbackContext context) { SetDown(context, "_debug1"); }
    public void OnDebug2(InputAction.CallbackContext context) { SetDown(context, "_debug2"); }

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
    public bool GetMenu1() { return IsActive && _get["_menu1"]; }
    public bool GetMenu2Down() { return IsActive && _getDown["_menu2"]; }
    public bool GetDebug0Down() { return IsActive && _getDown["_debug0"]; }
    public bool GetDebug1Down() { return IsActive && _getDown["_debug1"]; }
    public bool GetDebug2Down() { return IsActive && _getDown["_debug2"]; }
}