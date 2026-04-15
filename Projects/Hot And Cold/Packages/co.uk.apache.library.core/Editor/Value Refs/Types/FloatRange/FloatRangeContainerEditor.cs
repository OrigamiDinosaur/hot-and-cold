using UnityEditor;

namespace Apache.Core {

	[CustomEditor(typeof(FloatRangeContainer))]
	public class FloatRangeContainerEditor : ValueContainerEditor<FloatRange> {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() {
			valueContainer.Val = EditorGuiLayoutUtils.FloatRangeField(RUNTIME_VALUE_LABEL, valueContainer.Val);
		}
	}
}