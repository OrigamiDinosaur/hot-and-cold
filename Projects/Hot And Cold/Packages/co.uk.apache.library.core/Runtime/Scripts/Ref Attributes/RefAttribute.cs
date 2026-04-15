using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Field)]
	public abstract class RefAttribute : Attribute {

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		private const string AUTO_REF_UNDO_NAME = "Auto Reference";

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public virtual void UpdateRefs(object @object, FieldInfo field) {

			// make sure to record object before referencing to enable undo and to mark the scene dirty.
			// N.B. we don't need to specify a more specific undo name in subclasses because all of our undos are collapsed into one in ApacheComponentEditor.cs.
#if UNITY_EDITOR
			Object unityObject = @object as Object;
			if (unityObject != null) {
				UnityEditor.Undo.RecordObject(unityObject, AUTO_REF_UNDO_NAME);
			}
#endif

			// check if this is an array or collection.
			// N.B. doing it this way instead of IsAssignableFrom avoids issues with things like transforms which implement custom IEnumerables.
			bool isArrayOrCollection = (field.FieldType.GetInterface(typeof(IEnumerable<>).FullName) != null);

			// if it's not an array nor a collection, simply grab the component corresponding to the type in children, backing out if we don't find it.
			if (!isArrayOrCollection) {

				// get the object and set value, which may be null.
				Object refObject = GetObjectFromObject(@object, field.FieldType);
				field.SetValue(@object, refObject);
				return;
			}

			// we're dealing with an array or a collection from here on out...

			// get the type of the elements in this collection (array or generic collection), backing out if we don't have one.
			Type elementType = (field.FieldType.IsArray) ? field.FieldType.GetElementType() : field.FieldType.GetGenericArguments().Single();
			if (elementType == null) return;

			// attempt to grab the components corresponding to the type in children, backing out if we don't find any.
			Object[] refObjects = GetObjectsFromObject(@object, elementType);

			// N.B. refObjects may be null here, and that's fine, as we want to ensure we null out any collection references when we lose those references.

			// if it's an array...
			if (field.FieldType.IsArray) {

				// create an array of the appropriate types, and copy elements to it.
				Array typedArray = null;
				if (refObjects != null && refObjects.Length > 0) {
					typedArray = Array.CreateInstance(elementType, refObjects.Length);
					Array.Copy(refObjects, typedArray, refObjects.Length);
				}

				// finally assign the array of typed objects (which may be null) to the field.
				field.SetValue(@object, typedArray);
				return;
			}

			// it's another kind of enumerable, so it's got to be a generic list collection as that's the only other type serialised in the inspector...

			// create a null list which we'll populate if we have objects.
			IList list = null;
			if (refObjects != null && refObjects.Length > 0) {

				// create a an IList of generic type corresponding to the element type.
				Type listType = typeof(List<>).MakeGenericType(elementType);
				list = (IList)Activator.CreateInstance(listType);

				// add each component to the IList.
				foreach (Object refObject in refObjects) {
					list.Add(refObject);
				}
			}

			// finally assign the IList (which may be null).
			field.SetValue(@object, list);
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected abstract Object GetObjectFromObject(object @object, Type type);

		protected abstract Object[] GetObjectsFromObject(object @object, Type type);

		/// <summary>
		/// Converts a given array of Unity <c>Components</c> into a Unity <c>Object</c> array, also accounting for whether we want
		/// <c>GameObject</c> references from the <c>Component</c>s.</summary>
		protected Object[] ConvertComponentsToObjects(Component[] components, bool shouldGetGameObjectRefs) {
			Object[] objects = new Object[components.Length];
			for (int i = 0; i < components.Length; i++) {
				objects[i] = (shouldGetGameObjectRefs) ? components[i].gameObject : (Object)components[i];
			}
			return objects;
		}
	}
}