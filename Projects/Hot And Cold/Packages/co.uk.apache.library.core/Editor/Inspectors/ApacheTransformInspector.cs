using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Apache.Core.Editor {

	[CustomEditor(typeof(Transform))]
	[CanEditMultipleObjects]
	public sealed class ApacheTransformInspector : UnityEditor.Editor {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string POSITION_BUTTON_LABEL = "P";
		private const string POSITION_BUTTON_DESCRIPTION = "Reset Position";
		private const string ROTATION_BUTTON_LABEL = "R";
		private const string ROTATION_BUTTON_DESCRIPTION = "Reset Rotation";
		private const string SCALE_BUTTON_LABEL = "S";
		private const string SCALE_BUTTON_DESCRIPTION = "Reset Scale";

		private const string SET_TRANSFORM_DESCRIPTION = "Transform Change";

		private const string VECTOR3_X_LABEL = "X";
		private const string VECTOR3_Y_LABEL = "Y";
		private const string VECTOR3_Z_LABEL = "Z";

		private const float BUTTON_WIDTH = 20;
		private const float LABEL_WIDTH = 15;
		private const float FIELD_MIN_WIDTH = 30;

		private const float FLOATING_POINT_CORRECTION_THRESHOLD = 0.0001f;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnInspectorGUI() {

			// configure control style.
			EditorGUIUtility.labelWidth = LABEL_WIDTH;

			// grab primary transform and other selected transforms.
			Transform[] transforms = targets.Select(t => t as Transform).ToArray();

			// if nothing is selected, return.
			if (transforms.Length == 0) return;

			// get the position/rotation/scale, where variation in the selected elements is represented by NaN.
			Vector3 position = GetVariedVectors(transforms.Select(t => t.localPosition).ToArray());
			Vector3 newPosition = position;

			Vector3 rotation = GetVariedVectors(transforms.Select(t => t.localEulerAngles).ToArray(), true);
			Vector3 newRotation = rotation;

			Vector3 scale = GetVariedVectors(transforms.Select(t => t.localScale).ToArray());
			Vector3 newScale = scale;

			// draw position button.
			EditorGUILayout.BeginHorizontal();
			{
				if (DrawButton(POSITION_BUTTON_LABEL, POSITION_BUTTON_DESCRIPTION, IsResetPositionValid(transforms))) {
					RegisterUndo(POSITION_BUTTON_DESCRIPTION, transforms);

					foreach (Transform transform in transforms) {
						transform.localPosition = Vector3.zero;
					}
				}
				newPosition = DrawVector3(newPosition);
				UpdateVectors(position, newPosition, transforms, PositionAction);
			}
			EditorGUILayout.EndHorizontal();

			// draw rotation button.
			EditorGUILayout.BeginHorizontal();
			{
				if (DrawButton(ROTATION_BUTTON_LABEL, ROTATION_BUTTON_DESCRIPTION, IsResetRotationValid(transforms))) {
					RegisterUndo(ROTATION_BUTTON_DESCRIPTION, transforms);

					foreach (Transform transform in transforms) {
						transform.localEulerAngles = Vector3.zero;
					}
				}
				newRotation = DrawVector3(newRotation);
				UpdateVectors(rotation, newRotation, transforms, RotationAction);
			}
			EditorGUILayout.EndHorizontal();

			// draw scale button.
			EditorGUILayout.BeginHorizontal();
			{
				if (DrawButton(SCALE_BUTTON_LABEL, SCALE_BUTTON_DESCRIPTION, IsResetScaleValid(transforms))) {
					RegisterUndo(SCALE_BUTTON_DESCRIPTION, transforms);

					foreach (Transform transform in transforms) {
						transform.localScale = Vector3.one;
					}
				}
				newScale = DrawVector3(newScale);
				UpdateVectors(scale, newScale, transforms, ScaleAction);
			}
			EditorGUILayout.EndHorizontal();
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		// Updates elements of a referenced transform, using an action, if x/y/z components of newVector are not equal to oldVector.
		private static void UpdateVectors(Vector3 oldVector, Vector3 newVector, Transform[] transforms, Action<Transform, float, float, float> onAction) {
			if (!FloatEquals(newVector.x, oldVector.x)) {
				RegisterUndo(SET_TRANSFORM_DESCRIPTION, transforms);

				foreach (Transform otherTransform in transforms) {
					onAction(otherTransform, ValidateVector(newVector).x, float.NaN, float.NaN);
				}
			}
			if (!FloatEquals(newVector.y, oldVector.y)) {
				RegisterUndo(SET_TRANSFORM_DESCRIPTION, transforms);

				foreach (Transform otherTransform in transforms) {
					onAction(otherTransform, float.NaN, ValidateVector(newVector).y, float.NaN);
				}
			}
			if (!FloatEquals(newVector.z, oldVector.z)) {
				RegisterUndo(SET_TRANSFORM_DESCRIPTION, transforms);

				foreach (Transform otherTransform in transforms) {
					onAction(otherTransform, float.NaN, float.NaN, ValidateVector(newVector).z);
				}
			}
		}

		// Position action, used by UpdateVectors.
		private static void PositionAction(Transform transform, float x = float.NaN, float y = float.NaN, float z = float.NaN) {
			Vector3 localPosition = transform.localPosition;
			localPosition.x = float.IsNaN(x) ? localPosition.x : x;
			localPosition.y = float.IsNaN(y) ? localPosition.y : y;
			localPosition.z = float.IsNaN(z) ? localPosition.z : z;
			transform.localPosition = localPosition;
		}

		// Rotation action, used by UpdateVectors.
		private static void RotationAction(Transform transform, float x = float.NaN, float y = float.NaN, float z = float.NaN) {
			Vector3 localRotation = transform.localEulerAngles;
			localRotation.x = float.IsNaN(x) ? localRotation.x : x;
			localRotation.y = float.IsNaN(y) ? localRotation.y : y;
			localRotation.z = float.IsNaN(z) ? localRotation.z : z;
			transform.localEulerAngles = localRotation;
		}

		// Scale action, used by UpdateVectors.
		private static void ScaleAction(Transform transform, float x = float.NaN, float y = float.NaN, float z = float.NaN) {
			Vector3 localScale = transform.localScale;
			localScale.x = float.IsNaN(x) ? localScale.x : x;
			localScale.y = float.IsNaN(y) ? localScale.y : y;
			localScale.z = float.IsNaN(z) ? localScale.z : z;
			transform.localScale = localScale;
		}

		// Check if floats are equal, supports NaN.
		private static bool FloatEquals(float a, float b) {
			return (a == b || float.IsNaN(a) && float.IsNaN(b));
		}

		// Return a vector which IsNaN in x/y/z if the passed vector[] contains variation in that component.
		private static Vector3 GetVariedVectors(Vector3[] vectors, bool roundFloatingPointImprecision = false) {
			float x = ContainsVariation(vectors.Select(v => v.x).ToArray(), roundFloatingPointImprecision) ? float.NaN : vectors.First().x;
			float y = ContainsVariation(vectors.Select(v => v.y).ToArray(), roundFloatingPointImprecision) ? float.NaN : vectors.First().y;
			float z = ContainsVariation(vectors.Select(v => v.z).ToArray(), roundFloatingPointImprecision) ? float.NaN : vectors.First().z;
			return new Vector3(x, y, z);
		}

		// Check whether an array is made up of only identical keys.
		private static bool ContainsVariation(float[] values, bool roundFloatingPointImprecisions = false) {

			// if the values are of length one, there can't be any difference.
			if (values.Length == 1) return false;

			// grab the first value, and see if we have a difference...
			float firstValue = values.First();
			foreach (float value in values) {

				// we're rounding, so check if we have a difference outside of the threshold.
				if (roundFloatingPointImprecisions) {
					if (Mathf.Abs(value - firstValue) > FLOATING_POINT_CORRECTION_THRESHOLD) return true;
				}

				// we're not rounding, do simple equality comparison.
				else {
					if (value != firstValue) return true;
				}
			}

			// no differences found in the array of values!
			return false;
		}

		// Validates values to remove 'NaN's.
		private static Vector3 ValidateVector(Vector3 vector) {
			vector.x = float.IsNaN(vector.x) ? 0 : vector.x;
			vector.y = float.IsNaN(vector.y) ? 0 : vector.y;
			vector.z = float.IsNaN(vector.z) ? 0 : vector.z;
			return vector;
		}

		// Draws a button in either enabled or disabled state.
		private static bool DrawButton(string title, string tooltip, bool enabled) {
			GUI.enabled = enabled;
			bool pressed = GUILayout.Button(new GUIContent(title, tooltip), GUILayout.Width(BUTTON_WIDTH));
			GUI.enabled = true;
			return pressed;
		}

		// Draws the three fields of float values.
		private static Vector3 DrawVector3(Vector3 value) {
			GUILayoutOption layoutOption = GUILayout.MinWidth(FIELD_MIN_WIDTH);
			value.x = DrawFloatField(value.x, VECTOR3_X_LABEL, layoutOption);
			value.y = DrawFloatField(value.y, VECTOR3_Y_LABEL, layoutOption);
			value.z = DrawFloatField(value.z, VECTOR3_Z_LABEL, layoutOption);
			return value;
		}

		// Draws a float field if the value is !NaN. Otherwise, draw a mixed value and try to parse out any changes.
		private static float DrawFloatField(float value, string label, GUILayoutOption layoutOption) {

			// if we have a NaN value, we have a mixed value...
			if (float.IsNaN(value)) {

				// so begin checking if a change occurs, and render out a mixed value, which looks like a dash.
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = true;
				value = EditorGUILayout.FloatField(label, value, layoutOption);
				EditorGUI.showMixedValue = false;
			}
			else {
				value = EditorGUILayout.FloatField(label, value, layoutOption);
			}

			return value;
		}

		// Determines whether it's worth showing the reset position button.
		private static bool IsResetPositionValid(params Transform[] targetTransform) {
			return targetTransform.Select(t => t.localPosition).Aggregate(false, (current, v) => current | (v.x != 0 || v.y != 0 || v.z != 0));
		}

		// Determines whether it's worth showing the reset rotation button.
		private static bool IsResetRotationValid(params Transform[] targetTransform) {
			return targetTransform.Select(t => t.localEulerAngles).Aggregate(false, (current, v) => current | (v.x != 0 || v.y != 0 || v.z != 0));
		}

		// Determines whether it's worth showing the reset scale button.
		private static bool IsResetScaleValid(params Transform[] targetTransform) {
			return targetTransform.Select(t => t.localScale).Aggregate(false, (current, v) => current | (v.x != 1 || v.y != 1 || v.z != 1));
		}

		// Registers an action to be undoable.
		private static void RegisterUndo(string name, params Transform[] objects) {
			for (int i = 0; i < objects.Length; i++) {
				if (objects[i] == null) continue;
				Undo.RecordObject(objects[i], name);
				EditorUtility.SetDirty(objects[i]);
			}
		}
	}
}