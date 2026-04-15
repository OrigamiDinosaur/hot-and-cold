using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(IntContainer))]
	public class IntContainerEditor : ValueContainerEditor<int> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.IntField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}