using System;
using UnityEditor;
using UnityEngine;

namespace Apache.Core.Editor {

	[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
	public class EnumFlagDrawer : PropertyDrawer {

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent content) {

			// given the property may be deeply nested inside the serialised object, get the object the property's path refers to.
			object obj = EditorSerialisedUtils.GetObjectAtPropertyPath(property);
			Type enumType = obj.GetType();

			// convert the value into an object and from there into a System.Enum, which is required for Unity's EnumMaskField.
			object valueObject = Enum.ToObject(enumType, property.intValue);
			Enum newValue = EditorGUI.EnumFlagsField(position, content, valueObject as Enum);

			// and if things change, update the property value as an integer.
			if (GUI.changed) {
				property.intValue = newValue.GetHashCode();
			}
		}
	}
}