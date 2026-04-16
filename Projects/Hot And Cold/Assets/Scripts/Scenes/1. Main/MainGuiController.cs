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
	[SerializeField] protected LoadingScreen loadingScreen;

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