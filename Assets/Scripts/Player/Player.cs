using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

/// <summary>
/// "Singleton" that contain references to important player references for other scripts to access.
/// There's probably a better way to do this, but I'll wait for the review.
/// </summary>
public class Player : Singleton<Player>
{
    public static PlayerMovement Movement { get { return Instance.playerController; } }
    public static GameObject Object { get { return Instance.playerController.gameObject; } }
    public static Transform Transform { get { return Instance.playerController.transform; } }
    public static CinemachineBrain Camera { get { return Instance.playerCamera; } }
    public static Transform PivotCamera { get { return Instance.pivotCamera; } }
    public static CinemachineBrain MoveCamera { get { return Instance.pivotCamera.GetComponent<CinemachineBrain>(); } }
    // ;3c

    [Header("References")]
    [SerializeField] private PlayerMovement playerController;
    [SerializeField] private CinemachineBrain playerCamera;
    [SerializeField] private Transform pivotCamera;
}
