using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Apache.Core {
	public class AssetRefAttribute : OrderableRefAttribute {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		public const string DEFAULT_ASSET_PATH = "Assets/";

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private string path = DEFAULT_ASSET_PATH;

		private Object[] assetsAtAssetPath;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		// ReSharper disable once ConvertToAutoProperty

		public string Path {
			// ReSharper disable ArrangeAccessorOwnerBody
			get { return path; }
			set { path = value; }
			// ReSharper restore ArrangeAccessorOwnerBody
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public void UpdateAssetsAtAssetPath(Object[] newAssetsAtAssetPath) {
			assetsAtAssetPath = newAssetsAtAssetPath;
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected override Object GetObjectFromObject(object @object, Type type) {
			Object[] objects = GetObjectsFromAssets(type);
			if (objects == null || objects.Length == 0) return null;
			return objects[0];
		}

		protected override Object[] GetObjectsFromObject(object @object, Type type) {
			Object[] objects = GetObjectsFromAssets(type);
			if (objects == null || objects.Length == 0) return null;
			objects = OrderObjects(objects);
			return objects;
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private Object[] GetObjectsFromAssets(Type type) {
			if (assetsAtAssetPath == null) return null;

			List<Object> objects = new List<Object>();

			foreach (Object assetObject in assetsAtAssetPath) {

				// if the type matches, add it to the object list.
				if (assetObject.GetType() == type) {
					objects.Add(assetObject);
				}

				// if it doesn't match, but it's a game object, and it has the component, get it.
				// ReSharper disable once MergeCastWithTypeCheck
				else if (assetObject is GameObject) {
					Component component = ((GameObject)assetObject).GetComponent(type);
					if (component != null) {
						objects.Add(component);
					}
				}
			}

			// finally convert the object list back into an array.
			return objects.ToArray();
		}
	}
}