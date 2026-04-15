using System;
using Apache.Core.Extensions;
using UnityEngine;

namespace Apache.Core {
	public class ApacheComponent : MonoBehaviour {

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------
		
		private ApacheSequence _sequence;

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		protected virtual ApacheSequence sequence => _sequence ?? (_sequence = new ApacheSequence(SequenceMonoBehaviour));

		/// <summary>
		/// The <c>MonoBehaviour</c> that our sequence runs on. Overriding this allows us to supply an alternative sequence <c>MonoBehaviour</c>.
		/// </summary>
		protected virtual MonoBehaviour SequenceMonoBehaviour => this;

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Invokes an action.
		/// </summary>
		/// <param name="action">The action to invoke.</param>
		public void InvokeAction(Action action) {
			action?.Invoke();
		}

		/// <summary>
		/// Invokes an action after a delay.
		/// </summary>
		/// <param name="delay">The delay before invoking the action.</param>
		/// <param name="action">The action to invoke.</param>
		public void InvokeAction(float delay, Action action) {
			GlobalSequence.Do(delay, action);
		}

		/// <summary>
		/// Invokes an action after a realtime delay uninfluenced by time scale.
		/// </summary>
		/// <param name="delay">The realtime delay before invoking the action.</param>
		/// <param name="action">The action to invoke.</param>
		public void InvokeActionRealtime(float delay, Action action) {
			GlobalSequence.DoRealtime(delay, action);
		}

		/// <summary>
		/// Invokes an action on the next frame.
		/// </summary>
		/// <param name="action">The action to be performed on the next frame.</param>
		public void InvokeActionNextFrame(Action action) {
			GlobalSequence.NextFrame(action);
		}

		/// <summary>
		/// Invokes an action after a given number of frames.
		/// </summary>
		/// <param name="numFrames">The number of frames after the present frame in which to execute the action.</param>
		/// <param name="action">The action to be performed in the determined frame.</param>
		public void InvokeActionAfterFrames(int numFrames, Action action) {
			GlobalSequence.AfterFrames(numFrames, action);
		}

		/// <summary>
		/// Invokes an action at the end of the current frame.
		/// </summary>
		/// <param name="action">The action to be performed in the determined frame.</param>
		public void InvokeActionEndOfFrame(Action action) {
			GlobalSequence.EndOfFrame(action);
		}

		/// <summary>
		/// Invokes an action on the next fixed update.
		/// </summary>
		/// <param name="action">The action to be performed on the next fixed update.</param>
		public void InvokeActionFixedUpdate(Action action) {
			GlobalSequence.FixedUpdate(action);
		}

		/// <summary>Deactivates and destroys our own game object.</summary>
		public void DeactivateAndDestroy() {
			gameObject.DeactivateAndDestroy();
		}

		//-----------------------------------------------------------------------------------------
		// Editor Methods:
		//-----------------------------------------------------------------------------------------

#if UNITY_EDITOR
		/// <summary>
		/// An empty event listener used for wiring up persistent Unity events in the editor.
		/// </summary>
		public void NullEventListener() {
			throw new NotImplementedException();
		}
#endif
	}
}