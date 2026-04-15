using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apache.Core.Extensions;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Apache.Core {

	/// <summary>
	/// Top-level, static controller class for configuring, loading, and generally interacting with all subscenes.
	/// </summary>
	public static class SubscenesController {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		public const string SET_LOW_RENDER_PRIORITY_OBJECTS_DISABLED_UNDO_NAME = "Set Low Render Priority Objects Disabled";

		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public static event Action SubscenesInited;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static bool isLoading;

		private static Subscenes[] subscenes;
		private static bool[] haveSubscenesLoaded;
		
		private static MonoBehaviour coroutineMonoBehaviour;
		private static Action onLoadAction;
		private static bool shouldLoadAsync;

		// we won't ever set this in the build, so we need to disable any unset warnings.
	#pragma warning disable 649
		private static bool isLoadingViaEditorScript;
	#pragma warning restore 649

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public static void LoadSubscenes(IList<Subscenes> aSubscenes, MonoBehaviour aCoroutineMonoBehaviour, Action anOnLoadAction = null, bool aShouldLoadAsync = false) {

			// ensure we're not already loading, as something is very wrong if we are (e.g. multiple main scenes open).
			Assert.IsFalse(isLoading);

			// keep track of relevant arguments as field values.
			if (!aSubscenes.IsNullOrEmpty()) {
				subscenes = aSubscenes.ToArray();
			}
			coroutineMonoBehaviour = aCoroutineMonoBehaviour;
			onLoadAction           = anOnLoadAction;
			shouldLoadAsync        = aShouldLoadAsync;

			// figure out if we have subscenes to load.
			bool hasSubsceneToLoad = (!subscenes.IsNullOrEmpty());
			if (hasSubsceneToLoad && isLoadingViaEditorScript) {

				// because we might not load some scenes via editor script, make sure there is at least one to load in that case.
				bool hasSubsceneToLoadViaEditorScript = false;
				foreach (Subscenes subscene in subscenes) {
					if (!SubscenesInfo.ShouldLoadInEditor(subscene)) continue;
					hasSubsceneToLoadViaEditorScript = true;
					break;
				}

				// so now, whether we have subscenes to load is whether we have them to load via editor script.
				hasSubsceneToLoad = hasSubsceneToLoadViaEditorScript;
			}

			// if we have no subscenes to load...
			if (!hasSubsceneToLoad) {

				// if we have no coroutine to call on the next frame, call the on load action right away and return.
				if (coroutineMonoBehaviour == null) {
					onLoadAction?.Invoke();
					return;
				}

				// we have a coroutine, so call on load action on the next frame.
				// N.B. calling it on the next frame guarantees that every component's Awake is called, as we may be accessing components on load.
				coroutineMonoBehaviour.StartCoroutine(InvokeOnNextFrame(onLoadAction));
				return;
			}

			// initialise list ready for flagging scene loads.
			haveSubscenesLoaded = new bool[subscenes.Length];

			// before we get started, check if we have any high render priority subscenes to load...
			bool hasHighRenderPrioritySubscene = false;
			foreach (Subscenes subscene in subscenes) {
				if (!SubscenesInfo.IsHighRenderPriority(subscene)) continue;
				if (isLoadingViaEditorScript && !SubscenesInfo.ShouldLoadInEditor(subscene)) continue;
				hasHighRenderPrioritySubscene = true;
				break;
			}

			// if we have a high render priority subscene, we need to do the work now of disabling lower priority render objects.
			// N.B. we have to do this before load, because if we load the scene and it has main camera, we'll receive an info log saying two active audio listeners are in the scene.
			// Unfortunately there's no way to say inside the subscene that we should do something about its main camera (or ours) because in order to access any objects in the scene,
			// and query them, we need to wait for them to load, which necessarily results in the log.
			if (hasHighRenderPrioritySubscene) {

				// if we're loading via editor script start an undo group.
	#if UNITY_EDITOR
				const string undoGroupName = SET_LOW_RENDER_PRIORITY_OBJECTS_DISABLED_UNDO_NAME;
				int undoGroupId = -1;
				if (isLoadingViaEditorScript) {
					Undo.SetCurrentGroupName(undoGroupName);
					undoGroupId = Undo.GetCurrentGroup();
				}
	#endif

				// grab low render priority objects from super scene.
				SuperSceneLowRenderPriorityObject[] lowRenderPriorityObjects = Object.FindObjectsByType<SuperSceneLowRenderPriorityObject>();
				foreach (SuperSceneLowRenderPriorityObject lowRenderPriorityObject in lowRenderPriorityObjects) {

					// in the editor, make an undo item for when we disable the object.
					if (isLoadingViaEditorScript) {
	#if UNITY_EDITOR
						Undo.RecordObject(lowRenderPriorityObject.gameObject, undoGroupName);
	#endif
					}

					lowRenderPriorityObject.gameObject.SetActive(false);
				}

				// collapse all undo group operations into a single undo.
	#if UNITY_EDITOR
				if (isLoadingViaEditorScript) {
					Undo.CollapseUndoOperations(undoGroupId);
				}
	#endif
			}

			// wire up delegate to be notified of scene loads.
			if (isLoadingViaEditorScript) {
	#if UNITY_EDITOR
				EditorSceneManager.sceneOpened += EditorSceneManager_SceneOpened;
	#endif
			}
			else {
				SceneManager.sceneLoaded += SceneManager_SceneLoaded;
			}

			// load additively each subscene.
			foreach (Subscenes subscene in subscenes) {

				// continue if we're loading via editor script and shouldn't load this one.
				if (isLoadingViaEditorScript && !SubscenesInfo.ShouldLoadInEditor(subscene)) continue;

				// get the appropriate scene based on index.
				Scene subsceneScene = SceneManager.GetSceneByBuildIndex((int)subscene);

				// if the scene is valid, we have it being loaded already, so continue.
				// N.B. in this scene's awake, IsValid will return true for the subscene, but isLoaded will return false, as it's presently being loaded.
				if (subsceneScene.IsValid()) continue;

				// load scene, via editor if necessary.
				if (isLoadingViaEditorScript) {
	#if UNITY_EDITOR
					EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex((int)subscene), OpenSceneMode.Additive);
	#endif
				}
				else {
					if (shouldLoadAsync) {

						// set background loading priority low to minimise stuttering then load async additive.
						Application.backgroundLoadingPriority = ThreadPriority.Low;
						SceneManager.LoadSceneAsync((int)subscene, LoadSceneMode.Additive);
					}
					else {
						SceneManager.LoadScene((int)subscene, LoadSceneMode.Additive);
					}
				}

				// grab scene ref again, and if its still not valid, throw exception.
				// N.B. we can't reuse above Scene ref; we do have to grab the scene again before it will tell us that it's valid now.
				subsceneScene = SceneManager.GetSceneByBuildIndex((int)subscene);
				if (!subsceneScene.IsValid()) throw new InvalidOperationException();
			}
		}

		public static void OnSubscenesInited() {
			SubscenesInited?.Invoke();
		}

		// Provides a clearer hook for opening subscenes in the editor.
	#if UNITY_EDITOR
		public static void EditorOpenSubscenes(List<Subscenes> aSubscenes, Action anOnLoadAction = null) {
			isLoadingViaEditorScript = true;
			LoadSubscenes(aSubscenes, null, anOnLoadAction);
		}
	#endif

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		// N.B. event handlers are below public methods in this class because it more sensibly maps on to the logic flow.

		private static void SceneManager_SceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {

			// ignore all scene loads which are not additive, as that is the only way a subscene is loaded.
			if (loadSceneMode != LoadSceneMode.Additive) return;
			
			// locate the loaded subscene from the list of subscenes...
			for (int i = 0; i < subscenes.Length; i++) {
				Subscenes subscene = subscenes[i];

				// if this is the subscene we just loaded, flag.
				if (scene.buildIndex == (int)subscene) {
					haveSubscenesLoaded[i] = true;
				}
			}

			// if we find we still need to load a scene, back out.
			foreach (bool hasSubsceneLoaded in haveSubscenesLoaded) {
				if (!hasSubsceneLoaded) return;
			}

			// unwire loaded/opened delegate.
			if (isLoadingViaEditorScript) {
	#if UNITY_EDITOR
				EditorSceneManager.sceneOpened -= EditorSceneManager_SceneOpened;
	#endif
			}
			else {
				SceneManager.sceneLoaded -= SceneManager_SceneLoaded;
			}

			// if we have a mono behaviour, call the invoke on next frame coroutine (to wait for loading) in order to promote prioritised lighting
			// subscene to main scene and call on load action.
			// N.B. this is annoying but important: the scene is considered loaded, but we can't get at any objects until the next frame.
			if (coroutineMonoBehaviour != null) {
				coroutineMonoBehaviour.StartCoroutine(InvokeOnNextFrame(HandleLoadedScenesAndInvokeOnLoadAction));
				return;
			}

			// in the editor, use editor application update in order to wait for the next frame.
	#if UNITY_EDITOR
			if (isLoadingViaEditorScript) {
				EditorApplication.update += EditorApplication_Update;
			}
	#else

			// if we make it here, we don't have the tools we need to complete the job, so throw invalid op.
			throw new InvalidOperationException();
	#endif
		}

		// Provide a wrapper for editor open scene events which uses the scene loaded event handler above.
	#if UNITY_EDITOR
		private static void EditorSceneManager_SceneOpened(Scene scene, OpenSceneMode openSceneMode) {

			// work out the appropriate load scene mode and call the other event handler.
			LoadSceneMode loadSceneMode;
			switch (openSceneMode) {
				case OpenSceneMode.Single:
					loadSceneMode = LoadSceneMode.Single;
					break;
				default:
					loadSceneMode = LoadSceneMode.Additive;
					break;
			}

			SceneManager_SceneLoaded(scene, loadSceneMode);
		}
	#endif

	#if UNITY_EDITOR
		private static void EditorApplication_Update() {

			// unwire delegate as we no longer need it.
			// ReSharper disable once DelegateSubtraction
			EditorApplication.update -= EditorApplication_Update;

			HandleLoadedScenesAndInvokeOnLoadAction();
		}
	#endif

		//-----------------------------------------------------------------------------------------
		// Coroutines:
		//-----------------------------------------------------------------------------------------

		// N.B. we use a coroutine to wait for the next frame as opposed to an Apache Sequence because it is a common pattern to change state
		// on Start and thereby cause a sequence cancellation. Because this load code is always called on Awake and may well continue running
		// into its Start, it's too risky to use a sequence and potentially cause a difficult-to-track-down buggette.
		private static IEnumerator InvokeOnNextFrame(Action action) {
			yield return Wait.EndOfFrame;
			action?.Invoke();
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private static void HandleLoadedScenesAndInvokeOnLoadAction() {
			
			// if we're not loading via an editor script...
			if (!isLoadingViaEditorScript) {

				// for the first subscene we encounter with priority rendering enabled (if any), promote it to main (active) scene.
				foreach (Subscenes subscene in subscenes) {

					// continue if we're not high render priority; the below work is all about that.
					if (!SubscenesInfo.IsHighRenderPriority(subscene)) continue;

					// if the scene is invalid, log an error, as this likely means the subscene value isn't synced up with what we actually loaded.
					Scene subsceneScene = SceneManager.GetSceneByBuildIndex((int)subscene);
					if (!subsceneScene.IsValid()) {
						Debug.LogError(
							"Failed to promote loaded scene to active scene. Is the subscene value on the Subscene Controller set to the correct value? " +
							"It is currently \"" + subscene + "\".");
						return;
					}

					// finally set it the active scene, which will result in Unity using its lightmaps.
					SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)subscene));

					break;
				}
			}

			// find all subscene controllers and initialise them as such.
			SubsceneControllerBase[] subsceneControllers = Object.FindObjectsByType<SubsceneControllerBase>();
			foreach (SubsceneControllerBase subsceneController in subsceneControllers) {
				subsceneController.InitAsSubscene();
			}

			// clean up remaining state.
			isLoading = false;
			subscenes = null;
			isLoadingViaEditorScript = false;
			shouldLoadAsync = false;

			// invoke action if we have one.
			onLoadAction?.Invoke();
			onLoadAction = null;
		}
	}
}