using UnityEngine;

namespace Apache.Core {

	/// <inheritdoc />
	/// <remarks>
	/// With <c>SingletonProtectedHidden</c>, its <c>Instance</c> is not publicly accessible and its game object will not be visible in
	/// the hierarchy. However, it will be a member of the active scene and destroyed when changing scenes (something not afforded by
	/// <c>HideFlags.HideAndDontSave</c>) because it is likely that this object would be used to interact with <c>MonoBehaviour</c>s which
	/// often do die during a scene transition. Not dieing ourselves in that instance would result in <c>MissingReferenceException</c>s.
	/// </remarks>
	public abstract class SingletonProtectedHidden<T> : SingletonProtected<T> where T : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected override void OnInstanceCreated() {
			base.OnInstanceCreated();

			// if the application is playing, allow the game object to be hidden in the hierarchy; it will be a member of the scene and
			// die with it (i.e. during a transition to another scene), but because we're playing, we don't need to worry about the
			// object sticking around while hidden and difficult to detect, for it will be destroyed when we exit play mode.
			if (Application.isPlaying) {
				gameObject.hideFlags = HideFlags.HideInHierarchy;
				return;
			}

			// we're not in play mode, so error.
			Debug.LogError($"{ GetType().Name } with name \"{ gameObject.name }\" was created but, because we are not in play mode, " +
			               "it was not hidden. Instantiating hidden game objects in the editor while not in play mode can cause an amassment " +
			               "of invisible objects which fill up the scene without the developer's notice.");
		}
	}
}