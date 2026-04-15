using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(RenderTextureContainer))]
	public class RenderTextureContainerEditor : ValueContainerEditor<RenderTexture> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGUILayout.ObjectField(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val, typeof(RenderTexture), false) as RenderTexture;
		}
	}
}