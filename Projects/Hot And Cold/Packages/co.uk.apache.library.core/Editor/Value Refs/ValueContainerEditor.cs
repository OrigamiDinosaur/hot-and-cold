using UnityEngine;

namespace Apache.Core {

	public abstract class ValueContainerEditor<T> : UnityEditor.Editor {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		protected const string RUNTIME_VALUE_LABEL = "Runtime Value";

		//-----------------------------------------------------------------------------------------
		// Protected Fields:
		//-----------------------------------------------------------------------------------------

		protected ValueContainer<T> valueContainer;

		protected T previousValue;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public void OnEnable() {
			valueContainer = target as ValueContainer<T>;
			if (valueContainer != null) previousValue = valueContainer.Val;
		}

#if UNITY_EDITOR
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			if (!Application.isPlaying) return;

			// draw child specific editor code.
			ValueTypeSpecificEditorCode();

			if (!GUI.changed) return;
			if (Equals(previousValue, valueContainer.Val)) return;
			previousValue = valueContainer.Val;
			valueContainer.InvokeChangedEvent();
		}
#endif

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected abstract void ValueTypeSpecificEditorCode();
	}
}