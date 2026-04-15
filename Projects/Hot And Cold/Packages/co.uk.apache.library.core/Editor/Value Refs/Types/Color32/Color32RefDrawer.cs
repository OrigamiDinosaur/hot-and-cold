using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomPropertyDrawer(typeof(Color32Ref))]
	public class Color32RefDrawer : ValueRefDrawer {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			OnTypeVariableGUI<Color32Container>(position, property, label);
		}
	}
}