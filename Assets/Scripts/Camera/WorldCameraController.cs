using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

using UnityEngine;
using UnityEditor;

using Cinemachine;
using NaughtyAttributes;


public class WorldCameraController : MonoBehaviour
{
    #region ======== [ OBJECT REFERENCES ] ========
    [Foldout("Object References")] public CinemachineVirtualCamera VirtualCamera;
    [Foldout("Object References")] public CinemachineVirtualCamera VirtualMovement;
    #endregion

    #region ======== [ ENUMS ] ========
    public enum MovementDirection
    {
        Fixed, Dynamic
    }

    public enum Body
    {
        None, Transposer, TrackedDolly
    }

    public enum Aim
    {
        None, Composer
    }
    #endregion

    #region ======== [ PARAMETERS ] ========
    [Header("Parameters")]
    [Tooltip("Will activate this controller on Awake. Should be true for one controller in the scene.")] [SerializeField] 
    private bool activeOnAwake = false;
    [Tooltip("The view camera's FOV.")] [SerializeField] [MinValue(1), MaxValue(179)] 
    private int fieldOfView = 60;
    [Tooltip("Time for the camera to move to this controller.")] [SerializeField] [MinValue(0)] 
    private float cameraTransitionTime = 2f;
    [Tooltip("Time for the movement axis to match this controller's.")] [SerializeField] [MinValue(0)] 
    private float movementTransitionTime = 1f;
    [Tooltip("Dictates what axis the player moves in. \n" +
        "Dynamic: Determines the axis automagically based on the camera's position\n" +
        "Fixed: Axis is set by the \"Fixed Direction Degrees\"")] [SerializeField] 
    private MovementDirection movementDirection = MovementDirection.Dynamic;
    [Tooltip("Determines which way is forward based on the degrees.")] [ShowIf("movementDirection", MovementDirection.Fixed)] [SerializeField] 
    private float fixedDirectionDegrees = 0;
    [Tooltip("List of camera controllers that won't be transitioned to if the current camera is active.")] [SerializeField] 
    private List<WorldCameraController> blacklistedControllers = new List<WorldCameraController>();


    [BoxGroup("Body")] [Tooltip("The Cinemachine body type for the camera")] [SerializeField]
    private Body bodyType = Body.Transposer;
    [BoxGroup("Body")] [SerializeField] [Range(0, 20)] [HideIf("bodyType", Body.None)]
    private float followDampening = 2;
    [BoxGroup("Body")] [SerializeField] [ShowIf("bodyType", Body.Transposer)]
    private Vector3 transposerPosition = new Vector3(0, 3, -8);
    [BoxGroup("Body")] [SerializeField] [ShowIf("bodyType", Body.TrackedDolly)] [MinValue(1)]
    private int movementPathResolution = 16;
    [BoxGroup("Body")] [SerializeField] [ShowIf("bodyType", Body.TrackedDolly)]
    private int cameraPathResolution = 8;


    [BoxGroup("Aim")]
    [SerializeField]
    private Aim aimType = Aim.Composer;
    [BoxGroup("Aim")] [SerializeField] [Range(0, 1)] [ShowIf("aimType", Aim.Composer)]
    private float lookAheadDistance = 0.5f;
    [BoxGroup("Aim")] [SerializeField] [Range(0, 30)] [ShowIf("aimType", Aim.Composer)]
    private float lookAheadSmoothing = 5f;
    [BoxGroup("Aim")] [SerializeField] [Range(0, 20)] [ShowIf("aimType", Aim.Composer)]
    private float aimDampening = 2;
    [BoxGroup("Aim")] [SerializeField] [ShowIf("aimType", Aim.Composer)]
    private Vector3 aimOffset;
    [BoxGroup("Aim")] [SerializeField] [ShowIf("aimType", Aim.None)]
    private Vector3 fixedAimRotation = Vector3.right * 15f;


    [BoxGroup("Controls")]
    [Label("Auto-Update Cameras")]
    [SerializeField] private bool autoUpdate = true;

    #endregion

    #region ======== [ PRIVATE PROPERTIES ] ========

    private static WorldCameraController _currentCamera = null;
    private static WorldCameraController _previousCamera = null;
    private bool _started = false;

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

        Player.Camera.m_DefaultBlend.m_Time = cameraTransitionTime;
        Player.MoveCamera.m_DefaultBlend.m_Time = movementTransitionTime;

        _previousCamera = _currentCamera;
        _currentCamera?.Disable();
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
        if (_previousCamera == null)
        {
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

#if UNITY_EDITOR
    /// <summary>
    /// If UpdatesVirtualCameras is true, this method changes the Virtual Camera parameters automagically :D
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]   // So that this doesn't appear in the autocomplete
    void OnValidate()
    {
        // VirtualCameras might be null on start up
        if (!autoUpdate) return;

        // Prevents errors from OnValidate running when a scene loads
        if (Application.isPlaying && !_started) return;

        UpdateVirtualCameras();
    }


    [HideIf("autoUpdate")] [Button("Manually Update Camera Values")]
    private void UpdateVirtualCameras()
    {
        VirtualCamera.m_Lens.FieldOfView = fieldOfView;

        // Handle Body Stages
        switch (bodyType)
        {
            case Body.None:
                HandleBodyNone();
                break;
            case Body.Transposer:
                HandleBodyTransposer();
                break;
            case Body.TrackedDolly:
                HandleBodyTrackedDolly();
                break;
        }

        // Handle Follower Types
        switch (aimType)
        {
            case Aim.None:
                HandleAimNone();
                break;
            case Aim.Composer:
                HandleAimComposer();
                break;
        }
    }


    private void HandleBodyNone()
    {
        if (VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) || VirtualMovement.GetCinemachineComponent(CinemachineCore.Stage.Body))
        {
            // Delay Call is Required to DestroyImmediates with OnValidate
            EditorApplication.delayCall += () =>
                DestroyImmediate(VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body));
        }
    }


    private void HandleBodyTransposer()
    {
        if (VirtualCamera.GetCinemachineComponent<CinemachineTransposer>() && VirtualMovement.GetCinemachineComponent<CinemachineTransposer>())
            UpdateBodyTransposerValues();
        else EditorApplication.delayCall += () => UpdateBodyTransposerValues();
    }


    private void HandleBodyTrackedDolly()
    {
        if (VirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>() && VirtualMovement.GetCinemachineComponent<CinemachineTrackedDolly>())
            UpdateBodyTrackedDollyValues();
        else EditorApplication.delayCall += () => UpdateBodyTrackedDollyValues();
    }

    
    private void HandleAimNone()
    {
        if (VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim))
        {
            EditorApplication.delayCall += () => DestroyImmediate(VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim));
        }

        VirtualCamera.transform.rotation = Quaternion.Euler(fixedAimRotation);
        VirtualMovement.transform.eulerAngles = movementDirection == MovementDirection.Fixed ? 
            Vector3.up * fixedDirectionDegrees : fixedAimRotation; // If movement direction is fixed, override it
    }


    private void HandleAimComposer()
    {
        if (VirtualCamera.GetCinemachineComponent<CinemachineComposer>() && VirtualMovement.GetCinemachineComponent<CinemachineComposer>())
            UpdateAimComposerValues();
        else EditorApplication.delayCall += () => UpdateAimComposerValues();
    }


    private void UpdateBodyTransposerValues()
    {
        if (VirtualCamera == null) return;

        // Get Existing or Add a CinemachineTransposer
        var mainFT = AddCinemachineComponent<CinemachineTransposer>(VirtualCamera, CinemachineCore.Stage.Body);
        var moveFT = AddCinemachineComponent<CinemachineTransposer>(VirtualMovement, CinemachineCore.Stage.Body);

        // Apply Parameters for Movement and Camera
        mainFT.m_FollowOffset = transposerPosition;
        moveFT.m_FollowOffset = transposerPosition;

        mainFT.m_XDamping = followDampening;
        mainFT.m_YDamping = followDampening;
        mainFT.m_ZDamping = followDampening;

        moveFT.m_XDamping = 0;
        moveFT.m_YDamping = 0;
        moveFT.m_ZDamping = 0;
    }


    private void UpdateBodyTrackedDollyValues()
    {
        if (VirtualCamera == null) return;

        // Get Existing or Add a CinemachineTransposer
        var mainTD = AddCinemachineComponent<CinemachineTrackedDolly>(VirtualCamera, CinemachineCore.Stage.Body);
        var moveTD = AddCinemachineComponent<CinemachineTrackedDolly>(VirtualMovement, CinemachineCore.Stage.Body);

        // Apply Parameters for Movement and Camera
        VirtualCamera.transform.rotation = Quaternion.Euler(fixedAimRotation);
        VirtualMovement.transform.eulerAngles = movementDirection == MovementDirection.Fixed ? 
            Vector3.up * fixedDirectionDegrees : fixedAimRotation; // If movement direction is fixed, override it

        mainTD.m_XDamping = followDampening;
        mainTD.m_YDamping = followDampening;
        mainTD.m_ZDamping = followDampening;

        moveTD.m_XDamping = 0;
        moveTD.m_YDamping = 0;
        moveTD.m_ZDamping = 0;

        mainTD.m_Path = GetComponent<CinemachinePath>() ??
            gameObject.AddComponent(typeof(CinemachinePath)) as CinemachinePath;
        moveTD.m_Path = GetComponent<CinemachinePath>();

        mainTD.m_AutoDolly.m_Enabled = true;
        moveTD.m_AutoDolly.m_Enabled = true;

        mainTD.m_AutoDolly.m_SearchResolution = cameraPathResolution;
        moveTD.m_AutoDolly.m_SearchResolution = movementPathResolution;

        if (mainTD.m_Path.PathLength == 0)
            GetComponent<CinemachinePath>().m_Waypoints = new CinemachinePath.Waypoint[1]
            {
                new CinemachinePath.Waypoint { position = transform.position, tangent = new Vector3(1, 0, 0) }
            };
    }


    private void UpdateAimComposerValues()
    {
        if (VirtualCamera == null) return;

        var mainC = AddCinemachineComponent<CinemachineComposer>(VirtualCamera, CinemachineCore.Stage.Aim);

        mainC.m_HorizontalDamping = aimDampening;
        mainC.m_VerticalDamping = aimDampening;

        mainC.m_LookaheadTime = lookAheadDistance;
        mainC.m_LookaheadSmoothing = lookAheadSmoothing;

        mainC.m_TrackedObjectOffset = aimOffset;

        if (movementDirection == MovementDirection.Dynamic)
        {
            var moveC = AddCinemachineComponent<CinemachineComposer>(VirtualMovement, CinemachineCore.Stage.Aim);
            moveC.m_HorizontalDamping = 0;
            moveC.m_VerticalDamping = 0;
            moveC.m_LookaheadTime = 0;
            moveC.m_LookaheadSmoothing = 0;
        }
        else
        {
            if (VirtualMovement.GetCinemachineComponent(CinemachineCore.Stage.Aim))
                EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(VirtualMovement.GetCinemachineComponent(CinemachineCore.Stage.Aim));
                };
            VirtualMovement.transform.eulerAngles = Vector3.up * fixedDirectionDegrees;
        }
    }


    private ComponentType AddCinemachineComponent<ComponentType>(CinemachineVirtualCamera virtualCamera, CinemachineCore.Stage stage)
        where ComponentType : CinemachineComponentBase
    {
        CinemachineComponentBase comp = virtualCamera.GetCinemachineComponent(stage);
        if (!(comp is ComponentType))
        {
            DestroyImmediate(comp); 
            return virtualCamera.AddCinemachineComponent<ComponentType>();
        }
        else
        {
            return comp as ComponentType;
        }
    }
#endif

    void Awake()
    {
        // Activates this Camera if ActiveOnAwake is True
        if (activeOnAwake)
        {
            // Warning if another camera is also ActiveOnAwake
            if (_currentCamera != null)
            {
                Debug.LogWarning($"There is another WorldCameraController " +
                    $"({_currentCamera.gameObject} with \"Active On Awake\" enabled.");
                Disable();
                return;
            }
            Activate();
        }
        else
        {
            Disable();
        }
    }


    void Start()
    {
        // Error Catching
        if (_currentCamera == null)
        {
            Debug.LogError($"No WorldCameraControllers are active. " +
                $"Make sure one WorldCameraController has \"Active On Awake\" enabled.");
        }

        if (!GetComponent<Collider>() || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("Make sure WorldCameraController has an " +
                "attached collider with \"Is Trigger\" Enabled");
        }

        if (VirtualCamera == null)
        {
            Debug.LogError("Virtual Camera (CinemachineVirtualCamera) has not been assigned.");
        }

        if (VirtualMovement == null)
        {
            Debug.LogError("Virtual Movement (CinemachineVirtualCamera) has not been assigned.");
        }

        _started = true;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Activate();
        }
    }

    #endregion
}
