using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(LateralsContainer))]
	public class LateralsContainerEditor : ValueContainerEditor<Laterals> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val =
				(Laterals)EditorGUILayout.EnumPopup(new GUIContent(RUNTIME_VALUE_LABEL), valueContainer.Val);
		}
	}
}