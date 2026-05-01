using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

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
	}

	protected void OnDisable() {
		guiController.PlayGameRequested -= GuiController_PlayGameRequested; 
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void GuiController_PlayGameRequested() {
		guiController.ShowLoadingScreen();

		StartCoroutine(LoadLevel());
	}

	//-----------------------------------------------------------------------------------------
	// Coroutines:
	//-----------------------------------------------------------------------------------------

	private IEnumerator LoadLevel() {

		sceneLoadOpHandle = Addressables.LoadSceneAsync(levelToLoad, activateOnLoad: false);

		while (!sceneLoadOpHandle.IsDone) {
			
			guiController.UpdateLoadingProgression(sceneLoadOpHandle.PercentComplete);

			yield return null;
		}

		guiController.UpdateLoadingProgression(1.0f);

		yield return new WaitForSeconds(0.1f);

		sceneLoadOpHandle.Result.ActivateAsync();
	}
}