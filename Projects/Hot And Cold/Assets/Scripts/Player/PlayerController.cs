using Apache.Core;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private static readonly int SPEED_PARAMETER = Animator.StringToHash("Speed");

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected CharacterController cc;
	[SerializeField] protected Animator animator; 
	[SerializeField] protected Transform robotRoot;

	[Header("Movement")]

	[SerializeField] protected float movementSpeed;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Update() {

		cc.Move(Vector3.down * 9.98f * Time.deltaTime);

		float horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
		float vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

		if (horizontal == 0 && vertical == 0) {

			animator.SetFloat(SPEED_PARAMETER, 0.0f);
			return; 
		}
		
		Vector3 movementDirection = (transform.forward * vertical) + (transform.right * horizontal);
		movementDirection.Normalize();
		
		animator.SetFloat(SPEED_PARAMETER, 1.0f); 

		UpdateMovement(movementDirection);
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void UpdateMovement(Vector3 movementDirection) {

		robotRoot.transform.forward = movementDirection;

		Vector3 movement = (movementDirection * movementSpeed) * Time.deltaTime;

		cc.Move(movement);
	}
}