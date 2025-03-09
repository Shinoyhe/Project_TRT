using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    #region ========== [ PARAMETERS ] ==========

    [SerializeField, BoxGroup("Doors")] private HingeJoint leftDoor;
    [SerializeField, BoxGroup("Doors")] private HingeJoint rightDoor;

    [ReadOnly] public bool IsOpen = false;
    #endregion

    #region ========== [ PRIVATE PROPERTIES ] ==========

    private JointMotor _leftMotor;
    private JointMotor _rightMotor;

    #endregion

    #region ========== [ PUBLIC METHODS ] ==========

    public void OpenDoor()
    {
        leftDoor.useMotor = true;
        rightDoor.useMotor = true;

        IsOpen = true;

        LockedInteractable lockedInteractable = GetComponent<LockedInteractable>();
        if (lockedInteractable != null)
        {
            lockedInteractable.HideIcon = true;
        }
    }

    #endregion

    #region ========== [ PRIVATE METHODS ] ==========

    // Start is called before the first frame update
    void Start()
    {
        _leftMotor = leftDoor.motor;
        _rightMotor = rightDoor.motor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion
}
