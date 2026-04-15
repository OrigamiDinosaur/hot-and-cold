using Apache.Core.Extensions;
using UnityEngine;

namespace Apache.Core {
	public class Follow : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("Position")]

		[SerializeField] protected TransformRef follow;
		[SerializeField] protected Vector3 positionOffset;
		[SerializeField] protected bool dontFollowY;
		
		[Header("Rotation")]

		[InspectorLabelBool]
		[SerializeField] protected bool shouldRotate;
		[SerializeField] protected Vector3 rotationOffset;

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		protected void Start() {

			// warn if we don't have a follow transform.
			if (follow.Val == null) {
				Debug.LogWarning($" { nameof(Follow) } component on game object \"" + gameObject.name + "\" does not have a follow target assigned. Is this intentional? " +
				                 $"If so, it may be best just to remove the { nameof(Follow) } component.",
				                 gameObject);
			}
		}

		protected void LateUpdate() {
			if (follow == null) return;

			// position at follow position, grabbing y for reinstatement if necessary.
			float prevY = transform.position.y;
			transform.position = follow.Val.position;

			// apply position offset locally.
			Vector3 localPosition = transform.localPosition;
			localPosition += positionOffset;
			transform.localPosition = localPosition;

			// if not following y, reinstate previous y.
			if (dontFollowY) {
				transform.position = transform.position.WithY(prevY);
			}

			// rotate and apply rotation offset if necessary.
			if (!shouldRotate) return;
			transform.rotation = follow.Val.rotation;
			transform.localRotation *= Quaternion.Euler(rotationOffset);
		}
	}
}