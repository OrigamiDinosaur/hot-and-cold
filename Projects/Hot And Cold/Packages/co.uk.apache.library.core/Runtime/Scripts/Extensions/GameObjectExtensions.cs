using UnityEngine;

namespace Apache.Core.Extensions {
	public static class GameObjectExtensions {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Deactivates and destroys the game object.</summary>
		/// <remarks>
		/// This is nice to do as it ensures that all subsequent Unity lifecycle events do not run on this game object while
		/// it is waiting to be destroyed after the current Update loop.
		/// </remarks>
		public static void DeactivateAndDestroy(this GameObject self) {
			self.SetActive(false);
			Object.Destroy(self);
		}
	}
}