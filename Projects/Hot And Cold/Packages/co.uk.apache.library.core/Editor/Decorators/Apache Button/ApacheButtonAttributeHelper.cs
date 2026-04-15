using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Apache.Core.Editor {
	public class ApacheButtonAttributeHelper {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private static readonly object[] EMPTY_PARAMS = new object[0];

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private IList<MethodInfo> methods = new List<MethodInfo>();
		private Object targetObject;

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public void Init(Object aTargetObject) {
			targetObject = aTargetObject;
			methods =
				 targetObject.GetType()
								 .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
								 .Where(m =>
									  m.GetCustomAttribute(typeof(ApacheButtonAttribute), false) != null &&
									  m.GetParameters().Length == 0 &&
									  !m.ContainsGenericParameters
								 ).ToList();
		}

		public void DrawButtons() {
			if (methods.Count <= 0) return;
			EditorGUILayout.Space();
			ShowMethodButtons();
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void ShowMethodButtons() {
			foreach (MethodInfo method in methods) {
				
				// grab the Apache button attribute, continuing if we can't find it for whatever reason.
				Attribute apacheButtonAttribute = method.GetCustomAttribute(typeof(ApacheButtonAttribute), false);
				if (apacheButtonAttribute == null) continue;

				ApacheButtonAttribute attribute = (ApacheButtonAttribute)apacheButtonAttribute;

				// if we're only showing in play mode and this is not play mode, continue.
				if (attribute.ShouldOnlyShowInPlayMode && !Application.isPlaying) continue;

				// if we have a label for the Apache button, use that, otherwise just display the method name nicely.
				string buttonText = ((ApacheButtonAttribute)apacheButtonAttribute).Label ?? ObjectNames.NicifyVariableName(method.Name);
				if (GUILayout.Button(buttonText)) {
					method.Invoke(targetObject, EMPTY_PARAMS);
				}
			}
		}
	}
}