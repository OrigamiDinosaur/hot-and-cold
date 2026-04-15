using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(Vector3Container))]
	public class Vector3ContainerEditor : ValueContainerEditor<Vector3> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.Vector3Field(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}