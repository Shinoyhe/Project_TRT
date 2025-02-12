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

    [BoxGroup("Setup"), SerializeField] private InventoryCardData requiredCard;
    [BoxGroup("Setup"), SerializeField] private Transform startingWaypoint;
    [BoxGroup("Setup"), SerializeField, Range(0f, 10f)] private float movementDurationSeconds;
    #endregion

    #region ========== [ PRIVATE PROPERTIES ] ==========

    private int startingWaypointIndex = -1;
    private int nextIndexModifier = 1;
    private int currentWaypointIndex = 0;
    private bool moving = false;

    #endregion

    #region ========== [ PUBLIC METHODS ] ==========

    /// <summary>
    /// Moves the elevator along the Waypoints in order, reverses when it reaches the end.
    /// </summary>
    [Button("Move Elevator")]
    public void MoveElevator()
    {
        if (requiredCard != null && !GameManager.Inventory.HasCard(requiredCard)) return;
        if (moving) { return; }

        moving = true;

        if ((startingWaypointIndex == waypoints.Count - 1 && nextIndexModifier > 0) ||
            (startingWaypointIndex == 0 && nextIndexModifier < 0))
        {
            nextIndexModifier *= -1;
        }
        int targetWaypointIndex = startingWaypointIndex + nextIndexModifier;
        Transform targetWaypoint = waypoints[targetWaypointIndex];

        objectRoot.transform.DOMove(targetWaypoint.position, movementDurationSeconds)
            .SetEase(Ease.InOutQuad).OnComplete(() => { moving = false; });

        startingWaypoint = targetWaypoint;
        SetStartingWaypointIndex();
    }


    public void MoveElevator(Transform targetWaypoint)
    {
        if (requiredCard != null && !GameManager.Inventory.HasCard(requiredCard)) return;
        if (moving) return;

        moving = true;
        int targetIndex = GetWaypointIndex(targetWaypoint);

        if (startingWaypointIndex == targetIndex) return;

        // Set nextIndexModifier so that we are moving towards the target
        if ((startingWaypointIndex > targetIndex && nextIndexModifier > 0) ||
            (startingWaypointIndex < targetIndex && nextIndexModifier < 0))
        {
            nextIndexModifier *= -1;
        }

        // Moving from start to end: InOutQuad, duration is movementDurationSeconds
        if (startingWaypointIndex == currentWaypointIndex && NextWaypointIs(targetWaypoint)) {
            objectRoot.transform.DOMove(targetWaypoint.position, movementDurationSeconds)
                .SetEase(Ease.InOutQuad).OnComplete(() => { moving = false; });
            startingWaypoint = targetWaypoint;
            SetStartingWaypointIndex();
            return;
        }

        // Moving from start to in-between: InQuad, duration is movementDurationSeconds
        if (startingWaypointIndex == currentWaypointIndex && !NextWaypointIs(targetWaypoint))
        {
            Transform nextWaypoint = waypoints[currentWaypointIndex + nextIndexModifier];

            objectRoot.transform.DOMove(nextWaypoint.position, movementDurationSeconds)
                .SetEase(Ease.InQuad).OnComplete(() => { moving = false; MoveElevator(targetWaypoint); });
            currentWaypointIndex += nextIndexModifier;
            return;
        }

        // Moving from in-between to in-between: no ease, duration is a bit shorter
        if (startingWaypointIndex != currentWaypointIndex && !NextWaypointIs(targetWaypoint))
        {
            Transform nextWaypoint = waypoints[currentWaypointIndex + nextIndexModifier];

            objectRoot.transform.DOMove(nextWaypoint.position, movementDurationSeconds * .75f)
                .SetEase(Ease.Linear).OnComplete(() => { moving = false; MoveElevator(targetWaypoint); });
            currentWaypointIndex += nextIndexModifier;
            return;
        }

        // Moving from in-between to end: OutQuad, duration is movementDurationSeconds
        if (startingWaypointIndex != currentWaypointIndex && NextWaypointIs(targetWaypoint))
        {
            objectRoot.transform.DOMove(targetWaypoint.position, movementDurationSeconds)
                .SetEase(Ease.OutQuad).OnComplete(() => { moving = false; });
            startingWaypoint = targetWaypoint;
            SetStartingWaypointIndex();
            return;
        }
    }

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

    private int GetWaypointIndex(Transform targetWaypoint)
    {
        int returnIndex = -1;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == targetWaypoint)
            {
                returnIndex = i;
                break;
            }
        }
        if (returnIndex == -1)
        {
            Debug.LogError("Elevator: Could not get Waypoint Index, waypoint is not in Waypoints list.");
        }

        return returnIndex;
    }

    private bool NextWaypointIs(Transform waypoint)
    {
        int tempWaypointIndex = currentWaypointIndex + nextIndexModifier;
        if ((tempWaypointIndex >= waypoints.Count) ||
            (tempWaypointIndex < 0))
        {
            nextIndexModifier *= -1;
            tempWaypointIndex = currentWaypointIndex + nextIndexModifier;
        }

        return waypoints[tempWaypointIndex] == waypoint;
    }

    private void SetStartingWaypointIndex()
    {
        if (!startingWaypoint) Debug.LogError("Elevator: Starting Waypoint not Assigned");

        startingWaypointIndex = GetWaypointIndex(startingWaypoint);
        currentWaypointIndex = startingWaypointIndex;
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
