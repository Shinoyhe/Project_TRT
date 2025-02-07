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
    private bool _primaryTrigDown,
                 _secondaryTrigDown,
                 _startDown,
                 _affirmDown,
                 _rejectDown,
                 _menu1Down,
                 _menu2Down;
   
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
        _primaryTrigDown = false;
        _secondaryTrigDown = false;
        _startDown = false;
        _affirmDown = false;
        _rejectDown = false;
        _menu1Down = false;
        _menu2Down = false;
    }

    public void OnControlAxis(InputAction.CallbackContext context) 
    {
        _controlAxisVector = context.ReadValue<Vector2>();
    }

    public void OnPrimaryTrigger(InputAction.CallbackContext context)
    {
        if (context.started) _primaryTrigDown = true;
        if (context.canceled) _primaryTrigDown = false;
    }

    public void OnSecondaryTrigger(InputAction.CallbackContext context)
    {
        if (context.started) _secondaryTrigDown = true;
        if (context.canceled) _secondaryTrigDown = false;
    }

    public void OnStartButton(InputAction.CallbackContext context)
    {
        if (context.started) _startDown = true;
        if (context.canceled) _startDown = false;
    }

    public void OnAffirmButton(InputAction.CallbackContext context)
    {
        if (context.started) _affirmDown = true;
        if (context.canceled) _affirmDown = false;
    }

    public void OnRejectButton(InputAction.CallbackContext context)
    {
        if (context.started) _rejectDown = true;
        if (context.canceled) _rejectDown = false;
    }

    public void OnMenuButton1(InputAction.CallbackContext context)
    {
        if (context.started) _menu1Down = true;
        if (context.canceled) _menu1Down = false;
    }

    public void OnMenuButton2(InputAction.CallbackContext context)
    {
        if (context.started) _menu2Down = true;
        if (context.canceled) _menu2Down = false;
    }

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

    public bool GetPrimaryTrigger() { return _primaryTrigDown; }
    public bool GetSecondaryTrigger() { return _secondaryTrigDown; }
    public bool GetStartButton() { return _startDown; }
    public bool GetAffirmButton() { return _affirmDown; }
    public bool GetRejectButton() { return _rejectDown; }
    public bool GetMenuButton1() { return _menu1Down; }
    public bool GetMenuButton2() { return _menu2Down; }
}