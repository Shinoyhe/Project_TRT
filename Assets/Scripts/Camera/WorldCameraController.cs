using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

using Cinemachine;
using NaughtyAttributes;

[ExecuteAlways]
public class WorldCameraController : MonoBehaviour
{
    #region ======== [ OBJECT REFERENCES ] ========
    [Foldout("Object References")] public CinemachineVirtualCamera VirtualCamera;
    [Foldout("Object References")] public CinemachineVirtualCamera VirtualMovement;
    #endregion

    #region ======== [ ENUMS ] ========
    public enum MovementDirection {
        Fixed, Dynamic
    }

    public enum Body {
        Fixed, Transposer, TrackedDolly
    }

    public enum Aim {
        Fixed, Composer
    }
    #endregion

    #region ======== [ PARAMETERS ] ========
    [Header("Parameters")]
    [Tooltip("Will activate this controller on Awake. Should be true for one controller in the scene.")] [SerializeField] 
    private bool activeOnAwake = false;
    [Tooltip("The view camera's FOV.")] [SerializeField] [MinValue(1), MaxValue(179)] [OnValueChanged("AutoUpdateFOV")]
    private int fieldOfView = 60;
    [Tooltip("Time for the camera to move to this controller.")] [SerializeField] [MinValue(0)] 
    private float cameraTransitionTime = 2f;
    [Tooltip("Time for the movement axis to match this controller's.")] [SerializeField] [MinValue(0)] 
    private float movementTransitionTime = 1f;
    [Tooltip("Dictates what axis the player moves in. \n" +
        "Dynamic: Determines the axis automagically based on the camera's position\n" +
        "Fixed: Axis is set by the \"Fixed Direction Degrees\"")] [SerializeField] [OnValueChanged("AutoUpdateMovementAim")]
    private MovementDirection movementDirection = MovementDirection.Dynamic;
    [Tooltip("Determines which way is forward based on the degrees.")] [ShowIf("movementDirection", MovementDirection.Fixed)] [SerializeField] [OnValueChanged("AutoUpdateMovementAim")]
    private float fixedDirectionDegrees = 0;
    [Tooltip("List of camera controllers that won't be transitioned to if the current camera is active.")] [SerializeField] 
    private List<WorldCameraController> blacklistedControllers = new List<WorldCameraController>();


    [BoxGroup("Body")] [Tooltip("The Cinemachine body type for the camera. (Influences position.)")] [SerializeField] [OnValueChanged("AutoUpdateBody")]
    private Body bodyType = Body.Transposer;
    [BoxGroup("Body")] [Tooltip("Dampening for the Cinemachine Body")] [SerializeField] [Range(0, 20)] [HideIf("bodyType", Body.Fixed)] [OnValueChanged("AutoUpdateBody")]
    private float followDampening = 2;
    [BoxGroup("Body")] [Tooltip("CinemachineTransposer's position relative to the target")] [SerializeField] [ShowIf("bodyType", Body.Transposer)] [OnValueChanged("AutoUpdateBody")]
    private Vector3 transposerPosition = new Vector3(0, 3, -8);
    [BoxGroup("Body")] [Tooltip("Increase to reduce character movement jitter with the dolly at the cost of performance.")] [SerializeField] [ShowIf("bodyType", Body.TrackedDolly)] [MinValue(1)] [OnValueChanged("AutoUpdateBody")]
    private int movementPathResolution = 16;
    [BoxGroup("Body")] [Tooltip("Increase to reduce camera jitter with the dolly at the cost of performance." +
        "Note that follow dampening can also negative jitter, at the cost of the camera lagging behind.")] [SerializeField] [ShowIf("bodyType", Body.TrackedDolly)] [OnValueChanged("AutoUpdateBody")]
    private int cameraPathResolution = 8;
    [BoxGroup("Body")] [Tooltip("The world position of the camera when the Body is Fixed.")] [SerializeField] [ShowIf("bodyType", Body.Fixed)] [OnValueChanged("AutoUpdateBody")]
    private Vector3 fixedPosition = new Vector3(0, 3, -8);


    [BoxGroup("Aim")]
    [SerializeField] [Tooltip("The Cinemachine aim type for the camera. (Influences rotation.)")] [OnValueChanged("AutoUpdateMainAim")]
    private Aim aimType = Aim.Composer;
    [BoxGroup("Aim")] [Tooltip("How far ahead of the player the composer will rotate towards.")] [SerializeField] [Range(0, 1)] [ShowIf("aimType", Aim.Composer)] [OnValueChanged("AutoUpdateMainAim")]
    private float lookAheadDistance = 0.5f;
    [BoxGroup("Aim")] [Tooltip("Smooths the rotation when the camera looks ahead.")] [SerializeField] [Range(0, 30)] [ShowIf("aimType", Aim.Composer)] [OnValueChanged("AutoUpdateMainAim")]
    private float lookAheadSmoothing = 5f;
    [BoxGroup("Aim")] [Tooltip("Adds dampening to the camera rotating.")] [SerializeField] [Range(0, 20)] [ShowIf("aimType", Aim.Composer)] [OnValueChanged("AutoUpdateMainAim")]
    private float aimDampening = 2;
    [BoxGroup("Aim")] [Tooltip("Offset where the camera is looking at relative to the target.")] [SerializeField] [ShowIf("aimType", Aim.Composer)] [OnValueChanged("AutoUpdateMainAim")]
    private Vector3 aimOffset;
    [BoxGroup("Aim")] [Tooltip("The world euler angle rotation of the camera when the Aim is Fixed.")] [SerializeField] [ShowIf("aimType", Aim.Fixed)] [OnValueChanged("AutoUpdateMainAim")]
    private Vector3 fixedAimRotation = Vector3.right * 15f;


    [BoxGroup("Controls")]
    [Label("Auto-Update Cameras")] [Tooltip("Automatically updates the child virtual cameras when changing the script's parameters." +
        "Disable if you want to manually change the virtual cameras.")]
    [SerializeField] private bool autoUpdate = true;

    #endregion

    #region ======== [ PRIVATE PROPERTIES ] ========

    private static WorldCameraController _currentCamera = null;
    private static WorldCameraController _previousCamera = null;
    private static Player _player;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Returns whether the camera is active or not.
    /// </summary>
    public bool IsActive()
    {
        return _currentCamera == this;
    }


    /// <summary>
    /// Activates this camera and deactivates all other
    /// </summary>
    public void Activate()
    {
        if (IsActive()) return;

        if (_currentCamera && _currentCamera.blacklistedControllers.Contains(this)) return;

        GameManager.Player.Camera.m_DefaultBlend.m_Time = cameraTransitionTime;
        GameManager.Player.MoveCamera.m_DefaultBlend.m_Time = movementTransitionTime;

        _previousCamera = _currentCamera;
        if (_currentCamera != null) {
            _currentCamera.Disable();
        }
        _currentCamera = this;
        VirtualCamera.gameObject.SetActive(true);
        VirtualMovement.gameObject.SetActive(true);
    }


    /// <summary>
    /// Deactivates this camera and resort back to previous camera
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive()) return;
        if (_previousCamera == null) {
            Debug.LogWarning("No previous camera detected. Aborting Deactivation");
            return;
        }

        _previousCamera.Activate();
        _previousCamera = this;
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========
    private void Disable()
    {
        VirtualCamera.gameObject.SetActive(false);
        VirtualMovement.gameObject.SetActive(false);
    }

    void Awake()
    {
        if (!Application.isPlaying ) return;
        #if UNITY_EDITOR
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;
        #endif

        // Activates this Camera if ActiveOnAwake is True
        if (activeOnAwake)
        {
            // Warning if another camera is also ActiveOnAwake
            if (_currentCamera != null) {
                Debug.LogWarning($"There is another WorldCameraController " +
                                 $"({_currentCamera.gameObject} with \"Active On Awake\" enabled.");
                Disable();
                return;
            }
            _currentCamera = this;
        }
        else
        {
            Disable();
        }
    }

    private void AssignTargets()
    {
        // GameManager.Player may be null if the Editor is in Edit Mode
        // so we manually search if it is.
        _player = FindObjectOfType<Player>();

        if (_player == null) return;

        VirtualCamera.Follow = _player.Transform;
        VirtualMovement.Follow = _player.Transform;

        VirtualCamera.LookAt = _player.LookTarget;
        VirtualMovement.LookAt = _player.LookTarget;
    }

    [HideIf("autoUpdate"), Button("Manually Update Cameras")]
    private void UpdateAll()
    {
        UpdateFOV();
        UpdateBody();
        UpdateMainAim();
        RecUndo("UpdateAll");
    }

    private void AutoUpdateFOV() { if (autoUpdate) UpdateFOV(); RecUndo("UpdateFOV"); }
    private void AutoUpdateMainAim() { if (autoUpdate) UpdateMainAim(); RecUndo("UpdateMainAim"); }
    private void AutoUpdateMovementAim() { if (autoUpdate) UpdateMovementAim(); RecUndo("UpdateMovementAim"); }
    private void AutoUpdateBody() { if (autoUpdate) UpdateBody(); RecUndo("UpdateBody"); }
    private void AutoUpdateTrackedDolly() { if (autoUpdate) UpdateTrackedDolly(); RecUndo("UpdateTrackedDolly"); }
    private void AutoUpdateTransposer() { if (autoUpdate) UpdateTransposer(); RecUndo("UpdateTransposer"); }

    private void UpdateFOV()
    {
        VirtualCamera.m_Lens.FieldOfView = fieldOfView;
    }

    private void UpdateMainAim()
    {
        CinemachineComponentBase mainAim = VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim);
        if (aimType == Aim.Fixed) {
            if (mainAim != null) DestroyImmediate(mainAim);
            VirtualCamera.transform.eulerAngles = fixedAimRotation;
        } else if (aimType == Aim.Composer) {
            CinemachineComposer mainComposer =
                AddOrGetCinemachineComponent<CinemachineComposer>(VirtualCamera, CinemachineCore.Stage.Aim);
            mainComposer.m_HorizontalDamping = aimDampening;
            mainComposer.m_VerticalDamping = aimDampening;
            mainComposer.m_LookaheadTime = lookAheadDistance;
            mainComposer.m_LookaheadSmoothing = lookAheadSmoothing;
            mainComposer.m_TrackedObjectOffset = aimOffset;
            RecCompUndo(mainComposer, "Changed Main's Composer");
        }
        UpdateMovementAim();
    }

    private void UpdateMovementAim()
    {
        if (movementDirection == MovementDirection.Fixed) {
            // Destroy Aim Component if Exists
            DestroyImmediate(VirtualMovement.GetCinemachineComponent(CinemachineCore.Stage.Aim));
            VirtualMovement.transform.eulerAngles = Vector3.up * fixedDirectionDegrees;
            return;
        }

        // If aim components are different, need to change Movement Camera's aim
        CinemachineComponentBase moveAim = VirtualMovement.GetCinemachineComponent(CinemachineCore.Stage.Aim);

        if (aimType == Aim.Fixed) {
            if (moveAim != null) DestroyImmediate(moveAim);
            VirtualMovement.transform.rotation = VirtualCamera.transform.rotation;
        } else if (aimType == Aim.Composer) {
            CinemachineComposer moveComposer =
                AddOrGetCinemachineComponent<CinemachineComposer>(VirtualMovement, CinemachineCore.Stage.Aim);
            moveComposer.m_HorizontalDamping = 0;
            moveComposer.m_VerticalDamping = 0;
            moveComposer.m_LookaheadTime = 0;
            moveComposer.m_LookaheadSmoothing = 0;
            RecCompUndo(moveComposer, "Changed Move's Composer");
        }
    }

    private void UpdateBody()
    {
        switch (bodyType)
        {
            case Body.Fixed:
                DestroyImmediate(VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body));
                DestroyImmediate(VirtualMovement.GetCinemachineComponent(CinemachineCore.Stage.Body));
                VirtualCamera.transform.position = fixedPosition;
                VirtualMovement.transform.position = fixedPosition;
                break;
            case Body.TrackedDolly:
                UpdateTrackedDolly();
                break;
            case Body.Transposer:
                UpdateTransposer();
                break;
        }
    }

    private void UpdateTrackedDolly()
    {
        CinemachineTrackedDolly mainTD =
            AddOrGetCinemachineComponent<CinemachineTrackedDolly>(VirtualCamera, CinemachineCore.Stage.Body);
        CinemachineTrackedDolly moveTD =
            AddOrGetCinemachineComponent<CinemachineTrackedDolly>(VirtualMovement, CinemachineCore.Stage.Body);

        mainTD.m_XDamping = followDampening;
        mainTD.m_YDamping = followDampening;
        mainTD.m_ZDamping = followDampening;

        moveTD.m_XDamping = 0;
        moveTD.m_YDamping = 0;
        moveTD.m_ZDamping = 0;

        mainTD.m_Path = GetComponent<CinemachinePath>();
        if (mainTD.m_Path == null) {
            mainTD.m_Path = gameObject.AddComponent(typeof(CinemachinePath)) as CinemachinePath;
        }
            
        moveTD.m_Path = GetComponent<CinemachinePath>();

        mainTD.m_AutoDolly.m_Enabled = true;
        moveTD.m_AutoDolly.m_Enabled = true;

        mainTD.m_AutoDolly.m_SearchResolution = cameraPathResolution;
        moveTD.m_AutoDolly.m_SearchResolution = movementPathResolution;

        if (mainTD.m_Path.PathLength == 0) {
            GetComponent<CinemachinePath>().m_Waypoints = new CinemachinePath.Waypoint[1] {
                new CinemachinePath.Waypoint { position = transform.position, tangent = new Vector3(1, 0, 0) }
            };
        }

        RecCompUndo(mainTD, "Changed Main's Tracked Dolly");
        RecCompUndo(moveTD, "Changed Move's Tracked Dolly");
    }

    private void UpdateTransposer()
    {
        // Get Existing or Add a CinemachineTransposer
        var mainT = AddOrGetCinemachineComponent<CinemachineTransposer>(VirtualCamera, CinemachineCore.Stage.Body);
        var moveT = AddOrGetCinemachineComponent<CinemachineTransposer>(VirtualMovement, CinemachineCore.Stage.Body);

        // Apply Parameters for Movement and Camera
        mainT.m_FollowOffset = transposerPosition;
        moveT.m_FollowOffset = transposerPosition;

        mainT.m_XDamping = followDampening;
        mainT.m_YDamping = followDampening;
        mainT.m_ZDamping = followDampening;

        moveT.m_XDamping = 0;
        moveT.m_YDamping = 0;
        moveT.m_ZDamping = 0;

        RecCompUndo(mainT, "Updated Main's Transposer");
        RecCompUndo(moveT, "Updated Move's Transposer");
    }

    private ComponentType AddOrGetCinemachineComponent<ComponentType>(CinemachineVirtualCamera virtualCamera, 
                                                                      CinemachineCore.Stage stage)
        where ComponentType : CinemachineComponentBase
    {
        CinemachineComponentBase comp = virtualCamera.GetCinemachineComponent(stage);
        if (comp is not ComponentType) {
            DestroyImmediate(comp);
            return virtualCamera.AddCinemachineComponent<ComponentType>();
        } else {
            return comp as ComponentType;
        }
    }

    private void RecUndo(string message)
    {
        if (Application.isPlaying) return;
        #if UNITY_EDITOR
        Undo.RecordObject(VirtualCamera, message);
        Undo.RecordObject(VirtualMovement, message);
        EditorUtility.SetDirty(VirtualCamera);
        EditorUtility.SetDirty(VirtualMovement);
        #endif
    }

    private void RecCompUndo(CinemachineComponentBase comp, string message)
    {
        if (Application.isPlaying || comp == null) return;
        #if UNITY_EDITOR
        Undo.RecordObject(comp, message);
        EditorUtility.SetDirty(comp);
        #endif 
    }

    void Start()
    {
        // Check if in Prefab Mode
        #if UNITY_EDITOR
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;
        #endif

        AssignTargets();

        // Don't run if the game is not running
        if (!Application.isPlaying) return;

        // Error Catching
        if (_currentCamera == null) {
            Debug.LogError($"No WorldCameraControllers are active. " +
                $"Make sure one WorldCameraController has \"Active On Awake\" enabled.");
        }

        if (!GetComponent<Collider>() || !GetComponent<Collider>().isTrigger) {
            Debug.LogWarning("Make sure WorldCameraController has an " +
                "attached collider with \"Is Trigger\" Enabled");
        }

        if (VirtualCamera == null) {
            Debug.LogError("Virtual Camera (CinemachineVirtualCamera) has not been assigned.");
        }

        if (VirtualMovement == null) {
            Debug.LogError("Virtual Movement (CinemachineVirtualCamera) has not been assigned.");
        }

        _currentCamera.Activate();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Activate();
        }
    }

    #endregion
}
