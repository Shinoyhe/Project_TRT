using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ElevatorObject : MonoBehaviour
{
    public ElevatorController controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField, Button("Snap Selected Waypoint to Object")]
    private void SnapWaypointToObject()
    {
        controller.SnapWaypointToObject();
    }
}
