using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Apache.Core.Editor {
	public static class EditorLightingUtils {

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Applies the lighting from a scene (determined by the given name and index) to the currently active scene.
		/// </summary>
		/// <param name="sceneProjectPath">The project-relative path (the directory one level higher than "Assets") of the scene to copy lighting from.</param>
		public static void ApplySceneLightingToActiveScene(string sceneProjectPath) {

			// create a new undo group.
			const string undoGroupName = "Apply Scene Lighting to Active Scene";
			Undo.SetCurrentGroupName(undoGroupName);
			int undoGroupId = Undo.GetCurrentGroup();

			// get the current scene.
			Scene currentScene = SceneManager.GetActiveScene();

			// determine if the scene already loaded.
			Scene sourceScene = SceneManager.GetSceneByPath(sceneProjectPath);
			bool isSourceSceneAlreadyLoaded = sourceScene.IsValid();
			
			// open the source scene additively (if not already loaded).
			if (!isSourceSceneAlreadyLoaded) {
				sourceScene = EditorSceneManager.OpenScene(sceneProjectPath, OpenSceneMode.Additive);
			}

			// make it the active scene so we can get lighting information.
			SceneManager.SetActiveScene(sourceScene);

			// grab the lighting settings from the source scene.
			LightingSettingsSnapshot sourceLightingSettingsSnapshot = new LightingSettingsSnapshot();

			// close the scene (if we opened it) and make the current scene (before we opened the new one) the active scene again.
			if (!isSourceSceneAlreadyLoaded) {
				EditorSceneManager.CloseScene(sourceScene, true);
			}
			SceneManager.SetActiveScene(currentScene);

			// if the source scene was already loaded, null out sun source, as this will result in a cross-scene ref and a warning.
			if (isSourceSceneAlreadyLoaded) {
				sourceLightingSettingsSnapshot.SunSource.Value = null;
			}

			// save the source lighting settings as our current settings.
			sourceLightingSettingsSnapshot.SetCurrentSceneLightingSettingsWithValues();

			// collapse all undo group operations into a single undo.
			Undo.CollapseUndoOperations(undoGroupId);
		}
	}
}