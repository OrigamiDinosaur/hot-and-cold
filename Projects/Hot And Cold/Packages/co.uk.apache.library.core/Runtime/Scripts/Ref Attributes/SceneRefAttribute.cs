using System;
using System.Collections.Generic;
using Apache.Core.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Apache.Core {
	public class SceneRefAttribute : OrderableRefAttribute {

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected override Object GetObjectFromObject(object @object, Type type) {
			Object[] objects = GetObjectsFromObject(@object, type);
			return (!objects.IsNullOrEmpty()) ? objects[0] : null;
		}

		protected override Object[] GetObjectsFromObject(object @object, Type type) {

			// work out the scene in which we'll search, defaulting to active scene, and use the scene from the object if we can.
			Scene scene = SceneManager.GetActiveScene();
			GameObject gameObject = GetGameObjectFromObject(@object);
			if (gameObject != null) {
				scene = gameObject.scene;
			}

			// find all components as objects, cast all elements into a component array, sort, and return.
			Object[] objects = FindSceneObjectsOfType(type, scene);
			if (objects.IsNullOrEmpty()) return null;
			objects = OrderObjects(objects);
			return objects;
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private static Object[] FindSceneObjectsOfType(Type type, Scene scene) {

			// find objects, which includes those on disk.
			Object[] objects = Resources.FindObjectsOfTypeAll(type);

			// N.B. I could filter the above objects using LINQ and Where and save a lot of the code below, but this method is called often
			// (every time an ApacheComponent window is drawn, for each [SceneRef] attribute), so I want to go the more performant route.
			// In addition, because of the UNITY_EDITOR requirement below, it would necessitate two Wheres. Faster is better in this case.

			// loop through all objects adding them to the list...
			List<Object> objectList = new List<Object>();
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (Object @object in objects) {

				// if this is not a hidden object, don't allow it.
				if (@object.hideFlags != HideFlags.None) continue;

				// in the editor, if this is a persistent (i.e. on disk) object, don't allow it.
				// N.B. this work is only ever done in editor windows, so we can safely assume this will always be called from the editor.
				// However, we should make sure not to call into editor APIs outside of UNITY_EDITOR, as these attributes are not in an editor directory.
#if UNITY_EDITOR
				if (UnityEditor.EditorUtility.IsPersistent(@object)) continue;
#endif

				// get the game object from the object, and if it is in a different scene, continue.
				GameObject gameObject = GetGameObjectFromObject(@object);
				if (gameObject != null && gameObject.scene != scene) continue;

				// it survived filtration, so add it to the list.
				objectList.Add(@object);
			}

			// if we're working with cameras, if any of them is the main camera and it's not at the beginning, move it to the beginning of the list.
			// N.B. note we start with an index of 1 so we can avoid checking if it's not at the beginning.
			if (type == typeof(Camera) && objectList.Count > 1) {
				for (int i = 1; i < objectList.Count; i++) {

					// grab the object as a camera and continue if it isn't the main camera.
					Camera cam = objectList[i] as Camera;
					if (cam == null || cam != Camera.main) continue;

					// we have the main camera object, so shift it to the front of the list and break out.
					Object mainCamObject = objectList[i];
					objectList.RemoveAt(i);
					objectList.Insert(0, mainCamObject);
					break;
				}
			}

			return objectList.ToArray();
		}

		private static GameObject GetGameObjectFromObject(object @object) {
			GameObject gameObject = @object as GameObject;
			if (gameObject != null) return gameObject;
			Component component = @object as Component;
			return (component != null) ? component.gameObject : null;
		}
	}
}