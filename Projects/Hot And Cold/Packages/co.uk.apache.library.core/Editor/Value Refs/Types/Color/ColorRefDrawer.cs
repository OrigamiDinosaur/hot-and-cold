using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomPropertyDrawer(typeof(ColorRef))]
	public class ColorRefDrawer : ValueRefDrawer {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			OnTypeVariableGUI<ColorContainer>(position, property, label);
		}
	}
}