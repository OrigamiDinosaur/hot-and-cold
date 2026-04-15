using UnityEngine;

namespace Apache.Core {
	public class KeepAliveInScenes : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected Scenes[] scenes;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {

			// loop through all scenes, and if we're in one of them, back out.
			for (int i = 0; i < scenes.Length; i++) {
				if (AppController.Scene == scenes[i]) return;
			}

			// if we make it here, we're not in a relevant scene, so deactivate and destroy ourselves.
			DeactivateAndDestroy();
		}
	}
}