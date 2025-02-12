using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCallButton : ItemInteractable
{
    [SerializeField] private ElevatorController controller;
    [SerializeField] private Transform waypoint;

    public override void Interaction()
    {
        base.Interaction();
        controller.MoveElevator(waypoint);
    }
}
