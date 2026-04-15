using System;
using Apache.Core.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Apache.Core {
	public class ComponentRefAttribute : RefAttribute {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public bool IsRequired { get; set; }

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public ComponentRefAttribute() { }

		public ComponentRefAttribute(bool isRequired) {
			IsRequired = isRequired;
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected override Object GetObjectFromObject(object @object, Type type) {
			Object[] objects = GetObjectsFromObject(@object, type);
			return (!objects.IsNullOrEmpty()) ? objects[0] : null;
		}

		protected override Object[] GetObjectsFromObject(object @object, Type type) {

			// if we're dealing with game objects, get transforms.
			bool isGameObjectRef = (type == typeof(GameObject));
			if (isGameObjectRef) {
				type = typeof(Transform);
			}

			Component[] components = ((Component)@object).GetComponents(type);

			// if we couldn't find the component and it's required, add it.
			// N.B. it's impossible we'd ever not have a game object or a transform.
			if (components.IsNullOrEmpty() && IsRequired) {
				components = new[] { ((Component)@object).gameObject.AddComponent(type) };
			}

			// convert components into objects (if we have them), otherwise return null.
			return (!components.IsNullOrEmpty()) ? ConvertComponentsToObjects(components, isGameObjectRef) : null;
		}
	}
}