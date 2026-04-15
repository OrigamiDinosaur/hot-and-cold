using System;
using UnityEditor;
using UnityEngine;

namespace Apache.Core {

	public class ValueRefDrawer : PropertyDrawer {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		/// <summary>Options to display in the popup to select constant or variable.</summary>
		private static readonly string[] POPUP_OPTIONS = { "Use Value", "Use Container", "Create Container" };

		private const int POPUP_RIGHT_MARGIN = 2;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private SerializedProperty valueProperty;
		private SerializedProperty shouldUseContainerProperty;
		private SerializedProperty containerProperty;

		//-----------------------------------------------------------------------------------------
		// Editor Methods:
		//-----------------------------------------------------------------------------------------

		protected void OnTypeVariableGUI<T>(Rect position, SerializedProperty property, GUIContent label)
			where T : ScriptableObject {

			Rect totalEditorRect = position;

			// create popup style for option popup
			GUIStyle popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions")) {
				imagePosition = ImagePosition.ImageOnly,
				alignment = TextAnchor.MiddleRight,
				margin = { right = POPUP_RIGHT_MARGIN }
			};

			label = EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, label);

			// once we have our position rect, adjust our total editor rect to match its height, and set our label width
			totalEditorRect.yMin = position.yMin;
			totalEditorRect.yMax = position.yMax;

			EditorGUIUtility.labelWidth = (totalEditorRect.width - (position.width));
			EditorGUI.BeginChangeCheck();

			// Get properties
			valueProperty = property.FindPropertyRelative("value");
			shouldUseContainerProperty = property.FindPropertyRelative("shouldUseContainer");
			containerProperty = property.FindPropertyRelative("container");

			position.xMax -= popupStyle.fixedWidth + popupStyle.margin.right;
			totalEditorRect.xMax -= popupStyle.fixedWidth + popupStyle.margin.right;

			// Calculate rect for configuration button
			Rect buttonRect = new Rect(position) {
				xMin = position.xMax,
				width = popupStyle.fixedWidth + popupStyle.margin.right
			};
			buttonRect.yMin += popupStyle.margin.top;

			// Store old indent level and set it to 0, the PrefixLabel takes care of it
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			if (shouldUseContainerProperty.boolValue) {
				EditorGUI.PropertyField(position, containerProperty, GUIContent.none);
			}
			else {
				// if we are displaying a value rather than a container than create a fake label and reposition the property field such that we have a draggable hotzone.
				EditorGUI.PropertyField(totalEditorRect, valueProperty, new GUIContent(" "));
			}

			// keep track of whether we're current using a container property.
			bool prevShouldUseContainerProperty = shouldUseContainerProperty.boolValue;

			// display a pop-up choosing between pop-up options.
			int result = EditorGUI.Popup(buttonRect, (shouldUseContainerProperty.boolValue) ? 1 : 0, POPUP_OPTIONS, popupStyle);
			switch (result) {
				case 0:
					shouldUseContainerProperty.boolValue = false;
					break;
				case 1:
					shouldUseContainerProperty.boolValue = true;
					break;
				case 2:

					// attempt to create a reference to value container, reinstating previous should use container if failed.
					bool didCreateReferenceToValueContainer = CreateReferenceToValueContainer<T>();
					shouldUseContainerProperty.boolValue = (didCreateReferenceToValueContainer || prevShouldUseContainerProperty);
					break;
			}

			if (EditorGUI.EndChangeCheck())
				property.serializedObject.ApplyModifiedProperties();

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();

			if (shouldUseContainerProperty.boolValue) return;

			shouldUseContainerProperty.boolValue = LinkReferenceOnDragAndDrop<T>(position);
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private bool LinkReferenceOnDragAndDrop<T>(Rect position) where T : ScriptableObject {

			// grab the current event.
			Event evt = Event.current;

			switch (evt.type) {

				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (!position.Contains(evt.mousePosition))
						return false;

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					if (evt.type == EventType.DragPerform) {
						DragAndDrop.AcceptDrag();

						if (DragAndDrop.objectReferences.Length > 1) return false;

						T objectToReference = DragAndDrop.objectReferences[0] as T;

						if (objectToReference != null) {

							containerProperty.objectReferenceValue = objectToReference;

							return true;
						}
					}

					break;
			}

			return false;
		}

		private bool CreateReferenceToValueContainer<T>() where T : ScriptableObject {

			// create a new instance of our scriptable object.
			T newValueContainer = ScriptableObject.CreateInstance<T>();

			// retrieve the name of our scriptable object.
			string valueContainerName = typeof(T).FullName;

			// grab the desired save path.
			string path = EditorUtility.SaveFilePanelInProject(
				"Create " + valueContainerName,
				"",
				"New" + valueContainerName,
				"asset"
			);

			// back out if we don't have a path.
			if (path.Length == 0) return false;

			// back out if the path does not contain application assets.
			if (!path.Contains(Application.dataPath)) {
				Debug.LogError("Path selected is not relative to current project.");
				return false;
			}

			// find the starting point of an Assets file reference.
			int index = path.IndexOf("Assets", StringComparison.Ordinal);
			string cutPath = path.Substring(index);

			// create a new type variable at our target path.
			AssetDatabase.CreateAsset(newValueContainer, cutPath);

			// assign new type variable to our reference property.
			containerProperty.objectReferenceValue = newValueContainer;

			return true;
		}
	}
}