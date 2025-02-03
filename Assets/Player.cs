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
    public static PlayerMovement Movement {
        get
        {
            if (Movement == null) Instance.UpdateValues();
            return Movement; 
        }
        private set
        {
            Movement = value;
        }
    }
    public static GameObject Object
    {
        get
        {
            if (Object == null) Instance.UpdateValues();
            return Object;
        }
        private set
        {
            Object = value;
        }
    }
    public static Transform Transform
    {
        get
        {
            if (Transform == null) Instance.UpdateValues();
            return Transform;
        }
        private set
        {
            Transform = value;
        }
    }
    public static CinemachineBrain Camera
    {
        get
        {
            if (Camera == null) Instance.UpdateValues();
            return Camera;
        }
        private set
        {
            Camera = value;
        }
    }
    public static Transform PivotCamera
    {
        get
        {
            if (PivotCamera == null) Instance.UpdateValues();
            return PivotCamera;
        }
        private set
        {
            PivotCamera = value;
        }
    }

    [Header("Assignment")]
    [SerializeField] private PlayerMovement playerController;
    [SerializeField] private CinemachineBrain playerCamera;
    [SerializeField] private Transform pivotCamera;

    protected override void Awake()
    {
        base.Awake();
        UpdateValues();
    }

    private void UpdateValues()
    {
        Object = playerController.gameObject;
        Transform = playerController.transform;
        Camera = playerCamera;
        PivotCamera = pivotCamera;
    }
}
