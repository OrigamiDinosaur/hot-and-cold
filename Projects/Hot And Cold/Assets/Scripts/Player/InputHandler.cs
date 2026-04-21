using Apache.Core;
using UnityEngine;

public class InputHandler : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected PlayerController playerController;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Update() {

		float horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
		float vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

		if (horizontal == 0 && vertical == 0) {
			playerController.UpdateMovement(Vector3.zero);
		}
		else {

			Vector3 movementDirection = (playerController.transform.forward * vertical) + (playerController.transform.right * horizontal);
			movementDirection.Normalize();

			playerController.UpdateMovement(movementDirection);
		}

		if (Input.GetKeyDown(KeyCode.Space)) {

			playerController.Search();
		}
	}
}