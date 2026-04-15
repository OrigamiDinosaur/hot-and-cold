using UnityEngine;

namespace Apache.Core {

	/// <summary>
	/// An <c>ApacheComponent</c> singleton designed to be assigned on a game object or prefab in the editor during design time
	/// as opposed to lazily instantiated when the <c>Instance</c> is accessed.
	/// </summary>
	/// <typeparam name="T">The underlying type which acquires the singleton behaviour.</typeparam>
	/// <remarks>With <c>ComponentSingletonProtected</c>, its <c>Instance</c> is not publicly accessible.</remarks>
	public abstract class ComponentSingletonProtected<T> : ApacheComponent where T : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		private static T _instance;

		//-----------------------------------------------------------------------------------------
		// Protected Fields:
		//-----------------------------------------------------------------------------------------

		protected bool isDuplicateInstance;

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		protected static T Instance {
			get {

				// log an error if there is no instance.
				if (_instance == null) {
					Debug.LogError("Component singleton instance of " + typeof(T) + " does not exist in scene. Did you forget to add a component?");
				}

				return _instance;
			}
		}

		protected virtual bool ShouldNotDestroyOnLoad => false;
		
		//-----------------------------------------------------------------------------------------
		// Protected Methods - Virtual:
		//-----------------------------------------------------------------------------------------
		
		protected virtual void OnIsDuplicateInstanceAndWillDelete() { }

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected virtual void Awake() {
			
			// if the instance is active already...
			if (_instance != null) {

				// tell ourselves that we're going to be deleted as a duplicate, then deactivate and destroy ourselves and back out.
				// N.B. it makes sense for the first instance created to be the one that lives on; it's a singleton, after all.
				// N.B. we don't deactivate and destroy because doing so would prevent any error messages being logged before we destroy.
				isDuplicateInstance = true;
				OnIsDuplicateInstanceAndWillDelete();
				Destroy(gameObject);
				return;
			}

			_instance = this as T;

			// if we're not destroying on load, handle that now.
			if (ShouldNotDestroyOnLoad) {
				DontDestroyOnLoad(gameObject);
			}
		}

		protected virtual void OnDestroy() {
			
			// if we're a duplicate instance, don't try to clear out our instance field, as we wouldn't have changed it.
			if (isDuplicateInstance) return;
			
			_instance = null;
		}
	}
}