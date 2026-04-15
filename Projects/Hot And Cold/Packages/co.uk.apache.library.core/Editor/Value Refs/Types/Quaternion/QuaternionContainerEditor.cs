using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(QuaternionContainer))]
	public class QuaternionContainerEditor : ValueContainerEditor<Quaternion> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			Vector4 quatAsVector4 = EditorGUILayout.Vector4Field(new GUIContent(RUNTIME_VALUE_LABEL), new Vector4(valueContainer.Val.x, valueContainer.Val.y, valueContainer.Val.z, valueContainer.Val.w));
			Quaternion returnQuaternion = new Quaternion(quatAsVector4.x, quatAsVector4.y, quatAsVector4.z, quatAsVector4.w);
			valueContainer.Val = returnQuaternion;
		}
	}
}