using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorInteractable : ItemInteractable
{
    [SerializeField] private ElevatorController controller;

    public override void Interaction()
    {
        base.Interaction();
        controller.MoveElevator();
    }
}
