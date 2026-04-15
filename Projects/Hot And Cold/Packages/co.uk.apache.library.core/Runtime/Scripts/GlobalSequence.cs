using System;
using System.Collections;

namespace Apache.Core {
	public class GlobalSequence : SingletonProtectedHidden<GlobalSequence> {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Invokes an action after a delay.
		/// </summary>
		/// <param name="delay">The delay before invoking the action.</param>
		/// <param name="action">The action to invoke.</param>
		public static void Do(float delay, Action action) {
			Instance.sequence.Do(delay, action);
		}

		/// <summary>
		/// Invokes an action after a realtime delay uninfluenced by time scale.
		/// </summary>
		/// <param name="delay">The realtime delay before invoking the action.</param>
		/// <param name="action">The action to invoke.</param>
		public static void DoRealtime(float delay, Action action) {
			Instance.sequence.DoRealtime(delay, action);
		}

		/// <summary>
		/// Invokes an action after a delay, either realtime or affected by time scale.
		/// </summary>
		/// <param name="delay">The delay before invoking the action.</param>
		/// <param name="isRealtime">Whether the delay is realtime (unaffected by time scale) or not (affected by time scale).</param>
		/// <param name="action">The action to invoke.</param>
		public static void Do(float delay, bool isRealtime, Action action) {
			Instance.sequence.Do(delay, isRealtime, action);
		}

		/// <summary>
		/// Starts a coroutine managed by the global sequence.
		/// </summary>
		/// <param name="coroutine">The coroutine to start.</param>
		public static void Coroutine(IEnumerator coroutine) {
			Instance.sequence.Coroutine(coroutine);
		}

		/// <summary>
		/// Queues up an action to be invoked after all other queued actions are processed and the given delay has elapsed.
		/// </summary>
		/// <param name="delay">The delay before invoking the action, after the existing queue has been processed.</param>
		/// <param name="action">The action to invoke.</param>
		public static void Queue(float delay, Action action) {
			Instance.sequence.Queue(delay, action);
		}

		/// <summary>
		/// Queues up an action to be invoked after all other queued actions are processed and the given realtime delay has elapsed.
		/// </summary>
		/// <param name="delay">The realtime delay before invoking the action, after the existing queue has been processed.</param>
		/// <param name="action">The action to invoke.</param>
		public static void QueueRealtime(float delay, Action action) {
			Instance.sequence.QueueRealtime(delay, action);
		}

		/// <summary>
		/// Handles a tween managed by the global sequence.
		/// </summary>
		/// <param name="tween">The tween to manage.</param>
		public static void Tween(LTDescr tween) {
			Instance.sequence.Tween(tween);
		}

		/// <summary>
		/// Handles a tween managed by the global sequence, with the tween occurring in realtime uninfluenced by time scale.
		/// </summary>
		/// <param name="tween">The tween to manage.</param>
		public static void TweenRealtime(LTDescr tween) {
			Instance.sequence.TweenRealtime(tween);
		}

		/// <summary>
		/// Invokes an action on the next frame.
		/// </summary>
		/// <param name="action">The action to be performed on the next frame.</param>
		public static void NextFrame(Action action) {
			Instance.sequence.NextFrame(action);
		}

		/// <summary>
		/// Invokes an action after a given number of frames.
		/// </summary>
		/// <param name="numFrames">The number of frames after the present frame in which to execute the action.</param>
		/// <param name="action">The action to be performed in the determined frame.</param>
		public static void AfterFrames(int numFrames, Action action) {
			Instance.sequence.AfterFrames(numFrames, action);
		}

		/// <summary>
		/// Invokes a function each frame until the function returns false.
		/// </summary>
		/// <param name="func">The function to be executed each frame. When it returns false it will no longer be called.</param>
		public static void EachFrame(Func<bool> func) {
			Instance.sequence.EachFrame(func);
		}

		/// <summary>
		/// Invokes an action at the end of the current frame.
		/// </summary>
		/// <param name="action">The action to be performed in the determined frame.</param>
		public static void EndOfFrame(Action action) {
			Instance.sequence.EndOfFrame(action);
		}

		/// <summary>
		/// Invokes a function each end of frame until the function returns false.
		/// </summary>
		/// <param name="func">The function to be executed each end of frame. When it returns false it will no longer be called.</param>
		public static void EachEndOfFrame(Func<bool> func) {
			Instance.sequence.EachEndOfFrame(func);
		}

		// N.B. the below ReSharper disables prevent ReSharper complaining about our use of Unity's FixedUpdate method signature, which clearly isn't a problem.
		// ReSharper disable once Unity.InvalidStaticModifier
		// ReSharper disable once Unity.InvalidParameters

		/// <summary>
		/// Invokes an action on the next fixed update.
		/// </summary>
		/// <param name="action">The action to be performed on the next fixed update.</param>
		public static void FixedUpdate(Action action) {
			Instance.sequence.FixedUpdate(action);
		}

		/// <summary>
		/// Invokes a function each fixed update until the function returns false.
		/// </summary>
		/// <param name="func">The function to be executed each fixed update. When it returns false it will no longer be called.</param>
		public static void EachFixedUpdate(Func<bool> func) {
			Instance.sequence.EachFixedUpdate(func);
		}
	}
}