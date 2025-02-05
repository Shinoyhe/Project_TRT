using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Object References")]
    [SerializeField] private PlayerInputHandler controls;
    [SerializeField] private Transform forwardTransform;
    private CharacterController characterController;

    #endregion


    #region ======== [ PARAMETERS ] ========

    [Header("Parameters")]
    [SerializeField] private float speed = 5f;

    #endregion


    #region ======== [ PRIVATE PROPERTIES ] ========

    private const float GRAVITY = 9.81f;
    private float _downwardForce = 0;

    #endregion


    #region ======== [ PRIVATE METHODS ] ========

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    
    void Update()
    {
        UpdateMovement();
        UpdateGravity();
    }


    private void UpdateMovement()
    {
        // Get Input
        Vector3 input = controls.GetMoveInput();

        // Relative to Target
        float y = forwardTransform.rotation.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, y, 0));

        // Move character
        Vector3 direction = targetRotation * input;
        characterController.Move(direction * speed * Time.deltaTime);
    }


    private void UpdateGravity()
    {
        if (!characterController.isGrounded)
        {
            _downwardForce += GRAVITY * Time.deltaTime;
            characterController.Move(_downwardForce * Vector3.down * Time.deltaTime);
        }
        else
        {
            _downwardForce = 0;
        }
    }
    #endregion
}
