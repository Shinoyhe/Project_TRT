using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

[ExecuteAlways]
public class ElevatorController : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    [Header("References")]
    [SerializeField] private Transform objectRoot;

    [Space, Header("Setting the Platform")]
    [SerializeField] private Transform startingWaypoint;

    private void OnValidate()
    {
        if (waypoints.Count > 0 && startingWaypoint != null)
        {
            SnapToWaypoint();
        }
    }

    [Button("Snap to Waypoint")]
    public void SnapToWaypoint()
    {
        if (objectRoot != null)
        {
            #if UNITY_EDITOR
            Undo.RecordObject(objectRoot, "Move Elevator Object");
            #endif

            objectRoot.position = startingWaypoint.position;

            #if UNITY_EDITOR
            EditorUtility.SetDirty(objectRoot);
            #endif

        }
    }
}
