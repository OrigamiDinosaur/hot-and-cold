using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Apache.Core.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Apache.Core.Editor {

	/// <summary>
	/// Defines Apache menu entries and provides general convenience methods and tools for working in the editor.
	/// </summary>
	public static class ApacheEditorTools {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		public const string MENU_ITEM_LOCATION = "Apache";
		public const int MENU_ITEM_STARTING_PRIORITY = 0;
		
		public const int SCENE_LOADING_MENU_ITEM_STARTING_PRIORITY = 77;
		public const int CG_MENU_ITEM_STARTING_PRIORITY = SCENE_LOADING_MENU_ITEM_STARTING_PRIORITY + 22;

		public const string GAME_OBJECT_NAME = "Game Object";
		public const string GROUP_GAME_OBJECT_NAME = "Group";

		public const string DIVIDER_GAME_OBJECT_NAME = "--------------------";

		public const string EDITOR_ONLY_TAG = "EditorOnly";

		// N.B. this regex includes a capture group on the name and the number, in case we want to use that for sorting.
		// N.B. the first group's contents is lazy so that it doesn't gobble up the first occurrence of '(', '[', or '{', or other whitespace.
		private static readonly Regex NAME_AND_NUMBER_SUFFIX_REGEX = new Regex(@"^(.*?)[\(\[\{\<]*\s*#?\s*([0-9]+)\s*[\)\]\}\>]*\s*$");

		// N.B. this regex includes a capture group only on the name.
		// N.B. the first group's contents is lazy so that it doesn't gobble up the first occurrence of '(', '[', or '{', or other whitespace.
		private static readonly Regex NAME_WITH_OPTIONAL_NUMBER_SUFFIX_REGEX = new Regex(@"^(.*?)[\(\[\{\<]*\s*#?\s*[0-9]*\s*[\)\]\}\>]*\s*$");

		private const string RENAME_BY_ORDER_NAME_SUFFIX_FORMAT = " ({0})";

		//-----------------------------------------------------------------------------------------
		// Editor Methods:
		//-----------------------------------------------------------------------------------------

		// create methods.

		[MenuItem(MENU_ITEM_LOCATION + "/Create GameObject %g", false, MENU_ITEM_STARTING_PRIORITY)]
		public static void CreateGameObject() {

			// determine if we intend to group based on whether we currently have multiple selected game objects.
			bool isGrouping = (Selection.objects.Length >= 2 && Selection.objects.All((o) => o is GameObject));
			List<GameObject> selectedGameObjects = null;
			if (isGrouping) {
				selectedGameObjects = Selection.objects.Convert((o) => o as GameObject).ToList();
			}

			// create a new undo group for grouping if necessary.
			const string GROUP_UNDO_GROUP_NAME = "Group Objects";
			int groupUndoGroupId = 0;
			if (isGrouping) {
				Undo.SetCurrentGroupName(GROUP_UNDO_GROUP_NAME);
				groupUndoGroupId = Undo.GetCurrentGroup();
			}

			// create the game object with the default name and place it at the origin.
			GameObject gameObject = new GameObject { name = (isGrouping) ? GROUP_GAME_OBJECT_NAME : GAME_OBJECT_NAME };
			Originise(gameObject);

			// if we're not grouping, start a rename and just record a standard creation and return.
			if (!isGrouping) {
				EditorRenameUtils.Rename(gameObject);
				Undo.RegisterCreatedObjectUndo(gameObject, "Create GameObject");
				return;
			}
			
			// create recursive lambda which determines if a transform has a parent in a list.
			// ReSharper disable once ConvertToLocalFunction
			Func<Transform, List<GameObject>, bool> hasParentInList = null;
			// ReSharper disable once ImplicitlyCapturedClosure
			hasParentInList = (transform, transforms) => {
				if (transform.parent == null) return false;
				foreach (GameObject potentialParent in transforms) {
					if (transform.parent == potentialParent.transform) {
						return true;
					}
				}
				return hasParentInList(transform.parent, transforms);
			};

			// sanitise the list of selected objects, removing any which are children of existing selections.
			selectedGameObjects.RemoveAll((g) => hasParentInList(g.transform, selectedGameObjects));

			// sort the selected game objects by their sibling indices taking into account ancestral sibling indices.
			selectedGameObjects.Sort(new GameObjectSiblingIndexSort());

			// we're grouping, so register the object with the group undo group.
			Undo.RegisterCreatedObjectUndo(gameObject, GROUP_UNDO_GROUP_NAME);

			// before parenting, grab the parent of the selected game object with the highest sort order, as we'll parent in context to that.
			Transform contextParent = selectedGameObjects[0].transform.parent;
			int contextSiblingIndex = selectedGameObjects[0].transform.GetSiblingIndex();

			// parent all of the previously selected objects under the new one.
			foreach (GameObject selectedGameObject in selectedGameObjects) {
				Undo.SetTransformParent(selectedGameObject.transform, gameObject.transform, GROUP_UNDO_GROUP_NAME);
			}

			// use reflection to expand the group object to show its new children.
			Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
			MethodInfo methodInfo = type.GetMethod("ExpandTreeViewItem", BindingFlag.INSTANCE_NON_PUBLIC);
			if (methodInfo != null) {

				// select the hierarchy window and invoke the expand method on it.
				EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
				EditorWindow window = EditorWindow.focusedWindow;
				methodInfo.Invoke(window, new object[] { gameObject.GetEntityId(), true });
			}

			// parent the new game object to the parent context and set its sibling index.
			Undo.SetTransformParent(gameObject.transform, contextParent, GROUP_UNDO_GROUP_NAME);
			Undo.RecordObject(gameObject.transform, GROUP_UNDO_GROUP_NAME);
			gameObject.transform.SetSiblingIndex(contextSiblingIndex);

			// start a rename of the group game object.
			EditorRenameUtils.Rename(gameObject);

			// collapse the group undo group down into a single operation.
			Undo.CollapseUndoOperations(groupUndoGroupId);
		}

		[MenuItem(MENU_ITEM_LOCATION + "/Create GameObject in Context %&g", false, MENU_ITEM_STARTING_PRIORITY)]
		public static void CreateGameObjectInContext() {
			GameObject gameObject = new GameObject { name = GAME_OBJECT_NAME };
			ParentToSelection(gameObject);
			Originise(gameObject);
			EditorRenameUtils.Rename(gameObject);
			Undo.RegisterCreatedObjectUndo(gameObject, "Create GameObject in Context");
		}

		[MenuItem(MENU_ITEM_LOCATION + "/Create Divider in Root %#d", false, MENU_ITEM_STARTING_PRIORITY)]
		public static void CreateDividerInRoot() {

			// create the game object and place it at the origin, then apply editor only tag.
			GameObject gameObject = new GameObject { name = DIVIDER_GAME_OBJECT_NAME };
			Originise(gameObject);
			gameObject.tag = EDITOR_ONLY_TAG;

			// select the object.
			Selection.activeGameObject = gameObject;

			// register object creation with undo.
			Undo.RegisterCreatedObjectUndo(gameObject, "Create Divider in Root");
		}

		// special functionality.

		/// <summary>Renames a list of objects based on the first object selected.</summary>
		[MenuItem(MENU_ITEM_LOCATION + "/Rename Objects by Order %&r", false, MENU_ITEM_STARTING_PRIORITY + 33)]
		public static void RenameObjectsByOrder() {

			// grab all selected objects, backing out if they're empty.
			Transform[] selectedTransforms = Selection.transforms;
			if (selectedTransforms.IsNullOrEmpty()) return;

			// create and populate a list to be sorted into sibling index.
			List<Transform> sortedBySiblingIndex = selectedTransforms.ToList();

			// sort our list by sibling index.
			sortedBySiblingIndex.Sort((t1, t2) => t1.GetSiblingIndex().CompareTo(t2.GetSiblingIndex()));
			
			// grab our base name from the first (highest) item we have selected and trim whitespace from beginning and end.
			string baseName = sortedBySiblingIndex[0].name.Trim();

			// match on the name with an optional number.
			Match baseNameMatch = NAME_WITH_OPTIONAL_NUMBER_SUFFIX_REGEX.Match(baseName);
			if (baseNameMatch.Success && baseNameMatch.Groups.Count > 0) {
				
				// N.B. the first match group represents the entire string, where as an ID of 1 represents the first capture group.
				const int NAME_MATCH_GROUP_ID = 1;

				// set base name to be everything except the numbers on the end, trimmed.
				baseName = baseNameMatch.Groups[NAME_MATCH_GROUP_ID].Value.Trim();
			}

			// create an undo group so that all of our actions are linked undo one undo order. 
			const string GROUP_UNDO_GROUP_NAME = "Rename Objects by Order";
			Undo.SetCurrentGroupName(GROUP_UNDO_GROUP_NAME);
			int groupUndoGroupId = Undo.GetCurrentGroup();

			// foreach sorted item in our selection...
			for (int i = 0; i < sortedBySiblingIndex.Count; i++) {

				// register an undo event and rename the item based on our base name, our index, and our index format.
				Undo.RegisterCompleteObjectUndo(sortedBySiblingIndex[i].gameObject, GROUP_UNDO_GROUP_NAME);
				sortedBySiblingIndex[i].gameObject.name = baseName + string.Format(RENAME_BY_ORDER_NAME_SUFFIX_FORMAT, (i + 1));
			}

			// collapse our undo group to create our undo object. 
			Undo.CollapseUndoOperations(groupUndoGroupId);
		}

		[MenuItem(MENU_ITEM_LOCATION + "/Order Alphabetically %&o", false, MENU_ITEM_STARTING_PRIORITY + 33)]
		public static void OrderAlphabetically() {

			// grab all selected objects, backing out if they're empty.
			Transform[] selectedTransforms = Selection.transforms;
			if (selectedTransforms.IsNullOrEmpty()) return;

			// sort them by our sorting function as a list.
			List<Transform> sortedByAlphabet = selectedTransforms.ToList();
			sortedByAlphabet.Sort(OrderAlphabeticallyWithNumberSuffixSort);

			// find the lowest sibling index, which will use as a base value and add to later.
			int lowestSiblingIndex = int.MaxValue;
			foreach (Transform sorted in sortedByAlphabet) {
				if (sorted.GetSiblingIndex() < lowestSiblingIndex) {
					lowestSiblingIndex = sorted.GetSiblingIndex();
				}
			}

			// create an undo group so that all of our actions are linked undo one undo order. 
			const string GROUP_UNDO_GROUP_NAME = "Order Alphabetically";
			Undo.SetCurrentGroupName(GROUP_UNDO_GROUP_NAME);
			int groupUndoGroupId = Undo.GetCurrentGroup();

			// iterate through transforms and set their sibling index to lowest plus an incrememnt.
			for (int i = 0; i < sortedByAlphabet.Count; i++) {
				Undo.SetTransformParent(sortedByAlphabet[i], sortedByAlphabet[i].parent, GROUP_UNDO_GROUP_NAME);
				sortedByAlphabet[i].SetSiblingIndex(lowestSiblingIndex + i);
			}

			// collapse our undo group to create our undo object. 
			Undo.CollapseUndoOperations(groupUndoGroupId);
		}

		// window shortcuts.

		[MenuItem(MENU_ITEM_LOCATION + "/Open Animator &a", false, MENU_ITEM_STARTING_PRIORITY + 44)]
		public static void OpenAnimator() {
			EditorApplication.ExecuteMenuItem("Window/Animation/Animator");
		}

		[MenuItem(MENU_ITEM_LOCATION + "/Open Lighting &l", false, MENU_ITEM_STARTING_PRIORITY + 44)]
		public static void OpenLighting() {
			EditorApplication.ExecuteMenuItem("Window/Rendering/Lighting");
		}

		[MenuItem(MENU_ITEM_LOCATION + "/Open Player Settings #p", false, MENU_ITEM_STARTING_PRIORITY + 44)]
		public static void OpenPlayerSettings() {
			SettingsService.OpenProjectSettings("Project/Player");
		}

		[MenuItem(MENU_ITEM_LOCATION + "/Open Profiler #r", false, MENU_ITEM_STARTING_PRIORITY + 44)]
		public static void OpenProfiler() {
			EditorApplication.ExecuteMenuItem("Window/Analysis/Profiler");
		}

		[MenuItem(MENU_ITEM_LOCATION + "/Open Timeline &t", false, MENU_ITEM_STARTING_PRIORITY + 44)]
		public static void OpenTimeline() {
			EditorApplication.ExecuteMenuItem("Window/Sequencing/Timeline");

			// look in the scene for enabled timelines, backing out if there are none or more than one.
			PlayableDirector[] directors = Object.FindObjectsByType<PlayableDirector>();
			if (directors == null || directors.Length != 1) return;

			// select the game object of the first and only director.
			Selection.activeGameObject = directors[0].gameObject;
		}
		
		// settings shortcuts.
		
		[MenuItem(MENU_ITEM_LOCATION + "/Sync and Open Settings &s", false, MENU_ITEM_STARTING_PRIORITY + 66)]
		public static void SyncAndOpenSettings() {

			// reload settings.
			// N.B. this will populate the objects with default values if they don't aready exist.
			Settings.ReloadAllSettings();

			// N.B. some objects, such as strings, will naturally default to null, so it's convenient to fix those with empty strings to
			// prevent null refs.

			// get settings properties which are not serialised elements; we want to work with settings objects themselves.
			PropertyInfo[] settingsObjectsProperties = typeof(Settings).GetProperties(BindingFlag.STATIC_PUBLIC)
				.Where(p => !(p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(SerialisedElement<>)))
				.ToArray();

			// loop through each settings object's properties looking for null strings.
			foreach (PropertyInfo settingsObjectProperty in settingsObjectsProperties) {

				// get each public instance property of the settings object which is a string.
				// N.B. the "instance" we're working on is static, so we pass null as the object to GetValue.
				object settingsObject = settingsObjectProperty.GetValue(null);
				PropertyInfo[] settingsStrings = settingsObject.GetType().GetProperties(BindingFlag.INSTANCE_PUBLIC)
					.Where(p => p.PropertyType == typeof(string))
					.ToArray();

				// set all of the null property values to an empty string.
				foreach (PropertyInfo settingsProperty in settingsStrings) {
					if (settingsProperty.GetValue(settingsObject) != null) continue;
					settingsProperty.SetValue(settingsObject, string.Empty);
				}

				// get non-value-type arrays.
				PropertyInfo[] settingsArrays = settingsObject.GetType().GetProperties(BindingFlag.INSTANCE_PUBLIC)
					.Where(p => p.PropertyType.IsArray && !p.PropertyType.IsValueType)
					.ToArray();

				foreach (PropertyInfo settingsProperty in settingsArrays) {

					// continue if the value is not null.
					if (settingsProperty.GetValue(settingsObject) != null) continue;

					// if it's an array of strings, set to an array with one empty string.
					if (settingsProperty.PropertyType == typeof(string[])) {
						settingsProperty.SetValue(settingsObject, new[] { string.Empty });
						continue;
					}

					// otherwise, it's some kind of other array, so set it to an array of one null item (as opposed to null itself).
					settingsProperty.SetValue(settingsObject, Activator.CreateInstance(settingsProperty.PropertyType, 1));
				}
			}

			// finally save settings, which will write the new values to disk, and open the directory.
			Settings.SaveAllSettings();
			SettingsUtils.OpenSettingsDirectoryInExplorer();
		}
		
		// scenes shortcuts.
		
		[MenuItem(MENU_ITEM_LOCATION + "/Open Main Scene &1", false, SCENE_LOADING_MENU_ITEM_STARTING_PRIORITY)]
		public static void OpenMainScene() {
			OpenScene(Scenes.Main);
		}

		[MenuItem(ApacheEditorTools.MENU_ITEM_LOCATION + "/Open Environment Subscene &#1", false, SCENE_LOADING_MENU_ITEM_STARTING_PRIORITY)]
		public static void OpenEnvironmentSubscene() {
			OpenScene(Subscenes.Environment);
		}

		[MenuItem(MENU_ITEM_LOCATION + "/Load Required Subscenes", false, SCENE_LOADING_MENU_ITEM_STARTING_PRIORITY + 11)]
		public static void LoadRequiredSubscenes() {

			// find scene controller and load subscenes from its required subscenes.
			// N.B. we don't beep because we may be loading a subscene which doesn't have a scene controller.
			SceneControllerBase sceneController = Object.FindAnyObjectByType<SceneControllerBase>();
			if (sceneController == null) return;

			// create a new undo group.
			const string undoGroupName = SubscenesController.SET_LOW_RENDER_PRIORITY_OBJECTS_DISABLED_UNDO_NAME;
			Undo.SetCurrentGroupName(undoGroupName);
			int undoGroupId = Undo.GetCurrentGroup();

			// open subscenes.
			// N.B. this will also take care of disabling super scene low render priority objects.
			SubscenesController.EditorOpenSubscenes(sceneController.RequiredSubscenes);

			// collapse all undo group operations into a single undo.
			Undo.CollapseUndoOperations(undoGroupId);
		}

		[MenuItem(MENU_ITEM_LOCATION + "/Unload Required Subscenes", false, SCENE_LOADING_MENU_ITEM_STARTING_PRIORITY + 11)]
		public static void UnloadRequiredSubScenes() {

			// create a new undo group.
			const string undoGroupName = "Unload Required Subscenes";
			Undo.SetCurrentGroupName(undoGroupName);
			int undoGroupId = Undo.GetCurrentGroup();

			// grab the scene init component so we know which scene to load.
			// N.B. there will (or really rather should be!) only one scene setter.
			SceneInit sceneInit = Object.FindAnyObjectByType<SceneInit>();
			Scenes scene = sceneInit.Scene;

			// open the scene, thereby unloading all others, but ask the user if they want to save first.
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex((int)scene));

			// grab low-render-prority objects (we have to do it in a roundabout way in order to get disabled objects).
			SuperSceneLowRenderPriorityObject[] objects = FindSceneObjectsOfType<SuperSceneLowRenderPriorityObject>();

			// ensure optional render objects are enabled.
			foreach (SuperSceneLowRenderPriorityObject @object in objects) {
				Undo.RecordObject(@object.gameObject, undoGroupName);
				@object.gameObject.SetActive(true);
			}

			// collapse all undo group operations into a single undo.
			Undo.CollapseUndoOperations(undoGroupId);
		}

		[MenuItem(MENU_ITEM_LOCATION + "/CG/Create Asset Folder in Context %#c", false, CG_MENU_ITEM_STARTING_PRIORITY)]
		public static void CreateCgiAssetFolderInContext() {

			// get a relevant path on the selected object.
			string path = null;
			Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
			for (int i = 0; i < selectedObjects.Length; i++) {
				path = AssetDatabase.GetAssetPath(selectedObjects[i]);
				if (File.Exists(path)) {
					path = Path.GetDirectoryName(path);
					break;
				}
			}

			// beep and return if we found nothing.
			if (path == null) {
				EditorApplication.Beep();
				return;
			}

			// create the folder.
			string assetFolderGuid = AssetDatabase.CreateFolder(path, "Asset");
			string assetFolderPath = AssetDatabase.GUIDToAssetPath(assetFolderGuid);

			// rename the folder, then create sub-folders afterwards, prompting a project window update.
			EditorRenameUtils.RenameAsset(assetFolderPath);
			AssetDatabase.CreateFolder(assetFolderPath, "Materials");
			AssetDatabase.CreateFolder(assetFolderPath, "Models");
			AssetDatabase.CreateFolder(assetFolderPath, "Textures");

			// N.B.: we are not recording anything here for undoing, as it is not possible in Unity to undo asset creation.
		}

		[MenuItem(MENU_ITEM_LOCATION + "/CG/Apply Lighting from Scene...", false, CG_MENU_ITEM_STARTING_PRIORITY + 11)]
		public static void ApplyLightingFromSelectedScene() {

			// get a path to the scene by asking for a scene selection, backing out if there's no selection.
			string scenePath = EditorUtility.OpenFilePanel("Select Scene", string.Empty, "unity");
			if (string.IsNullOrEmpty(scenePath)) return;

			// make the scene path relative to the project path.
			scenePath = scenePath.Replace(Application.dataPath + "/", "Assets/");

			// apply the lighting.
			EditorLightingUtils.ApplySceneLightingToActiveScene(scenePath);
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static void ParentToSelection(GameObject gameObject) {
			GameObject selectedGameObject = Selection.activeObject as GameObject;
			if (selectedGameObject != null) {
				gameObject.transform.parent = selectedGameObject.transform;
			}
		}

		public static void Originise(GameObject gameObject) {
			Transform gameObjectTransform = gameObject.transform;
			gameObjectTransform.localPosition = Vector3.zero;
			gameObjectTransform.localEulerAngles = Vector3.zero;
			gameObjectTransform.localScale = Vector3.one;
		}

		public static float Round(float value) {
			float intRoundPadding = (value >= 0) ? 0.5f : -0.5f;
			return (int)(value + intRoundPadding);
		}

		public static Vector3 Round(Vector3 vector3) {
			return new Vector3(Round(vector3.x), Round(vector3.y), Round(vector3.z));
		}
		
		public static void OpenScene(Scenes scene, bool shouldLoadRequiredSubscenes = true) {
			OpenScene((int)scene, shouldLoadRequiredSubscenes);
		}

		public static void OpenScene(Subscenes scene, bool shouldLoadRequiredSubscenes = true) {
			OpenScene((int)scene, shouldLoadRequiredSubscenes);
		}

		public static void OpenScene(int scene, bool shouldLoadRequiredSubscenes = true) {
			OpenSceneAtBuildSettingsIndex(scene);
			if (shouldLoadRequiredSubscenes) {
				LoadRequiredSubscenes();
			}
		}

		public static void OpenSceneAtBuildSettingsIndex(int index) {

			// beep and back out if we don't have any build scenes.
			if (EditorBuildSettings.scenes.Length <= index) {
				EditorApplication.Beep();
				return;
			}

			// get the build settings scene based on the index, beeping and backing out if unfound.
			EditorBuildSettingsScene scene = EditorBuildSettings.scenes[index];
			if (scene == null || !scene.enabled) {
				EditorApplication.Beep();
				return;
			}

			// ask the user if they want to save, and if they don't cancel, open the scene.
			// N.B. if the user chooses "Save" or "Don't Save", the method returns true. It returns false for "Cancel". Not totally intuitive.
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
				EditorSceneManager.OpenScene(scene.path);
			}
		}

		public static Object[] GetSelectedObjectsAndBeepIfNotFound() {

			// get selected objects.
			// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
			Object[] selectedObjects = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab | SelectionMode.Editable);
			// ReSharper restore BitwiseOperatorOnEnumWithoutFlags

			// beep and return if there are no selected objects.
			if (selectedObjects.Length == 0) {
				EditorApplication.Beep();
				return null;
			}

			return selectedObjects;
		}

		public static T[] FindSceneObjectsOfType<T>() where T : Object {

			// return all objects of the type...
			return Resources.FindObjectsOfTypeAll<T>()

			 // where they are not hidden and not persistent (on disk).
			 // N.B. this excludes editor internal objects which we really don't want to be accessing.
			 .Where(@object => (@object.hideFlags == HideFlags.None && !EditorUtility.IsPersistent(@object))).ToArray();
		}

		public static T FindSceneObjectOfType<T>() where T : Object {
			T[] sceneObjects = FindSceneObjectsOfType<T>();
			if (sceneObjects == null || sceneObjects.Length == 0) return null;
			return sceneObjects[0];
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------
		
		private static int OrderAlphabeticallyWithNumberSuffixSort(Component c1, Component c2) {
			return OrderAlphabeticallyWithNumberSuffixSort(c1.name, c2.name);
		}

		private static int OrderAlphabeticallyWithNumberSuffixSort(string s1, string s2) {

			// trim strings so whitespace at the beginning and end of each string is not factored in.
			s1 = s1.Trim();
			s2 = s2.Trim();

			// work out default string sort, which we'll return at numerous places if we don't have regex matches.
			int defaultAlphabeticalSortValue = string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);

			// work out if both transforms have names and number suffixes.
			Match t1Match = NAME_AND_NUMBER_SUFFIX_REGEX.Match(s1);
			Match t2Match = NAME_AND_NUMBER_SUFFIX_REGEX.Match(s2);

			// if we failed to get a match or a group capture, return default alphabetical sort.
			if (!t1Match.Success || !t2Match.Success || t1Match.Groups.Count == 0 || t2Match.Groups.Count == 0)
				return defaultAlphabeticalSortValue;

			// N.B. the first match group represents the entire string, where as an ID of 1 represents the first capture group.
			const int NAME_MATCH_GROUP_ID = 1;

			// if we got a match but the names are not the same (which is the string excluding its number), return default sort.
			string s1Name = t1Match.Groups[NAME_MATCH_GROUP_ID].Value.Trim();
			string s2Name = t2Match.Groups[NAME_MATCH_GROUP_ID].Value.Trim();
			if (s1Name != s2Name) return defaultAlphabeticalSortValue;

			const int NUMBER_MATCH_GROUP_ID = 2;

			// we have a match so try to extract the number capture groups as ints, returning default alphabetical sort if we fail.
			// ReSharper disable InlineOutVariableDeclaration
			int s1Number;
			int s2Number;
			// ReSharper restore InlineOutVariableDeclaration
			if (!int.TryParse(t1Match.Groups[NUMBER_MATCH_GROUP_ID].Value, out s1Number) || !int.TryParse(t2Match.Groups[NUMBER_MATCH_GROUP_ID].Value, out s2Number))
				return defaultAlphabeticalSortValue;

			// we extracted the number, so return a match with corresponding sort value.
			if (s1Number > s2Number) return 1;
			if (s1Number < s2Number) return -1;
			return 0;
		}
		
		//-----------------------------------------------------------------------------------------
		// Classes:
		//-----------------------------------------------------------------------------------------

		public class GameObjectSiblingIndexSort : IComparer<GameObject> {

			public int Compare(GameObject gameObjectA, GameObject gameObjectB) {
				
				// if they're the same, then there's no comparison.
				if (gameObjectA == gameObjectB) return 0;

				// if a is null and b is not, a goes first.
				if (gameObjectA == null) return -1;

				// if b is null and a is not, a goes after.
				if (gameObjectB == null) return 1;

				// grab transform references, using short aliases.
				Transform a = gameObjectA.transform;
				Transform b = gameObjectB.transform;

				// if b is a child of a, a goes first.
				if (b.IsChildOf(a)) return -1;

				// if a is a child of b, a goes after.
				if (a.IsChildOf(b)) return 1;

				// get lists of a and b's parents.
				List<Transform> aParents = GetParents(a);
				List<Transform> bParents = GetParents(b);

				// for all of a's parents...
				for (int aIndex = 0; aIndex < aParents.Count; aIndex++) {

					// if b is a child of any of a's parents, work out the index based on the shared parents of a and b.
					if (b.IsChildOf(aParents[aIndex])) {
						int bIndex = bParents.IndexOf(aParents[aIndex]) - 1;
						--aIndex;
						return aParents[aIndex].GetSiblingIndex() - bParents[bIndex].GetSiblingIndex();
					}
				}

				// otherwise it's just the difference between the last parent (i.e. most top-level ancestor) of a and b.
				return aParents[aParents.Count - 1].GetSiblingIndex() - bParents[bParents.Count - 1].GetSiblingIndex();
			}

			private static List<Transform> GetParents(Transform transform) {

				// make a list for collecting parents.
				List<Transform> parents = new List<Transform> { transform };

				// while our transform parent is not the root, keep adding transforms and then do the same on the parent recursively.
				while (transform.parent != null) {
					parents.Add(transform.parent);
					transform = transform.parent;
				}

				return parents;
			}
		}
	}
}