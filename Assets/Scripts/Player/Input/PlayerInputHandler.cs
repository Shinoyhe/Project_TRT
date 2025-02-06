using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class which manages inputs from the new input system, via PlayerControls.
/// Modded from the input handler from the Unity FPS Microgame.
/// </summary>
public class PlayerInputHandler : Singleton<PlayerInputHandler>, PlayerControls.IPlayerMovementActions {
    // Parameters =================================================================================

    [SerializeField, Tooltip("Sensitivity multiplier for moving the camera around")]
    private float LookSensitivity = 1f;
    [SerializeField, Tooltip("Used to flip the vertical input axis")]
    private bool InvertYAxis = false;
    [SerializeField, Tooltip("Used to flip the horizontal input axis")]
    private bool InvertXAxis = false;

    // Misc Internal Variables ====================================================================

    private enum Axis { Horizontal, Vertical };
    // Object references
    PlayerControls _controls;
    private bool _isActive;

    // Input states: set by InputAction callbacks, read by accessors
    private Vector2 _moveInputVector;
    private Vector2 _lookDeltaVector;
    private bool _jumpInputDown;
    private bool _sprintInput;
    private bool _interactDown;
   
    // Initializers and Finalizers ================================================================

    private void OnEnable() {
        if (_controls == null) {
            _controls = new PlayerControls();
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
        _jumpInputDown = false;
        _interactDown = false;
    }

    /// <summary>
    /// Callback function used with the PlayerControls object internal to PlayerInputHandler.
    /// DO NOT CALL MANUALLY.
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context) {
        _moveInputVector = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Callback function used with the PlayerControls object internal to PlayerInputHandler.
    /// DO NOT CALL MANUALLY.
    /// </summary>
    /// <param name="context"></param>
    public void OnCameraLook(InputAction.CallbackContext context) {
        _lookDeltaVector = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Callback function used with the PlayerControls object internal to PlayerInputHandler.
    /// DO NOT CALL MANUALLY.
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context) {
        if (context.started) _jumpInputDown = true;
        if (context.canceled) _jumpInputDown = false;
    }

    /// <summary>
    /// Callback function used with the PlayerControls object internal to PlayerInputHandler.
    /// DO NOT CALL MANUALLY.
    /// </summary>
    /// <param name="context"></param>
    public void OnInteract(InputAction.CallbackContext context) {
        if (context.started) _interactDown = true;
        if (context.canceled) _interactDown = false;
    }

    /// <summary>
    /// Callback function used with the PlayerControls object internal to PlayerInputHandler.
    /// DO NOT CALL MANUALLY.
    /// </summary>
    /// <param name="context"></param>
    public void OnSprintHold(InputAction.CallbackContext context) {
        if (context.started) _sprintInput = true;
        if (context.canceled) _sprintInput = false;
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
    /// Accessor for the last held values of the lateral move inputs.
    /// </summary>
    /// <returns>Vector3 - last known move input.</returns>
    public Vector3 GetMoveInput() {
        if (!GetCanProcessInput()) {
            return Vector3.zero;
        }

        Vector3 move = new(_moveInputVector.x, 0f, _moveInputVector.y);

        // 'Normalize' move vector but allow for sub-one values.
        return Vector3.ClampMagnitude(move, 1);
    }

    /// <summary>
    /// Accessor for if the jump input was pressed on the last frame.
    /// </summary>
    /// <returns>bool - if the jump input was pressed down on the last frame.</returns>
    public bool GetJumpInputDown() {
        return GetCanProcessInput() && _jumpInputDown;
    }

    /// <summary>
    /// Accessor for if the interact input was pressed on the last frame.
    /// </summary>
    /// <returns>bool - if the interact input was pressed down on the last frame.</returns>
    public bool GetInteractDown() {
        return GetCanProcessInput() && _interactDown;
    }

    /// <summary>
    /// Accessor for if the sprint input was held on the last frame.
    /// </summary>
    /// <returns>bool - last known hold state of the sprint input.</returns>
    public bool GetSprintInputHeld() {
        return GetCanProcessInput() && _sprintInput;
    }

    /// <summary>
    /// Accessor for the horizontal delta on the last frame for the look input.
    /// </summary>
    /// <returns>float - horizontal delta of the look input on the last frame.</returns>
    public float GetLookInputsHorizontal() {
        return GetLookInputsAxis(Axis.Horizontal);
    }

    /// <summary>
    /// Accessor for the vertical delta on the last frame for the look input.
    /// </summary>
    /// <returns>float - vertical delta of the look input on the last frame.</returns>
    public float GetLookInputsVertical() {
        return GetLookInputsAxis(Axis.Vertical);
    }

    private float GetLookInputsAxis(Axis axis) {
        // Shared functionality for GetLookInputsHorizontal and GetLookInputsVertical.
        // * Takes as argument an Axis enum (either Horizontal or Vertical, input is 2D)
        // * Returns the input delta on the last frame for that axis.
        // ================

        // If we can't process input, stop.
        if (!GetCanProcessInput()) {
            return 0f;
        }

        // Check if this look input is coming from a controller!
        bool isGamepad;
        float axisValue;

        if (axis == Axis.Horizontal) {
            isGamepad = _lookDeltaVector.x != 0f;
            axisValue = _lookDeltaVector.x;

            if (InvertXAxis) {
                axisValue *= -1f;
            }
        } else { // axis == Axis.Vertical
            isGamepad = _lookDeltaVector.y != 0f;
            axisValue = _lookDeltaVector.y;

            if (InvertYAxis) {
                axisValue *= -1f;
            }
        }

        // Apply sensitivity multiplier
        axisValue *= LookSensitivity;

        if (isGamepad) {
            // Mouse input is already deltaTime-dependant, so only scale input with frame time if
            // it's coming from the controller.
            axisValue *= Time.deltaTime;
        } else {
            // Reduce mouse input amount to be equivalent to stick movement.
            axisValue *= 0.01f;

#if UNITY_WEBGL
            // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more.
            axisValue *= WebglLookSensitivityMultiplier;
#endif
        }

        return axisValue;
    }
    public void SetActive(bool isActive) {
        _isActive = isActive;
    }
}