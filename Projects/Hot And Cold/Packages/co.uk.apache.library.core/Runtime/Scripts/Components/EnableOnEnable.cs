using System;
using UnityEngine;

namespace Apache.Core {
	public class EnableOnEnable : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Inspector Classes:
		//-----------------------------------------------------------------------------------------

		[Serializable]
		public class GameObjectRefs {

			public GameObject GameObject;
			public float Time;

			[ApacheSpace]

			public GameObject SwapOut;
		}

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField] protected GameObjectRefs[] gameObjects;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void OnEnable() {
			for (int i = 0; i < gameObjects.Length; i++) {

				// ensure the game object is disabled by default.
				gameObjects[i].GameObject.SetActive(false);

				// set the game object active after the delay.
				// N.B. we capture i so as to 'close' its value in the closure.
				int index = i;
				sequence.Do(gameObjects[i].Time, () => {
					gameObjects[index].GameObject.SetActive(true);

					// if we have a swap out, disable it.
					if (gameObjects[index].SwapOut != null) {
						gameObjects[index].SwapOut.SetActive(false);
					}
				});
			}
		}

		protected void OnDisable() {
			sequence.Cancel();
		}
	}
}