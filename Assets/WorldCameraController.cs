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
    // Object References
    [Header("Object References")]
    public CinemachineVirtualCamera VirtualCamera;
    public CinemachineVirtualCamera VirtualMovement;

    // Enums
    public enum MovementOrientation
    {
        Fixed, TowardsCamera
    }

    public enum Follower
    {
        None, Transposer, TrackedDolly
    }

    public enum Aimmer
    {
        None, Composer
    }


    // Parameters
    [Header("Parameters")]
    [SerializeField] private bool ActiveOnAwake = false;
    [SerializeField] private bool UpdatesVirtualCameras = true;
    [SerializeField] private MovementOrientation playerMovementOrientation;
    [SerializeField] private Vector3 forwardVector;

    [Header("Follower")]
    [SerializeField]
    private Follower followerType = Follower.Transposer;
    [SerializeField] [Range(0, 20)]
    private float followDampening = 2;

    [Header("Transposer Follower")]
    [SerializeField]
    private Vector3 transposePosition = new Vector3(0, 3, -8);
    [SerializeField]
    private Vector3 transposerRotation = Vector3.right * 15f;


    [Header("Aimmer")]
    [SerializeField]
    private Aimmer aimmerType = Aimmer.Composer;
    [SerializeField] [Range(0, 1)]
    private float lookAheadDistance = 0.5f;
    [SerializeField] [Range(0, 30)]
    private float lookAheadSmoothing = 5f;
    [SerializeField] [Range(0, 20)]
    private float aimDampening = 2;



    // Private Properties
    private static WorldCameraController _currentCamera = null;

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


    private void Deactivate()
    {
        VirtualCamera.gameObject.SetActive(false);
        VirtualMovement.gameObject.SetActive(false);
    }


    private void FuckTheCamera()
    {
        FuckTheCamera();
    }


    /// <summary>
    /// Changes the Virtual Camera parameters automagically :D
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    void OnValidate()
    {
        if (!UpdatesVirtualCameras) return;

        // Handle Follower Types
        switch (followerType)
        {
            case Follower.None:
                DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineTransposer>());
                DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>());
                break;
            case Follower.Transposer:
                UpdateFollowerTransposerValues();
                break;
            case Follower.TrackedDolly:
                break;
        }

        switch (aimmerType)
        {
            case Aimmer.None:
                DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineComposer>());
                break;
            case Aimmer.Composer:
                UpdateComposerValues();
                break;
        }
    }


    private void UpdateFollowerTransposerValues()
    {
        DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>());
        var mainFT = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>()
            ?? VirtualCamera.AddCinemachineComponent<CinemachineTransposer>();

        DestroyImmediate(VirtualMovement.GetCinemachineComponent<CinemachineTrackedDolly>());
        var moveFT = VirtualMovement.GetCinemachineComponent<CinemachineTransposer>()
            ?? VirtualMovement.AddCinemachineComponent<CinemachineTransposer>();


        // Apply Parameters for Movement and Camera
        mainFT.m_FollowOffset = transposePosition;
        moveFT.m_FollowOffset = transposePosition;
        VirtualCamera.transform.rotation = Quaternion.Euler(transposerRotation);
        VirtualMovement.transform.rotation = Quaternion.Euler(transposerRotation);

        mainFT.m_XDamping = followDampening;
        mainFT.m_YDamping = followDampening;
        mainFT.m_ZDamping = followDampening;

        moveFT.m_XDamping = 0;
        moveFT.m_YDamping = 0;
        moveFT.m_ZDamping = 0;
    }


    private void UpdateComposerValues()
    {
        var mainC = VirtualCamera.GetCinemachineComponent<CinemachineComposer>()
            ?? VirtualCamera.AddCinemachineComponent<CinemachineComposer>();

        mainC.m_HorizontalDamping = aimDampening;
        mainC.m_VerticalDamping = aimDampening;

        mainC.m_LookaheadTime = lookAheadDistance;
        mainC.m_LookaheadSmoothing = lookAheadSmoothing;
    }


    void Awake()
    {
        // Activates this Camera if ActiveOnAwake is True
        if (ActiveOnAwake)
        {
            if (_currentCamera != null)
            {
                Debug.LogWarning($"There is another WorldCameraController " +
                    $"({_currentCamera.gameObject} with \"Active On Awake\" enabled.");
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
            Debug.LogWarning($"No cameras are active. " +
                $"Make sure one WorldCameraController has \"Active On Awake\" enabled.");
        }

        if (!GetComponent<Collider>() || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("Make sure WorldCameraController has an " +
                "attached collider with \"Is Trigger\" Enabled");
        }
    }


    void Update()
    {

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Activate();
        }
    }
}


// From this https://stackoverflow.com/questions/63778384/unity-how-to-update-an-object-when-a-serialized-field-is-changed#63778437

/// <summary>
/// Calls a function when the value of the variable changes.
/// </summary>
public class OnChangedCallAttribute : PropertyAttribute
{
    public string methodName;
    public OnChangedCallAttribute(string methodNameNoArguments)
    {
        methodName = methodNameNoArguments;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(OnChangedCallAttribute))]
public class OnChangedCallAttributePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property, label);
        if (EditorGUI.EndChangeCheck())
        {
            OnChangedCallAttribute at = attribute as OnChangedCallAttribute;
            MethodInfo method = property.serializedObject.targetObject.GetType().GetMethods().Where(m => m.Name == at.methodName).First();

            if (method != null && method.GetParameters().Count() == 0)// Only instantiate methods with 0 parameters
                method.Invoke(property.serializedObject.targetObject, null);
        }
    }
}

#endif
