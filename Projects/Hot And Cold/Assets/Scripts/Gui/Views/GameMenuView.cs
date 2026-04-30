using System;
using UnityEngine;

public class GameMenuView : GuiSlidingView {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action PlayButtonClicked;
	public event Action ShopButtonClicked;
	public event Action ExitButtonClicked;

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]
	
	[SerializeField] protected OriButton[] buttons; 
	
	//-----------------------------------------------------------------------------------------
	// Event Handlers - Inspector:
	//-----------------------------------------------------------------------------------------

	public void PlayButton_Clicked() {
		SetButtonsEnabled(false);

		PlayButtonClicked?.Invoke();
	}

	public void ShopButton_Clicked() {
		SetButtonsEnabled(false);

		ShopButtonClicked?.Invoke();
	}

	public void ExitButton_Clicked() {
		SetButtonsEnabled(false);

		ExitButtonClicked?.Invoke();
	}
	
	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SetButtonsEnabled(bool isEnabled) {

		foreach (OriButton button in buttons) {
			button.enabled = isEnabled;
		}
	}
}