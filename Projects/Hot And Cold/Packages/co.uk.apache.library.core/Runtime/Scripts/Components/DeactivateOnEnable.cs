using UnityEngine;

namespace Apache.Core {
	public class DeactivateOnEnable : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const float DEFAULT_DELAY = 1;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[ApacheSpace]

		[SerializeField] protected float delay = DEFAULT_DELAY;

		[InspectorLabelBool]
		[SerializeField] protected bool shouldIgnoreTimescale;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void OnEnable() {
			sequence.Do(delay, shouldIgnoreTimescale, Deactivate);
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		[ApacheButton]
		public void Deactivate() {
			gameObject.SetActive(false);
		}
	}
}