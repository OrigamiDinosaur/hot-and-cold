using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(Vector2IntContainer))]
	public class Vector2IntContainerEditor : ValueContainerEditor<Vector2Int> {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		protected const string INITIAL_VALUE_LABEL = "Initial Value";

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private Vector2Int initialValue;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public new void OnEnable() {
			base.OnEnable();

			initialValue = (Vector2Int)GetInitialValueFieldInfo().GetValue(valueContainer);
		}

#if UNITY_EDITOR
		public override void OnInspectorGUI() {

			// the default inspector draws Vector2s weird so lets do it our nicer way instead.
			// N.B. because of this we have to add a layer on top of the valueContainer via the initialValue field in this class.
			initialValue = EditorGuiLayoutUtils.Vector2IntFieldFullWidth(INITIAL_VALUE_LABEL, initialValue);

			// if the application is playing then draw the runtime values.
			if (Application.isPlaying) {
				valueContainer.Val = EditorGuiLayoutUtils.Vector2IntFieldFullWidth(RUNTIME_VALUE_LABEL, valueContainer.Val);
			}

			if (!GUI.changed) return;

			// get initial value from field with reflection.
			FieldInfo initialValueFieldInfo = GetInitialValueFieldInfo();
			Vector2Int baseInitialValue = (Vector2Int)initialValueFieldInfo.GetValue(valueContainer);

			// if the initial value has changed in the editor compared with our underlying initial value... 
			if (!Equals(initialValue, baseInitialValue)) {

				// set value on our initial value field info.
				initialValueFieldInfo.SetValue(valueContainer, initialValue);
			}

			// if the application is play, then check if the valueContainer value has changed, and if it has invoke our changed event.
			if (!Application.isPlaying) return;
			if (Equals(previousValue, valueContainer.Val)) return;
			previousValue = valueContainer.Val;
			valueContainer.InvokeChangedEvent();
		}
#endif

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected override void ValueTypeSpecificEditorCode() { }

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private FieldInfo GetInitialValueFieldInfo() {
			return valueContainer.GetType().GetField("initialValue", BindingFlag.INSTANCE_NON_PUBLIC);
		}
	}
}