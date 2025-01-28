using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    void Awake()
    {
        Player.Camera = GetComponent<CinemachineBrain>();
    }
}
