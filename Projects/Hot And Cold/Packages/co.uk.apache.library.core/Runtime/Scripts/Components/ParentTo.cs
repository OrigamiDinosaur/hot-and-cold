using UnityEngine;

namespace Apache.Core {
	public class ParentTo : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected Transform parentTo;
		[SerializeField] protected bool worldPositionStays = true;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {

			// warn if the parent is null.
			if (parentTo == null) {
				Debug.LogWarning("The \"Parent To\" transform on \"" + gameObject.name + "\" is null, which will resolve to unparenting to the root. Was this intended? " +
				                 "If so, perhaps an UnparentFrom component is a better choice.", gameObject);
			}

			transform.SetParent(parentTo, worldPositionStays);
		}
	}
}