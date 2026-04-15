using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(DoubleContainer))]
	public class DoubleContainerEditor : ValueContainerEditor<double> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.DoubleField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}