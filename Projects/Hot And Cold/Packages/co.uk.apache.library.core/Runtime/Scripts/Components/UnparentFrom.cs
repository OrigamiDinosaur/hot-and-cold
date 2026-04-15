using UnityEngine;

namespace Apache.Core {
	public class UnparentFrom : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const bool DEFAULT_TO_PARENT_LEVEL = false;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		[Tooltip("If selected, the object will be reparented to be at the same level as its parent. Otherwise it will be unparented entirely, at the root.")]
		[SerializeField] protected bool toParentLevel = DEFAULT_TO_PARENT_LEVEL;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			transform.parent = (toParentLevel) ? transform.parent.parent : null;
		}
	}
}