using UnityEngine;

namespace Apache.Core {
	public class ChildTo : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected Transform childTo;
		[SerializeField] protected bool worldPositionStays = true;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			childTo.SetParent(transform, worldPositionStays);
		}
	}
}