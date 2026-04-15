using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomPropertyDrawer(typeof(Vector2IntRef))]
	public class Vector2IntRefDrawer : ValueRefDrawer {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			OnTypeVariableGUI<Vector2IntContainer>(position, property, label);
		}
	}
}