using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(StringContainer))]
	public class StringContainerEditor : ValueContainerEditor<string> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.TextField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}