using UnityEditor;
using UnityEngine;

namespace Apache.Core.Editor {
	public static class AssetRefAttributeHelper {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static Object[] LoadAllAssetsAtPath(string assetPath) {

			// if this is the default search path, don't include folder references.
			string[] assetGuids = (assetPath == AssetRefAttribute.DEFAULT_ASSET_PATH) ?
				AssetDatabase.FindAssets(string.Empty) :
				AssetDatabase.FindAssets(string.Empty, new[] { assetPath });

			Object[] objects = new Object[assetGuids.Length];
			for (int i = 0; i < assetGuids.Length; i++) {
				objects[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGuids[i]), typeof(Object));
			}

			return objects;
		}
	}
}