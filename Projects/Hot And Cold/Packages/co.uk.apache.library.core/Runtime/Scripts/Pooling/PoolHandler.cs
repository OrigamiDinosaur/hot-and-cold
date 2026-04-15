using System;
using UnityEngine;

namespace Apache.Core {
	public class PoolHandler : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string NAME_FORMAT = "Pool ({0})";

		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public event Action Destroyed;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void OnDestroy() {
			Destroyed?.Invoke();
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static PoolHandler Init<T>() {

			// create a game object in the scene and assign ourselves to it.
			// ReSharper disable once UseObjectOrCollectionInitializer
			GameObject gameObject = new GameObject();

			// give it a nice name in the editor.
	#if UNITY_EDITOR
			gameObject.name = string.Format(NAME_FORMAT, typeof(T));
	#endif

			// assign ourselves to the game object.
			return gameObject.AddComponent<PoolHandler>();
		}

		public void Handle(ApacheComponent poolable) {
			poolable.transform.parent = transform;
		}
	}
}