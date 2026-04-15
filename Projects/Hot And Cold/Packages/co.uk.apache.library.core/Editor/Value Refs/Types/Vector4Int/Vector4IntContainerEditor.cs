using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	[CustomEditor(typeof(Vector4IntContainer))]
	public class Vector4IntContainerEditor : ValueContainerEditor<Vector4Int> {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		protected const string INITIAL_VALUE_LABEL = "Initial Value";

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private Vector4Int initialValue;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public new void OnEnable() {
			base.OnEnable();

			initialValue = (Vector4Int)GetInitialValueFieldInfo().GetValue(valueContainer);
		}

#if UNITY_EDITOR
		public override void OnInspectorGUI() {

			// the default inspector draws Vector4Ints weird so lets do it our nicer way instead.
			// N.B. because of this we have to add a layer on top of the valueContainer via the initialValue field in this class.
			initialValue = EditorGuiLayoutUtils.Vector4IntField(INITIAL_VALUE_LABEL, initialValue);

			// if the application is playing then draw the runtime values.
			if (Application.isPlaying) {
				valueContainer.Val = EditorGuiLayoutUtils.Vector4IntField(RUNTIME_VALUE_LABEL, valueContainer.Val);
			}

			if (!GUI.changed) return;

			// get initial value from field with reflection.
			FieldInfo initialValueFieldInfo = GetInitialValueFieldInfo();
			Vector4Int baseInitialValue = (Vector4Int)initialValueFieldInfo.GetValue(valueContainer);

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