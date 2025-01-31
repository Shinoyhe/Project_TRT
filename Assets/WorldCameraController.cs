using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

using UnityEngine;
using UnityEditor;

using Cinemachine;


public class WorldCameraController : MonoBehaviour
{
    #region ========== [ OBJECT REFERENCES ] ==========
    [Header("Object References")]
    public CinemachineVirtualCamera VirtualCamera;
    public CinemachineVirtualCamera VirtualMovement;
    #endregion

    #region ========== [ ENUMS ] ==========
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

    #region ========== [ PARAMETERS ] ==========
    [Header("Parameters")]
    [SerializeField] private bool ActiveOnAwake = false;
    [SerializeField] private bool UpdatesVirtualCameras = true;
    [SerializeField] private MovementDirection movementDirection = MovementDirection.Dynamic;
    [SerializeField] private float fixedDirectionDegrees = 0;

    [Header("Follower")]
    [SerializeField]
    private Body bodyType = Body.Transposer;
    [SerializeField] [Range(0, 20)]
    private float followDampening = 2;

    [Header("Transposer Follower")]
    [SerializeField]
    private Vector3 transposePosition = new Vector3(0, 3, -8);


    [Header("Aimmer")]
    [SerializeField]
    private Aim aimType = Aim.Composer;
    [SerializeField] [Range(0, 1)]
    private float lookAheadDistance = 0.5f;
    [SerializeField] [Range(0, 30)]
    private float lookAheadSmoothing = 5f;
    [SerializeField] [Range(0, 20)]
    private float aimDampening = 2;
    [SerializeField]
    private Vector3 fixedAimRotation = Vector3.right * 15f;

    #endregion

    #region ========== [ PRIVATE PROPERTIES ] ==========

    private static WorldCameraController _currentCamera = null;
    // that's it lol

    #endregion

    #region ========== [ PUBLIC METHODS ] ==========

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

        _currentCamera?.Deactivate();
        _currentCamera = this;
        VirtualCamera.gameObject.SetActive(true);
        VirtualMovement.gameObject.SetActive(true);
    }

    #endregion

    #region ========== [ PRIVATE METHODS ] ==========
    private void Deactivate()
    {
        VirtualCamera.gameObject.SetActive(false);
        VirtualMovement.gameObject.SetActive(false);
    }


    /// <summary>
    /// If UpdatesVirtualCameras is true, this method changes the Virtual Camera parameters automagically :D
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]   // So that this doesn't appear in the autocomplete
    void OnValidate()
    {
        if (!UpdatesVirtualCameras) return;

        // Handle Body Stages
        switch (bodyType)
        {
            case Body.None:
                DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineTransposer>());
                DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>());
                break;
            case Body.Transposer:
                UpdateBodyTransposerValues();
                break;
            case Body.TrackedDolly:
                UpdateBodyTrackedDollyValues();
                break;
        }

        // Handle Follower Types
        switch (aimType)
        {
            case Aim.None:
                DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineComposer>());
                VirtualCamera.transform.rotation = Quaternion.Euler(fixedAimRotation);
                VirtualMovement.transform.rotation = Quaternion.Euler(fixedAimRotation);
                break;
            case Aim.Composer:
                UpdateComposerValues();
                break;
        }
    }


    private void UpdateBodyTransposerValues()
    {
        // Get Existing or Add a CinemachineTransposer
        var mainFT = AddCinemachineComponent<CinemachineTransposer>(VirtualCamera, CinemachineCore.Stage.Body);
        var moveFT = AddCinemachineComponent<CinemachineTransposer>(VirtualMovement, CinemachineCore.Stage.Body);

        // Apply Parameters for Movement and Camera
        mainFT.m_FollowOffset = transposePosition;
        moveFT.m_FollowOffset = transposePosition;

        mainFT.m_XDamping = followDampening;
        mainFT.m_YDamping = followDampening;
        mainFT.m_ZDamping = followDampening;

        moveFT.m_XDamping = 0;
        moveFT.m_YDamping = 0;
        moveFT.m_ZDamping = 0;
    }


    private void UpdateBodyTrackedDollyValues()
    {
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
    }


    private void UpdateComposerValues()
    {
        var mainC = AddCinemachineComponent<CinemachineComposer>(VirtualCamera, CinemachineCore.Stage.Aim);

        mainC.m_HorizontalDamping = aimDampening;
        mainC.m_VerticalDamping = aimDampening;

        mainC.m_LookaheadTime = lookAheadDistance;
        mainC.m_LookaheadSmoothing = lookAheadSmoothing;

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
            DestroyImmediate(VirtualMovement.GetCinemachineComponent<CinemachineComposer>());
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


    void Awake()
    {
        // Activates this Camera if ActiveOnAwake is True
        if (ActiveOnAwake)
        {
            // Warning if another camera is also ActiveOnAwake
            if (_currentCamera != null)
            {
                Debug.LogWarning($"There is another WorldCameraController " +
                    $"({_currentCamera.gameObject} with \"Active On Awake\" enabled.");
                Deactivate();
                return;
            }
            Activate();
        }
        else
        {
            Deactivate();
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
