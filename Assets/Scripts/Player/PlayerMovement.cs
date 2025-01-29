using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Object Assignment")]
    [SerializeField] private PlayerInputHandler controls;
    [SerializeField] private Transform forwardTransform;
    private CharacterController characterController;


    [Header("Parameters")]
    [SerializeField] private float speed = 5f;


    private const float GRAVITY = 9.81f;
    private float _downwardForce = 0;


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

        Quaternion forwardRot = Quaternion.FromToRotation(Vector3.forward, forwardTransform.forward);
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
