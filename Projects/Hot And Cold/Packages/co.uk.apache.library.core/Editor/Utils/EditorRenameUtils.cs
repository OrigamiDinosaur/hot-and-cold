using UnityEditor;
using UnityEngine;

namespace Apache.Core.Editor {

	/// <summary>Editor utility class for sending rename events.</summary>
	public static class EditorRenameUtils {

		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		private enum Windows {
			Hierarchy,
			Project
		}

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string HIERARCHY_WINDOW_MENU_ITEM_PATH = "Window/General/Hierarchy";
		private const string PROJECT_WINDOW_MENU_ITEM_PATH = "Window/General/Project";

		private static readonly Event RENAME_EVENT = new Event { keyCode = KeyCode.F2, type = EventType.KeyDown };

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static Windows currentWindow;

		private static bool hasFocussed;

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		private static void EditorApplication_Update() {

			if (!hasFocussed) {

				// determine the path to the relevant window menu item, then execute it.
				string windowMenuItemPath = null;
				switch (currentWindow) {
					case Windows.Hierarchy:
						windowMenuItemPath = HIERARCHY_WINDOW_MENU_ITEM_PATH;
						break;
					case Windows.Project:
						windowMenuItemPath = PROJECT_WINDOW_MENU_ITEM_PATH;
						break;
				}
				EditorApplication.ExecuteMenuItem(windowMenuItemPath);

				// flag we're focussed and return for the next frame.
				hasFocussed = true;
				return;
			}

			// rename by sending an F2 key press.
			EditorWindow.focusedWindow.SendEvent(RENAME_EVENT);

			// ensure we no longer receive update notifications.
			// ReSharper disable once DelegateSubtraction
			EditorApplication.update -= EditorApplication_Update;
		}

		private static void EditorApplication_WindowChanged() {

			// start receiving update notifications.
			EditorApplication.update += EditorApplication_Update;

			// ensure we no longer receive window changed notifications.
			// ReSharper disable DelegateSubtraction
			switch (currentWindow) {
				case Windows.Hierarchy:
					EditorApplication.hierarchyChanged -= EditorApplication_WindowChanged;
					break;
				case Windows.Project:
					EditorApplication.projectChanged -= EditorApplication_WindowChanged;
					break;
			}
			// ReSharper restore DelegateSubtraction
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static void Rename(GameObject gameObject) {
			Selection.activeGameObject = gameObject;
			RenameSelectedInHierarchy();
		}

		public static void RenameAsset(string path) {
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
			RenameSelectedInProject();
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private static void RenameSelectedInHierarchy() {
			currentWindow = Windows.Hierarchy;
			hasFocussed = false;
			EditorApplication.hierarchyChanged += EditorApplication_WindowChanged;
		}

		private static void RenameSelectedInProject() {
			currentWindow = Windows.Project;
			hasFocussed = false;
			EditorApplication.projectChanged += EditorApplication_WindowChanged;
		}
	}
}