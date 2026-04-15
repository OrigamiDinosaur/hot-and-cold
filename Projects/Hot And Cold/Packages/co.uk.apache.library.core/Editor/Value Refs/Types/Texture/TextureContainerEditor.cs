using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(TextureContainer))]
	public class TextureContainerEditor : ValueContainerEditor<Texture> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.ObjectField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val, typeof(Texture), false) as Texture;
		}
	}
}