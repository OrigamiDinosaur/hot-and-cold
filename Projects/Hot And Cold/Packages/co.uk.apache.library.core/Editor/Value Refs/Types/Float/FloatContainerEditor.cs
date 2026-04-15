using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(FloatContainer))]
	public class FloatContainerEditor : ValueContainerEditor<float> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.FloatField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}