using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

[ExecuteAlways]
public class ElevatorController : MonoBehaviour
{

    #region ========== [ Private Fields ] ==========

    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    [Header("References")]
    [SerializeField] private Transform objectRoot;

    [Space, Header("Setting the Platform")]
    [SerializeField] private Transform startingWaypoint;

    #endregion

    void OnValidate()
    {
        if (waypoints.Count > 0 && startingWaypoint != null)
        {
            SnapObjectToWaypoint();
        }
    }

    #region ========== [ Public Methods ] ==========

    /// <summary>
    /// Snaps the position of Object in the elevator to the selected starting waypoint
    /// </summary>
    [SerializeField, Button("Snap to Waypoint")]
    public void SnapObjectToWaypoint()
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

    /// <summary>
    /// Snaps the position of the selected starting waypoint to the Object in the elevator
    /// </summary>
    [SerializeField, Button("Snap Waypoint to Object")]
    public void SnapWaypointToObject()
    {
        if (objectRoot != null)
        {
            #if UNITY_EDITOR
            Undo.RecordObject(objectRoot, "Move waypoint");
            #endif

            startingWaypoint.position = objectRoot.position;

            #if UNITY_EDITOR
            EditorUtility.SetDirty(objectRoot);
            #endif

        }
    }

    #endregion
}
