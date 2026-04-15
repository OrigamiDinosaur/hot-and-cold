using UnityEngine;

namespace Apache.Core {

	/// <inheritdoc />
	/// <remarks>
	/// With <c>SingletonProtectedHiddenPersistent</c>, the hidden object persists across scene loads. One should be careful using this class
	/// and subscribing to events, because oftentimes those subscribed-to objects get destroyed with the scene but this class holds on to a
	/// reference of them, resulting in <c>MissingReferenceException</c>s.
	/// </remarks>
	public abstract class SingletonProtectedHiddenPersistent<T> : SingletonProtectedHidden<T> where T : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected override void OnInstanceCreated() {
			base.OnInstanceCreated();

			// if we're not playing, something is wrong, and the base class will log an error. All we need to do is back out.
			if (!Application.isPlaying) return;
			
			// ensure we don't die across scene loads.
			DontDestroyOnLoad(gameObject);
		}
	}
}