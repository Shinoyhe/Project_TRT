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

    // Enums
    public enum MovementOrientation
    {
        Fixed, TowardsCamera
    }

    public enum Follower
    {
        None, FramingTransposer, TrackedDolly
    }


    // Parameters
    [Header("Parameters")]
    [SerializeField] private bool ActiveOnAwake = false;
    [SerializeField] private MovementOrientation playerMovementOrientation;
    [SerializeField] private Vector3 forwardVector;

    [Header("Follower")]
    [SerializeField] [OnChangedCall("OnValueChanged")] 
    private Follower followerType = Follower.FramingTransposer;
    [SerializeField] [OnChangedCall("OnValueChanged")] [Range(0, 1)]
    private float lookAheadDistance = 0.5f;
    [SerializeField] [OnChangedCall("OnValueChanged")] [Range(0, 30)]
    private float lookAheadSmoothing = 5f;
    [SerializeField] [OnChangedCall("OnValueChanged")] [Range(0, 30)]
    private Vector3 dampening = Vector3.zero;

    [Header("Transposer Follower")]
    [SerializeField] [OnChangedCall("OnValueChanged")]
    private float transposerHeightOffset = 1.25f;
    [SerializeField] [OnChangedCall("OnValueChanged")]
    private float transposerDistance = 5f;
    [SerializeField] [OnChangedCall("OnValueChanged")]
    private Vector3 transposerRotation = Vector3.right * 15f;



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
    }


    private void Deactivate()
    {
        VirtualCamera.gameObject.SetActive(false);
    }


    private void UpdatePlayerForwardVector()
    {
        if (!IsActive()) return;

        Player.Movement.SetForwardVector(
            playerMovementOrientation switch
            {
                MovementOrientation.Fixed => forwardVector,
                MovementOrientation.TowardsCamera => GetTowardsCameraOrientation(),
                _ => GetTowardsCameraOrientation()
            });
    }


    private void Fuckmyass()
    {
        Fuckmyass();
    }

    private Vector3 GetTowardsCameraOrientation()
    {
        return (Player.Transform.position - Camera.main.transform.position).normalized;
    }


    /// <summary>
    /// Changes the Virtual Camera parameters automagically :D
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnValueChanged()
    {
        // Handle Follower Types
        switch (followerType)
        {
            case Follower.None:
                DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>());
                DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>());
                break;
            case Follower.FramingTransposer:
                UpdateFollowerTransposerValues();
                break;
            case Follower.TrackedDolly:
                break;
        }
    }


    private void UpdateFollowerTransposerValues()
    {
        DestroyImmediate(VirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>());
        var framingTransposer = VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>()
            ?? VirtualCamera.AddCinemachineComponent<CinemachineFramingTransposer>();

        framingTransposer.m_TrackedObjectOffset = Vector3.up * transposerHeightOffset;
        framingTransposer.m_CameraDistance = transposerDistance;
        VirtualCamera.transform.rotation = Quaternion.Euler(transposerRotation);

        framingTransposer.m_LookaheadTime = lookAheadDistance;
        framingTransposer.m_LookaheadSmoothing = lookAheadSmoothing;

        framingTransposer.m_XDamping = dampening.x;
        framingTransposer.m_YDamping = dampening.y;
        framingTransposer.m_ZDamping = dampening.z;
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
        UpdatePlayerForwardVector();
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