using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(Texture2DContainer))]
	public class Texture2DContainerEditor : ValueContainerEditor<Texture2D> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.ObjectField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val, typeof(Texture2D), false) as Texture2D;
		}
	}
}