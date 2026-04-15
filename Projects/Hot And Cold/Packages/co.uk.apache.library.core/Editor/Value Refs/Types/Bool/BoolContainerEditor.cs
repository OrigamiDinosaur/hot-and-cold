using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(BoolContainer))]
	public class BoolContainerEditor : ValueContainerEditor<bool> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.Toggle(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}