using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainSceneController : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected MainGuiController guiController;

	[SerializeField] protected string levelToLoad;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private AsyncOperationHandle<SceneInstance> sceneLoadOpHandle;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {
		guiController.PlayGameRequested += GuiController_PlayGameRequested;
		guiController.ExitGameRequested += GuiController_ExitGameRequested;
	}

	protected void OnDisable() {
		guiController.PlayGameRequested -= GuiController_PlayGameRequested;
		guiController.ExitGameRequested -= GuiController_ExitGameRequested; 
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void GuiController_PlayGameRequested() {
		guiController.ShowLoadingScreen();

		StartCoroutine(LoadLevel());
	}

	private void GuiController_ExitGameRequested() {
		
#if UNITY_STANDALONE
		Application.Quit();
#endif
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#endif
	}

	//-----------------------------------------------------------------------------------------
	// Coroutines:
	//-----------------------------------------------------------------------------------------

	private IEnumerator LoadLevel() {

		// start loading our level. 
		sceneLoadOpHandle = Addressables.LoadSceneAsync(levelToLoad, activateOnLoad: false);

		// while the scene is loading, update our percentage bar. 
		while (!sceneLoadOpHandle.IsDone) {
			guiController.UpdateLoadingProgression(sceneLoadOpHandle.PercentComplete);
			yield return null;
		}

		// when we're done make sure the bar fills. 
		// (if the bar loads to quickly we don't get an update to the bar so manually force it).
		guiController.UpdateLoadingProgression(1.0f);

		// give us a momemet after load to make sure we don't get screen flicker.
		yield return new WaitForSeconds(0.1f);

		// finish loading our scene. 
		sceneLoadOpHandle.Result.ActivateAsync();
	}
}