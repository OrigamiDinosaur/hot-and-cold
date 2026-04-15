using UnityEngine;

namespace Apache.Core {
	public class AnimationStateOnAwake : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[ComponentRef, SerializeField] protected Animator animator;

		[SerializeField] protected AnimatorStateEnumeration initialState;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			animator.Play(initialState.Hash, initialState.LayerIndex, 0);
		}
	}
}