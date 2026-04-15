using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomPropertyDrawer(typeof(Vector4IntRef))]
	public class Vector4IntRefDrawer : ValueRefDrawer {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			OnTypeVariableGUI<Vector4IntContainer>(position, property, label);
		}
	}
}