using Apache.Core.Extensions;
using UnityEngine;

namespace Apache.Core {

	/// <summary>
	/// An <c>ApacheComponent</c> singleton designed to be dynamically, lazily instantiated when <c>Instance</c> is accessed.
	/// </summary>
	/// <typeparam name="T">The underlying type which acquires the singleton behaviour.</typeparam>
	/// <remarks>With <c>SingletonProtected</c>, its <c>Instance</c> is not publicly accessible.</remarks>
	public abstract class SingletonProtected<T> : ApacheComponent where T : ApacheComponent {

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		private static T _instance;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable once StaticMemberInGenericType
		private static readonly object syncRoot = new object();

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		protected static T Instance {
			get {
				HandleExistingOrCreateNewInstance();
				return _instance;
			}
		}

		/// <summary>Determine whether the singleton has an instance without dynamically reinstantiating it.</summary>
		protected static bool HasInstance => (_instance != null);

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------
		
		protected virtual void OnDestroy() {
			_instance = null;
		}
		
		//-----------------------------------------------------------------------------------------
		// Protected Methods - Virtual:
		//-----------------------------------------------------------------------------------------

		protected virtual void OnInstanceCreated() { }

		protected virtual void OnIsDuplicateInstanceAndWillDelete() { }

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Attempts to use an existing instance and, if one cannot be found, creates it.</summary>
		protected static void HandleExistingOrCreateNewInstance() {

			// if we already have an instance, back out.
			if (_instance != null) return;

			lock (syncRoot) {

				// find all objects of this type.
				T[] existingObjects = FindObjectsByType<T>();
				if (existingObjects.Length > 0) {

					// if there are more than 0, clear all instances until there is only one.
					for (int i = 1; i < existingObjects.Length; i++) {

						// tell the existing instance that it's going to be deleted for being a duplicate.
						SingletonProtected<T> duplicateInstance = existingObjects[i] as SingletonProtected<T>;
						if (duplicateInstance != null) {
							duplicateInstance.OnIsDuplicateInstanceAndWillDelete();
						}
						
						existingObjects[i].gameObject.DeactivateAndDestroy();
					}

					// return the one pre-existing instance.
					_instance = existingObjects[0];
					return;
				}

				// no existing instances found, so create a new game object, naming it based on the type of T.
				GameObject newObject = new GameObject(typeof(T).Name.CamelOrPascalToTitleCase());

				// add a new component of type T.
				T instanceComponent = (T)newObject.AddComponent(typeof(T));

				// tell the singleton that a new instance has been created.
				SingletonProtected<T> instance = instanceComponent as SingletonProtected<T>;
				if (instance != null) {
					instance.OnInstanceCreated();
				}

				// return the new instance component.
				_instance = instanceComponent;
			}
		}
	}
}