using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(TransformContainer))]
	public class TransformContainerEditor : ValueContainerEditor<Transform> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.ObjectField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val, typeof(Transform), false) as Transform;
		}
	}
}