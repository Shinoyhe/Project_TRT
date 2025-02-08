using UnityEngine;

using Cinemachine;
using NaughtyAttributes;

/// <summary>
/// "Singleton" that contain references to important player references for other scripts to access.
/// There's probably a better way to do this, but I'll wait for the review.
/// </summary>
//[ExecuteAlways]
public class Player : MonoBehaviour
{
    [Header("Controller References")]
    [Required] public PlayerMovement Movement;
    [Required] public GameObject Object;
    [Required] public Transform Transform;

    [Header("Camera References")]
    [Required] public CinemachineBrain Camera;
    [Required] public CinemachineBrain MoveCamera;
    [Required] public Transform MovePivot;
    [Required] public Transform LookTarget;
    
    private void Awake()
    {
        if (!GameManager.Player){
            GameManager.Instance.player = this;
            Debug.Log(GameManager.Instance.player);
        }
    }
}
