using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Object Assignment")]
    [SerializeField] private PlayerInputHandler controls;
    private CharacterController characterController;


    [Header("Parameters")]
    [SerializeField] private float speed = 5f;


    private const float GRAVITY = 9.81f;
    private float _downwardForce = 0;
    private Vector3 _forwardVector = Vector3.forward;


    public void SetForwardVector(Vector3 direction)
    {
        direction.Scale(new Vector3(1, 0, 1));

        if (direction == Vector3.zero)
        {
            Debug.LogError("SetForwardDirection needs a Vector3 with non-zero x and z values.");
        }

        _forwardVector = direction;
        Debug.Log(direction);
    }




    void Awake()
    {
        Player.Movement = this;
        Player.Object = gameObject;
        Player.Transform = transform;
    }

    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    
    void Update()
    {
        Vector3 input = controls.GetMoveInput();

        Quaternion forwardRot = Quaternion.FromToRotation(Vector3.forward, _forwardVector);
        // transform.rotation = forwardRot;

        Vector3 direction = forwardRot * input;
        characterController.Move(direction * speed * Time.deltaTime);

        if (!characterController.isGrounded)
        {
            _downwardForce += GRAVITY * Time.deltaTime;
            characterController.Move(_downwardForce * Vector3.down * Time.deltaTime);
        }
        else
        {
            _downwardForce = 0;
        }

        // Debug.Log(_downwardForce * Time.deltaTime);
    }
}
