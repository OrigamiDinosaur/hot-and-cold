using UnityEngine;

namespace Apache.Core {
	public class TransformContainerSetter : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected TransformContainer container;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			container.Val = transform;
		}
	}
}