using System;
using Apache.Core;
using Apache.Core.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuView : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action PlayButtonClicked;
	public event Action ShopButtonClicked;
	public event Action ExitButtonClicked;
	public event Action MenuClosed;

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected RectTransform mainMenuRoot;

	[SerializeField] protected OriButton[] buttons; 

	[Header("Transitions")]

	[SerializeField] protected float rootStartingPositionX;
	[SerializeField] protected float rootShopPositionX;
	[SerializeField] protected float rootClosedPositionX;

	[SerializeField] protected float menuCloseDuration; 

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

	public void CloseMenu() {

		sequence.Tween(LeanTween.value(gameObject, SetRootPositionX, rootStartingPositionX, rootClosedPositionX, menuCloseDuration)
			.setEase(LeanTweenType.easeInOutBack)
			.setOnComplete(() => { MenuClosed?.Invoke(); }));
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void SetButtonsEnabled(bool isEnabled) {

		foreach (OriButton button in buttons) {
			button.enabled = isEnabled;
		}
	}

	private void SetRootPositionX(float x) {
		mainMenuRoot.anchoredPosition = mainMenuRoot.anchoredPosition.WithX(x); 
	}
}