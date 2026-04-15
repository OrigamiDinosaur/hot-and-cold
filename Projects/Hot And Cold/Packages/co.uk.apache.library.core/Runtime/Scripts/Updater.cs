using System;

namespace Apache.Core {

	/// <inheritdoc />
	/// <summary>
	/// Simply handles the updating of components via a single Unity Update event for performance reasons.
	/// </summary>
	public class Updater : SingletonProtectedHiddenPersistent<Updater> {

		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public static event Action Updated;
		public static event Action LateUpdated;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Update() {
			Updated?.Invoke();
		}

		protected void LateUpdate() {
			LateUpdated?.Invoke();
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		static Updater() {

			// create our singleton instance when statically accessed; this will stick around across scenes.
			HandleExistingOrCreateNewInstance();
		}
	}
}