using System;
using UnityEngine;

public class MainGuiController : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action PlayGameRequested;
	public event Action ExitGameRequested;

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

	public void ExitGameButton_Clicked() {
		ExitGameRequested?.Invoke();
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