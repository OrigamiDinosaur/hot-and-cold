using System;
using Apache.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class MainGuiController : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action PlayGameRequested;

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected GameObject mainView;
	[SerializeField] protected Image logoImage;
	[SerializeField] protected AssetReferenceSprite logoAssetReference;
	[SerializeField] protected LoadingScreen loadingScreen; 

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private AsyncOperationHandle<Sprite> logoLoadOpHandle;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Start() {

		if (logoAssetReference.RuntimeKeyIsValid()) {

			logoLoadOpHandle = Addressables.LoadAssetAsync<Sprite>(logoAssetReference);
			logoLoadOpHandle.Completed += LogoLoadOpHandle_Completed; 
		}
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void LogoLoadOpHandle_Completed(AsyncOperationHandle<Sprite> opHandle) {

		if (opHandle.Status == AsyncOperationStatus.Succeeded) {
			logoImage.sprite = opHandle.Result; 
		}
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers - Inspector:
	//-----------------------------------------------------------------------------------------

	public void PlayGameButton_Clicked() {
		PlayGameRequested?.Invoke();
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void ShowLoadingScreen() {

		// disable our main view. 
		mainView.SetActive(false);

		// show our loading screen. 
		loadingScreen.gameObject.SetActive(true); 
	}

	public void UpdateLoadingProgression(float normalizedProgression) {
		loadingScreen.UpdateProgress(normalizedProgression);
	}
}