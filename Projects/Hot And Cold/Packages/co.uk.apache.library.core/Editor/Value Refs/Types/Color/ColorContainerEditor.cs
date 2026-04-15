using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(ColorContainer))]
	public class ColorContainerEditor : ValueContainerEditor<Color> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.ColorField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}