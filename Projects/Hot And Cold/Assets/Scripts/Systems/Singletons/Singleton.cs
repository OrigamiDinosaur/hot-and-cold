using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

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
	// Public Properties:
	//-----------------------------------------------------------------------------------------

	public static T Instance {
		get {
			HandleExistingOrCreateNewInstance();
			return _instance;
		}
	}

	/// <summary>Determine whether the singleton has an instance without dynamically reinstantiating it.</summary>
	public static bool HasInstance => (_instance != null);
	
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
			T[] existingObjects = FindObjectsOfType<T>();
			if (existingObjects.Length > 0) {

				// if there are more than 0, clear all instances until there is only one.
				for (int i = 1; i < existingObjects.Length; i++) {

					// tell the existing instance that it's going to be deleted for being a duplicate.
					Singleton<T> duplicateInstance = existingObjects[i] as Singleton<T>;
					if (duplicateInstance != null) {
						duplicateInstance.OnIsDuplicateInstanceAndWillDelete();
					}
						
					existingObjects[i].gameObject.SetActive(false);
					Destroy(existingObjects[i]);
				}

				// return the one pre-existing instance.
				_instance = existingObjects[0];
				return;
			}

			// no existing instances found, so create a new game object, naming it based on the type of T.
			GameObject newObject = new GameObject(typeof(T).Name);

			// add a new component of type T.
			T instanceComponent = (T)newObject.AddComponent(typeof(T));

			// tell the singleton that a new instance has been created.
			Singleton<T> instance = instanceComponent as Singleton<T>;
			if (instance != null) {
				instance.OnInstanceCreated();
			}

			// return the new instance component.
			_instance = instanceComponent;
		}
	}
}