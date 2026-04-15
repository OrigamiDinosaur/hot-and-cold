using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(Color32Container))]
	public class Color32ContainerEditor : ValueContainerEditor<Color32> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.ColorField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}