using UnityEngine;

public class InputHandler : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected PlayerController playerController;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Update() {

		// get our movement axis. 
		float horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
		float vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

		// if we don't have into, stop our movement. 
		if (horizontal == 0 && vertical == 0) {
			playerController.UpdateMovement(Vector3.zero);
		}

		// otherwise normalize our direction and and update the player controller.
		else {

			Vector3 movementDirection = (playerController.transform.forward * vertical) + (playerController.transform.right * horizontal);
			movementDirection.Normalize();

			playerController.UpdateMovement(movementDirection);
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			playerController.AttemptSearchOrDill();
		}
	}
}