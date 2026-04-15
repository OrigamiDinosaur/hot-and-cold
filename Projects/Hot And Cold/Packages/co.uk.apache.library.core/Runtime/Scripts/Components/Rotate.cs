using UnityEngine;

namespace Apache.Core {
	public class Rotate : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private static readonly Vector3 DEFAULT_AXIS = Vector3.up;

		private const float DEFAULT_SECONDS_TO_360 = 5;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected Vector3 axis = DEFAULT_AXIS;

		[SerializeField] protected float secondsTo360 = DEFAULT_SECONDS_TO_360;

		[SerializeField] protected bool invertRotation;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected virtual void Update() {

			// perform rotation about the given axis at the appropriate speed.
			float degreesToRotate = 360f / (secondsTo360 / Time.deltaTime);
			transform.Rotate(axis, degreesToRotate * ((invertRotation) ? -1 : 1));
		}
	}
}