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
    public static PlayerMovement Movement;
    public static GameObject Object;
    public static Transform Transform;
    public static CinemachineBrain Camera;
    public static Transform PivotCamera;

    [Header("Assignment")]
    [SerializeField] private PlayerMovement playerController;
    [SerializeField] private CinemachineBrain playerCamera;
    [SerializeField] private Transform pivotCamera;

    protected override void Awake()
    {
        base.Awake();

        Object = playerController.gameObject;
        Transform = playerController.transform;
        Camera = playerCamera;
        PivotCamera = pivotCamera;
    }
}
