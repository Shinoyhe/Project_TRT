using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;
using DG.Tweening;

[ExecuteAlways]
public class ElevatorController : MonoBehaviour
{

    #region ========== [ PARAMETERS ] ==========

    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    [Header("References")]
    [SerializeField] private Transform objectRoot;

    [BoxGroup("Setup"), SerializeField] private Transform startingWaypoint;
    [BoxGroup("Setup"), SerializeField, Range(0f, 10f)] private float movementDurationSeconds;
    #endregion

    #region ========== [ PRIVATE PROPERTIES ] ==========

    private int startingWaypointIndex = -1;
    private int nextIndexModifier = 1;

    #endregion

    #region ========== [ PUBLIC METHODS ] ==========

    /// <summary>
    /// Snaps the position of Object in the elevator to the selected starting waypoint
    /// </summary>
    [SerializeField, Button("Snap Object to Waypoint")]
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

    #region ========== [ PRIVATE METHODS ] ==========

    private void SetStartingWaypointIndex()
    {
        if (!startingWaypoint) Debug.LogError("Elevator: Starting Waypoint not Assigned");

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == startingWaypoint)
            {
                startingWaypointIndex = i;
                break;
            }
        }
        if (startingWaypointIndex == -1)
        {
            Debug.LogError("Elevator: Could not get Starting Waypoint Index, waypoint is not in Waypoints list.");
        }
    }

    [SerializeField, Button("Move Elevator")]
    private void MoveElevator()
    {
        if ((startingWaypointIndex == waypoints.Count - 1 && nextIndexModifier > 0) || 
            (startingWaypointIndex == 0 && nextIndexModifier < 0)) {
            nextIndexModifier *= -1;
        }
        int targetWaypointIndex = startingWaypointIndex + nextIndexModifier;
        Transform targetWaypoint = waypoints[targetWaypointIndex];
    
        objectRoot.transform.DOMove(targetWaypoint.position, movementDurationSeconds).SetEase(Ease.InOutQuad);
        
        startingWaypoint = targetWaypoint;
        SetStartingWaypointIndex();
    }
    
    void OnValidate()
    {
        if (waypoints.Count > 0 && startingWaypoint != null)
        {
            SnapObjectToWaypoint();
        }
    }

    void Start()
    {
        SetStartingWaypointIndex();
    }

    void Update()
    {
        
    }
    #endregion
}
