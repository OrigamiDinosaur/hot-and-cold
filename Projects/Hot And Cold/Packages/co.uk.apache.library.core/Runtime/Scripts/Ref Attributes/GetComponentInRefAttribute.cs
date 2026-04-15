using System;
using System.Collections.Generic;
using System.Linq;
using Apache.Core.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Apache.Core {
	public abstract class GetComponentInRefAttribute : OrderableRefAttribute {

		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		protected enum Scopes {
			Child,
			Parent
		}

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		protected abstract Scopes Scope { get; }

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

			// grab components in scope, removing components on us, and back out if we don't find any.
			// N.B. see GetComponentsInExcludingSelf for why we want to exclude self.
			Component[] components = GetComponentsInExcludingSelf(@object, type, Scope);
			if (components.IsNullOrEmpty()) return null;

			// convert components to objects, order and return.
			Object[] objects = ConvertComponentsToObjects(components, isGameObjectRef);
			return OrderObjects(objects);
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------
		
		private static Component[] GetComponentsInExcludingSelf(object @object, Type type, Scopes scope) {
			
			// get all the components based on scope.
			Component objectComponent = (Component)@object;
			Component[] components;
			switch (scope) {
				case Scopes.Child:
					components = objectComponent.GetComponentsInChildren(type, true);
					break;
				case Scopes.Parent:
					components = objectComponent.GetComponentsInParent(type, true);
					break;
				default:
					throw new NotImplementedException();
			}

			// back out if we found nothing.
			if (components.IsNullOrEmpty()) return null;

			// grab any components on ourself, so we can remove them from our previously got components.
			// N.B. it's annoying, but we have to do this because GetComponentsIn[Children|Parent] rather questionably also gets components
			// on the object from which we're getting.

			// get components on ourselves, and if we find some...
			Component[] selfComponents = objectComponent.GetComponents(type);
			if (selfComponents != null && selfComponents.Length > 0) {
				foreach (Component component in selfComponents) {

					// remove them from the array (as a list) and convert it back into an array.
					List<Component> componentsList = components.ToList();
					componentsList.Remove(component);
					components = componentsList.ToArray();
				}
			}

			return components;
		}
	}
}