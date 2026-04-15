using System.Linq;
using System.Reflection;
using Apache.Core.Extensions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Apache.Core.Editor {

	[CustomPropertyDrawer(typeof(AnimatorStateEnumeration))]
	public class AnimatorStateEnumerationPropertyDrawer : PropertyDrawer {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string ANIMATOR_PROPERTY    = "animator";
		private const string LAYERS_PROPERTY      = "layers";
		private const string STATES_PROPERTY      = "states";
		private const string LAYER_INDEX_PROPERTY = "layerIndex";
		private const string STATE_INDEX_PROPERTY = "stateIndex";

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		private int _selectedLayer = -1;
		private int _selectedState = -1;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private string[] layers;
		private string[] states;

		private SerializedProperty baseProperty;

		//-----------------------------------------------------------------------------------------
		// Private Properties:
		//-----------------------------------------------------------------------------------------

		private int SelectedLayer {
			set {
				if (_selectedLayer != value) {
					_selectedLayer = value;
					baseProperty.FindPropertyRelative(LAYER_INDEX_PROPERTY).intValue = _selectedLayer;
					Initialise(baseProperty);
					ResetSelectedState();
				}
			}
		}

		private int SelectedState {
			set {
				if (_selectedState != value) {
					_selectedState = value;
					baseProperty.FindPropertyRelative(STATE_INDEX_PROPERTY).intValue = _selectedState;
				}
			}
		}

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			baseProperty = property;

			var properties = new {
				animator = property.FindPropertyRelative(ANIMATOR_PROPERTY),
				layers = property.FindPropertyRelative(LAYERS_PROPERTY),
				states = property.FindPropertyRelative(STATES_PROPERTY),
				layerIndex = property.FindPropertyRelative(LAYER_INDEX_PROPERTY),
				stateIndex = property.FindPropertyRelative(STATE_INDEX_PROPERTY)
			};

			bool initialised = Initialise(property);

			if (!initialised) {
				GUI.Label(position, "Animator or AnimationController missing.");
				return;
			}

			EditorGUI.BeginProperty(position, label, property);

			EditorGUI.PrefixLabel(position, label);
			Rect popupRect = position;
			popupRect.x += EditorGUIUtility.labelWidth;
			popupRect.width -= EditorGUIUtility.labelWidth;

			SelectedLayer = EditorGUI.Popup(popupRect.SplitX(0, 2), "", properties.layerIndex.intValue, layers);
			SelectedState = EditorGUI.Popup(popupRect.SplitX(1, 2), "", properties.stateIndex.intValue, states);

			EditorGUI.EndProperty();
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private bool Initialise(SerializedProperty property) {

			// find all the properties once and cache in an anonymous class type.
			var properties = new {
				animator   = property.FindPropertyRelative(ANIMATOR_PROPERTY),
				layers     = property.FindPropertyRelative(LAYERS_PROPERTY),
				states     = property.FindPropertyRelative(STATES_PROPERTY),
				layerIndex = property.FindPropertyRelative(LAYER_INDEX_PROPERTY),
				stateIndex = property.FindPropertyRelative(STATE_INDEX_PROPERTY)
			};

			// find the animator reference on the owning component.
			Animator animator = FindAnimatorOnComponent(property.serializedObject.targetObject);
			if (animator == null) return false;

			// find the animation controller on the animator.
			AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
			if (animatorController == null) return false;
			
			// find all the layers and assign them to the layers property.
			layers = animatorController.layers.Select(l => l.name).ToArray();
			properties.layers.arraySize = layers.Length;
			properties.layers.ClearArray();
			for (int i = 0; i < layers.Length; i++) {
				properties.layers.InsertArrayElementAtIndex(i);
				SerializedProperty element = properties.layers.GetArrayElementAtIndex(i);
				element.stringValue = layers[i];
			}

			// find the selected layer index.
			int layerIndex = properties.layerIndex.intValue;

			// find the state name from the active layer index.
			states = animatorController.layers[layerIndex].stateMachine.states.Select(s => s.state.name).ToArray();
			properties.states.arraySize = states.Length;
			properties.states.ClearArray();
			for (int i = 0; i < states.Length; i++) {
				properties.states.InsertArrayElementAtIndex(i);
				SerializedProperty element = properties.states.GetArrayElementAtIndex(i);
				element.stringValue = states[i];
			}

			return true;
		}

		private void ResetSelectedState() {
			SelectedState = -1;
			SelectedState = 0;
		}

		private static Animator FindAnimatorOnComponent(object targetObject) {
			if (targetObject == null) return null;

			MonoBehaviour monoBehaviour = targetObject as MonoBehaviour;
			if (monoBehaviour == null) return null;

			FieldInfo animatorFieldInfo = monoBehaviour.GetType().GetField(ANIMATOR_PROPERTY, BindingFlag.INSTANCE_NON_PUBLIC);
			if (animatorFieldInfo == null) return null;

			// if the animator is null, return it anyway.
			object animatorObject = animatorFieldInfo.GetValue(monoBehaviour);
			Animator animator = animatorObject as Animator;
			return animator;
		}
	}
}