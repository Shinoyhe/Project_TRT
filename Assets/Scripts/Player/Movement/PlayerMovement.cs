using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	#region ======== [ OBJECT REFERENCES ] ========

	[Header("Object References")]
	[SerializeField] private Transform forwardTransform;
	[SerializeField] private Animator animator;
	private CharacterController _characterController;

	#endregion

	#region ======== [ PARAMETERS ] ========

	[Header("Parameters")]
	[SerializeField] private float speed = 5f;

	#endregion

	#region ======== [ PRIVATE PROPERTIES ] ========

	private const float _gravity = 9.81f;
	private float _downwardForce = 0;

	#endregion

	#region ======== [ PRIVATE METHODS ] ========

	void Start()
	{
		_characterController = GetComponent<CharacterController>();
	}

	void Update()
	{
		UpdateMovement();
		UpdateGravity();
	}

	private void UpdateMovement()
	{
		// Get Input
		Vector3 input = GameManager.PlayerInput.GetMoveInput();

		// Relative to Target
		float y = forwardTransform.rotation.eulerAngles.y;
		Quaternion targetRotation = Quaternion.Euler(new Vector3(0, y, 0));

		// Move character
		Vector3 direction = targetRotation * input;
		_characterController.Move(speed * Time.deltaTime * direction);
		
		animator.SetBool("IsWalking", (direction * speed).magnitude > 0);
	}

	private void UpdateGravity()
	{
		if (!_characterController.isGrounded) {
			_downwardForce += _gravity * Time.deltaTime;
			_characterController.Move(_downwardForce * Time.deltaTime * Vector3.down);
		} else {
			_downwardForce = 0;
		}
	}
	#endregion
}
