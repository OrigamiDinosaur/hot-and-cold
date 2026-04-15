using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(Vector3IntContainer))]
	public class Vector3IntContainerEditor : ValueContainerEditor<Vector3Int> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.Vector3IntField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}