using UnityEngine;

namespace Apache.Core {
	public class LookAt : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const float DEFAULT_ROTATION_LIMIT = 360;

		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		public enum LookAxes {
			Z,
			ZInverted,
			Y,
			YInverted,
			X,
			XInverted
		}

		public enum UpModes {
			World,
			Object
		}

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("Look")]

		[SerializeField] protected TransformRef lookAt;
		[InspectorLabel("Axis")]
		[SerializeField] protected LookAxes lookAxis;
		[Range(0, 1)]
		[SerializeField] protected float magnitude = 1;
		[Range(0, 360)]
		[SerializeField] protected float rotationLimit = DEFAULT_ROTATION_LIMIT;

		[Header("Up")]

		[SerializeField] protected UpModes upMode;
		[InspectorLabelBool]
		[SerializeField] protected bool shouldRoll;

		[Header("Options")]

		[InspectorLabelBool]
		[SerializeField] protected bool shouldAddInitialOffset;

		[InspectorLabelBool]
		[SerializeField] protected bool shouldSupportAnimation;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private Quaternion defaultLocalRotation;
		private Vector3 defaultForward;

		private Quaternion initialGlobalOffset;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public float Magnitude {
			get => magnitude;
			set => magnitude = value;
		}

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			defaultLocalRotation = transform.localRotation;
			defaultForward       = transform.forward;
		}

		protected void Start() {

			// N.B. we do this in start because the transform ref may not be set in awake.

			// work out the initial offset by seeing the difference between how we start and where we would end up.
			Quaternion defaultRotation = transform.rotation;
			UpdateLookAt(true);
			Quaternion newRotation = transform.rotation;
			initialGlobalOffset = Quaternion.Inverse(newRotation) * defaultRotation;

			// we have the offset so reinstate original rotation.
			transform.rotation = defaultRotation;
		}

		protected void LateUpdate() {
			UpdateLookAt();
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void UpdateLookAt(bool isDeterminingInitialOffset = false) {

			// grab previous local rotation, which may have been edited by animation just after Update but before LateUpdate.
			Quaternion prevLocalRotation = transform.localRotation;
			Vector3 prevForward = transform.forward;

			// work out look vector and relative up.
			Vector3 look = (lookAt.Val.position - transform.position);
			Vector3 up = (shouldRoll) ? lookAt.Val.up : Vector3.up;
			
			// rotate object along look rotation.
			transform.rotation = Quaternion.LookRotation(look, up);

			// handle reorientating to maintain object default up.
			if (upMode == UpModes.Object) {
				transform.localRotation *= defaultLocalRotation;
			}

			// work out the base local rotation we slerp from to support magnitude, and, if we support animation, use the new animation rotation.
			Quaternion baseLocalRotation = defaultLocalRotation;
			Vector3    baseForward       = defaultForward;
			if (shouldSupportAnimation) {
				baseLocalRotation = prevLocalRotation;
				baseForward       = prevForward;
			}

			// if we have a rotation limit, reinstate base rotation if we go over the limit and back out.
			if (rotationLimit < 360) {

				// work out look rotations for base rotation and new rotation, sharing an up of world up, eliminating all roll in the comparison.
				Quaternion baseWorldLookRotation = Quaternion.LookRotation(baseForward,       Vector3.up);
				Quaternion newWorldLookRotation  = Quaternion.LookRotation(transform.forward, Vector3.up);

				// if the angle between the look rotations exceeds the limit, reinstate base local rotation.
				// N.B. we divide by two because it's useful for the inspector to expose in terms of 360, but angle doesn't discriminate between rotation
				// direction and therefore reports the angle between 0 and 180.
				if (Quaternion.Angle(baseWorldLookRotation, newWorldLookRotation) > (rotationLimit / 2)) {
					transform.localRotation = baseLocalRotation;

					// before we return, apply initial offset if necessary.
					if (shouldAddInitialOffset) {
						transform.rotation *= initialGlobalOffset;
					}

					return;
				}
			}

			// rotate locally based on look axis.
			switch (lookAxis) {
				case LookAxes.ZInverted:
					transform.localRotation *= Quaternion.AngleAxis(180, Vector3.up);
					break;
				case LookAxes.X:
				case LookAxes.XInverted:

					// N.B. for left/right, we rotate right to point left along look vector, so the multipliers are inverted from what might be expected.
					transform.localRotation *= Quaternion.AngleAxis(90 * ((lookAxis == LookAxes.XInverted) ? 1 : -1), Vector3.up);
					break;
				case LookAxes.Y:
				case LookAxes.YInverted:
					transform.localRotation *= Quaternion.AngleAxis(90 * ((lookAxis == LookAxes.Y) ? 1 : -1), Vector3.right);
					break;
			}

			// slerp from default to current based on magnitude.
			transform.localRotation = Quaternion.Slerp(baseLocalRotation, transform.localRotation, magnitude);

			// finally, apply initial offset globally, if we're doing so and we're not using this method to actually determine the intial offset.
			if (shouldAddInitialOffset && !isDeterminingInitialOffset) {
				transform.rotation *= initialGlobalOffset;
			}
		}
	}
}