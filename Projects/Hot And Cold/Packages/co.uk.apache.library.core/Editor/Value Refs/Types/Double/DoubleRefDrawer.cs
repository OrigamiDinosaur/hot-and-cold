using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomPropertyDrawer(typeof(DoubleRef))]
	public class DoubleRefDrawer : ValueRefDrawer {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			OnTypeVariableGUI<DoubleContainer>(position, property, label);
		}
	}
}