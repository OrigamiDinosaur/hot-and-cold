using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace Apache.Core.Editor {
	public static class EditorSerialisedUtils {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Gets the object that the property represents.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The object the property represents.</returns>
		public static object GetObjectAtPropertyPath(SerializedProperty property) {

			// N.B. inlining constants as these are quite specific.
			const char PROPERTY_PATH_NODE_DELIMITER = '.';
			const string ARRAY_IN_PROPERTY_DELIMITER = ".Array.data[";
			const string ARRAY_OPEN_DELIMITER = "[";
			const string ARRAY_CLOSE_DELIMITER = "]";

			object obj = property.serializedObject.targetObject;

			// property paths have strange array delimiters, so replace with something sensible.
			string path = property.propertyPath.Replace(ARRAY_IN_PROPERTY_DELIMITER, ARRAY_OPEN_DELIMITER);

			// split the path into all nodes.
			string[] pathNodes = path.Split(PROPERTY_PATH_NODE_DELIMITER);
			foreach (string pathNode in pathNodes) {

				// if the path node contains an array, figure out the index based on the path node and grab the object from that.
				if (pathNode.Contains(ARRAY_OPEN_DELIMITER)) {
					string elementName = pathNode.Substring(0, pathNode.IndexOf(ARRAY_OPEN_DELIMITER, StringComparison.Ordinal));
					int index = Convert.ToInt32(pathNode.Substring(pathNode.IndexOf(ARRAY_OPEN_DELIMITER, StringComparison.Ordinal))
					                                    .Replace(ARRAY_OPEN_DELIMITER, string.Empty)
					                                    .Replace(ARRAY_CLOSE_DELIMITER, string.Empty));
					obj = GetObjectAtIndexOfEnumerableFieldOrProperty(obj, elementName, index);
				}

				// it's just an object, so get field or property with the node.
				else {
					obj = GetFieldOrPropertyWithName(obj, pathNode);
				}
			}
			return obj;
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Gets a field or property with the given name.
		/// </summary>
		/// <param name="source">The object to get the property or field of.</param>
		/// <param name="name">The name of the property or field.</param>
		/// <returns></returns>
		private static object GetFieldOrPropertyWithName(object source, string name) {
			if (source == null) return null;

			Type type = source.GetType();
			while (type != null) {
				FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (field != null) {
					return field.GetValue(source);
				}
				PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (property != null) {
					return property.GetValue(source, null);
				}
				type = type.BaseType;
			}
			return null;
		}

		/// <summary>
		/// Gets an object at a given index of an enumerable field or property with a given name.
		/// </summary>
		/// <param name="source">The source object who has a field or property who is enumerable.</param>
		/// <param name="name">The name of the enumerable field or property.</param>
		/// <param name="index">The index of the object in the enumerable field or property.</param>
		/// <returns></returns>
		private static object GetObjectAtIndexOfEnumerableFieldOrProperty(object source, string name, int index) {
			// ReSharper disable once UseNegatedPatternMatching
			IEnumerable enumerable = GetFieldOrPropertyWithName(source, name) as IEnumerable;
			if (enumerable == null) return null;
			IEnumerator enumerator = enumerable.GetEnumerator();
			for (int i = 0; i <= index; i++) {
				if (!enumerator.MoveNext()) return null;
			}
			return enumerator.Current;
		}
	}
}