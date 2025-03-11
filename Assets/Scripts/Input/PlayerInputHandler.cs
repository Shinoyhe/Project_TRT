using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class which manages inputs from the new input system, via PlayerControls.
/// Modded from the input handler from the Unity FPS Microgame.
/// </summary>
public class PlayerInputHandler : MonoBehaviour, PlayerControls.IMainControlsActions, PlayerControls.IDebugActions
{
    [ReadOnly] public InputControlScheme LastUsedScheme;
    public InputControlScheme KeyboardScheme => _controls.KeyboardMouseScheme;
    public InputControlScheme GamepadScheme => _controls.GamepadScheme;
    public bool AllowNavbar;

    // Misc Internal Variables ====================================================================

    // Object references
    PlayerControls _controls;

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
        {"_click", false},
        {"_debug0", false},
        {"_debug1", false},
        {"_debug2", false}
    };

    private Dictionary<string, bool> _get = new() {};
   
    // Initializers and Finalizers ================================================================

    private void OnEnable() 
    {
        if (_controls == null) {
            _controls = new PlayerControls();
            // Tell the "MainControls" action map that we want to get told about
            // when actions get triggered.
            _controls.MainControls.SetCallbacks(this);
            // Likewise for the "Debug" action map.
            _controls.Debug.SetCallbacks(this);
        }

        foreach (var scheme in _controls.asset.controlSchemes) {
            Debug.Log(scheme.name);
        }

        // Initialize the _get dict from the _getDown dict
        foreach (string key in _getDown.Keys) {
            _get[key] = false;
        }

        _controls.MainControls.Enable();
        _controls.Debug.Enable();
        AllowNavbar = true;
    }

    private void OnDisable() 
    {
        _controls?.MainControls.Disable();
    }

    // InputAction Callbacks and Methods ==========================================================

    private void LateUpdate() 
    {
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

        if (context.started) {
            if (context.started) {
                UpdateLastUsedScheme(context);
            }
        }
    }

    private void SetDown(InputAction.CallbackContext context, string input)
    {
        if (context.started) _getDown[input] = _get[input] = true; 
        if (context.canceled) _getDown[input] = _get[input] = false;

        if (context.started) {
            UpdateLastUsedScheme(context);
        }
    }

    private void UpdateLastUsedScheme(InputAction.CallbackContext context)
    {
        foreach (InputControlScheme scheme in _controls.controlSchemes) {
            if (scheme.SupportsDevice(context.control.device)) {
                LastUsedScheme = scheme;
                return;
            }
        }
    }

    public void OnPrimaryTrigger(InputAction.CallbackContext context) { SetDown(context, "_primaryTrigger"); }
    public void OnSecondaryTrigger(InputAction.CallbackContext context) { SetDown(context, "_secondaryTrigger"); }
    public void OnAffirmButton(InputAction.CallbackContext context) { SetDown(context, "_affirm"); }
    public void OnStartButton(InputAction.CallbackContext context) { SetDown(context, "_start"); }
    public void OnRejectButton(InputAction.CallbackContext context) { SetDown(context, "_reject"); }
    public void OnMenuButton1(InputAction.CallbackContext context) { SetDown(context, "_menu1"); }
    public void OnMenuButton2(InputAction.CallbackContext context) { SetDown(context, "_menu2"); }
    public void OnClick(InputAction.CallbackContext context) { SetDown(context, "_click"); }
    public void OnDebug0(InputAction.CallbackContext context) { SetDown(context, "_debug0"); }
    public void OnDebug1(InputAction.CallbackContext context) { SetDown(context, "_debug1"); }
    public void OnDebug2(InputAction.CallbackContext context) { SetDown(context, "_debug2"); }

    // OnPointer and OnScroll are used only by the event system. 
    // Don't do anything with it manually, at least for now.
    public void OnPointer(InputAction.CallbackContext context) { return; } 
    public void OnScroll(InputAction.CallbackContext context) { return; }

    // Public Accessor Methods ====================================================================

    /// <summary>
    /// Accessor for the last held values of the lateral move inputs.
    /// </summary>
    /// <returns>Vector3 - last known move input.</returns>
    public Vector3 GetControlInput() 
    {
        Vector3 move = new(_controlAxisVector.x, 0f, _controlAxisVector.y);

        // 'Normalize' move vector but allow for sub-one values.
        return Vector3.ClampMagnitude(move, 1);
    }

    public bool GetPrimaryTriggerDown() { return _getDown["_primaryTrigger"]; }
    public bool GetSecondaryTriggerDown() { return _getDown["_secondaryTrigger"]; }
    public bool GetStartDown() { return _getDown["_start"]; }
    public bool GetAffirmDown() { return _getDown["_affirm"]; }
    public bool GetRejectDown() { return _getDown["_reject"]; }
    public bool GetMenu1Down() { return _getDown["_menu1"]; }
    public bool GetMenu1() { return _get["_menu1"]; }
    public bool GetMenu2Down() { return _getDown["_menu2"]; }
    public bool GetClickDown() { return _getDown["_click"]; }
    public bool GetDebug0Down() { return _getDown["_debug0"]; }
    public bool GetDebug1Down() { return _getDown["_debug1"]; }
    public bool GetDebug2Down() { return _getDown["_debug2"]; }
}