using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Object Assignment")]
    [SerializeField] private PlayerInputHandler controls;
    private CharacterController characterController;
    
    private const float GRAVITY = 9.81f;
    private float _downwardForce = 0;


    [Header("Parameters")]
    [SerializeField] private float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = controls.GetMoveInput();
        Vector3 direction = transform.right * input.x + transform.forward * input.z;
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

        Debug.Log(_downwardForce * Time.deltaTime);
    }
}
