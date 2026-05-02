using System;
using TMPro;
using UnityEngine;

public class ShopMenuView : GuiSlidingView {

	//-----------------------------------------------------------------------------------------
	// Type Definitions:
	//-----------------------------------------------------------------------------------------

	private enum ShopStates {
		Selection,
		Upgrades,
		Cosmetics
	}

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action BackButtonClicked;

	public event Action DrillUpgradeRequested;
	public event Action EngineUpgradeRequested;

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected TextMeshProUGUI goldValueText;
	[SerializeField] protected TextMeshProUGUI scrapValueText;
	
	[SerializeField] protected GuiSlidingView selectionSubView;
	[SerializeField] protected GuiSlidingView upgradesSubView;
	[SerializeField] protected GuiSlidingView cosmeticsSubView;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private ShopStates shopState = ShopStates.Selection;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {
		selectionSubView.TransitionCompleted += SelectionSubView_TransitionCompleted; 
	}

	protected void OnDisable() {
		selectionSubView.TransitionCompleted -= SelectionSubView_TransitionCompleted; 
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers - Inspector:
	//-----------------------------------------------------------------------------------------

	public void BackButton_Clicked() {

		switch (shopState) {
			case ShopStates.Selection:
				DismissShop();
				BackButtonClicked?.Invoke();
				break;
			case ShopStates.Upgrades:

				shopState = ShopStates.Selection;

				upgradesSubView.SetInteractable(false); 

				selectionSubView.SlideOnLeft();
				upgradesSubView.SlideOffRight();

				break;
			case ShopStates.Cosmetics:

				shopState = ShopStates.Selection;

				cosmeticsSubView.SetInteractable(false); 

				selectionSubView.SlideOnRight();
				cosmeticsSubView.SlideOffLeft();
				break;
		}
	}

	public void UpgradesButton_Clicked() {
		if (shopState != ShopStates.Selection) return;

		shopState = ShopStates.Upgrades;

		selectionSubView.SetInteractable(false);
		upgradesSubView.ShowHideView(true); 

		selectionSubView.SlideOffLeft();
		upgradesSubView.SlideOnRight();
	}

	public void CosmeticsButton_Clicked() {
		if (shopState != ShopStates.Selection) return;

		shopState = ShopStates.Cosmetics;

		selectionSubView.SetInteractable(false);
		cosmeticsSubView.ShowHideView(true); 

		selectionSubView.SlideOffRight();
		cosmeticsSubView.SlideOnLeft();
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void SelectionSubView_TransitionCompleted() {

		switch (shopState) {
			case ShopStates.Selection:
				selectionSubView.SetInteractable(true); 
				break;
			case ShopStates.Upgrades:
				upgradesSubView.SetInteractable(true); 
				break;
			case ShopStates.Cosmetics:
				cosmeticsSubView.SetInteractable(true); 
				break;
		}
	}

	//-----------------------------------------------------------------------------------------
	// Getters / Setters:
	//-----------------------------------------------------------------------------------------

	public void SetCurrencyValues(int goldValue, int scrapValue) {

		goldValueText.text = goldValue.ToString();
		scrapValueText.text = scrapValue.ToString();
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void PresentShop() {

		shopState = ShopStates.Selection;

		selectionSubView.ResetState();
		upgradesSubView.ResetState();
		cosmeticsSubView.ResetState();

		ShowHideView(true);
		selectionSubView.ShowHideView(true); 
		SlideOnRight();
	}

	public void DismissShop() {

		SlideOffRight();

		SetInteractable(false);

		selectionSubView.SetInteractable(false);
		upgradesSubView.SetInteractable(false);
		cosmeticsSubView.SetInteractable(false);

		upgradesSubView.ShowHideView(false);
		cosmeticsSubView.ShowHideView(false); 
	}

	//-----------------------------------------------------------------------------------------
	// Protected Methods:
	//-----------------------------------------------------------------------------------------

	protected override void OnTransitionCompleted() {

		switch (shopState) {
			case ShopStates.Selection:

				SetInteractable(true);
				selectionSubView.SetInteractable(true);
				break;
		}
	}
}