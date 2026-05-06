using System;
using System.Collections.Generic;
using System.Linq;
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

	public event IntAction HatPurchaseRequested;
	public event IntAction HatEquipRequested; 

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected TextMeshProUGUI goldValueText;
	[SerializeField] protected TextMeshProUGUI scrapValueText;

	[SerializeField] protected UpgradeLine drillLine;
	[SerializeField] protected UpgradeLine engineLine;

	[SerializeField] protected HatLine hatLinePrototype;
	
	[SerializeField] protected GuiSlidingView selectionSubView;
	[SerializeField] protected GuiSlidingView upgradesSubView;
	[SerializeField] protected GuiSlidingView cosmeticsSubView;

	[Header("Hats")]

	[SerializeField] protected float distanceBetweenHatLines;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private ShopStates shopState = ShopStates.Selection;

	private List<HatLine> hatLines; 

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {
		selectionSubView.TransitionCompleted += SelectionSubView_TransitionCompleted;

		drillLine.UpgradeRequested += DrillLine_UpgradeRequested;
		engineLine.UpgradeRequested += EngineLine_UpgradeRequested;
	}

	protected void OnDisable() {
		selectionSubView.TransitionCompleted -= SelectionSubView_TransitionCompleted;

		drillLine.UpgradeRequested -= DrillLine_UpgradeRequested;
		engineLine.UpgradeRequested -= EngineLine_UpgradeRequested;

		if (hatLines.Count > 0) {

			foreach (HatLine hatLine in hatLines) {
				hatLine.HatPurchaseRequested -= HatLine_HatPurchaseRequested;
				hatLine.HatEquipRequested -= HatLine_HatEquipRequested; 
			}
		}
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers - Inspector:
	//-----------------------------------------------------------------------------------------

	public void BackButton_Clicked() {

		switch (shopState) {
			case ShopStates.Selection:
				SelectionClicked();
				break;
			case ShopStates.Upgrades:
				UpgradesClicked();
				break;
			case ShopStates.Cosmetics:
				CosmeticsClicked();
				break;
		}
	}

	public void UpgradesButton_Clicked() {
		if (shopState != ShopStates.Selection) return;

		// update our shop state. 
		shopState = ShopStates.Upgrades;
		
		// make sure our upgrades view is visible. 
		upgradesSubView.ShowHideView(true); 

		// slide our menus into position. 
		selectionSubView.SlideOffLeft();
		upgradesSubView.SlideOnRight();
	}

	public void CosmeticsButton_Clicked() {
		if (shopState != ShopStates.Selection) return;

		// update our shop state. 
		shopState = ShopStates.Cosmetics;
		
		// make sure our cosmetics view is visible. 
		cosmeticsSubView.ShowHideView(true); 

		// slide our menus into position.
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

	private void DrillLine_UpgradeRequested() {
		DrillUpgradeRequested?.Invoke();
	}

	private void EngineLine_UpgradeRequested() {
		EngineUpgradeRequested?.Invoke();
	}

	private void HatLine_HatPurchaseRequested(int hatId) {
		HatPurchaseRequested?.Invoke(hatId);
	}

	private void HatLine_HatEquipRequested(int hatId) {
		HatEquipRequested?.Invoke(hatId);
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

		// update our shop state. 
		shopState = ShopStates.Selection;

		// reset our views. 
		selectionSubView.ResetState();
		upgradesSubView.ResetState();
		cosmeticsSubView.ResetState();

		// show our views.
		ShowHideView(true);
		selectionSubView.ShowHideView(true);
		
		// slide on our shop. 
		SlideOnRight();
	}

	public void DismissShop() {

		// slide off our shop. 
		SlideOffRight();

		// stop shop interaction. 
		SetInteractable(false);

		// stop our subviews being interactable. 
		selectionSubView.SetInteractable(false);
		upgradesSubView.SetInteractable(false);
		cosmeticsSubView.SetInteractable(false);

		// hide our subviews so they aren't coveringt the screen. 
		upgradesSubView.ShowHideView(false);
		cosmeticsSubView.ShowHideView(false); 
	}

	public void SetDrillStats(int level, int goldCost, int scrapCost, bool canAffordCost) {

		drillLine.SetUpgradeLevel(level);
		drillLine.SetUpgradeCost(goldCost, scrapCost);
		drillLine.SetCanAffordCost(canAffordCost);
	}

	public void SetEngineStats(int level, int goldCost, int scrapCost, bool canAffordCost) {

		engineLine.SetUpgradeLevel(level);
		engineLine.SetUpgradeCost(goldCost, scrapCost);
		engineLine.SetCanAffordCost(canAffordCost);
	}

	public void CreateHatLines(HatAsset[] hatAssets) {

		// get our default anchored position. 
		Vector2 baseAnchoredPosition = hatLinePrototype.GetComponent<RectTransform>().anchoredPosition;

		// init our hat lines list. 
		hatLines = new List<HatLine>();

		// iterate over our hat assets. 
		for (int i = 0; i < hatAssets.Length; i++) {

			// get our new anchored position. 
			Vector2 anchoredPosition = baseAnchoredPosition;
			anchoredPosition.y -= i * distanceBetweenHatLines;

			// create our new hatline and position it. 
			HatLine hatLine = Instantiate(hatLinePrototype, hatLinePrototype.transform.position, hatLinePrototype.transform.rotation, hatLinePrototype.transform.parent);
			hatLine.SetAnchoredPosition(anchoredPosition);

			// init its data. 
			hatLine.SetHatId(hatAssets[i].HatId);
			hatLine.SetHatName(hatAssets[i].HatName);
			hatLine.SetCost(hatAssets[i].GoldCost, hatAssets[i].ScrapCost);
			hatLine.SetDescription(hatAssets[i].HatDescription);

			// show the object. we do this before the next step due to event timings. 
			hatLine.gameObject.SetActive(true);

			// set whether we can afford this hat. 
			hatLine.SetCanAffordCost(GameState.GameData.PlayerGold >= hatAssets[i].GoldCost && GameState.GameData.PlayerScrap >= hatAssets[i].ScrapCost);

			// subscribe to its events. 
			hatLine.HatPurchaseRequested += HatLine_HatPurchaseRequested;
			hatLine.HatEquipRequested += HatLine_HatEquipRequested; 

			// add this line to our list. 
			hatLines.Add(hatLine); 
		}
	}

	public void UpdateCanAffordHats(int hatId, bool canAffordHat) {

		// iterate through our hats and set whether they can be afforded. 
		foreach (HatLine hatLine in hatLines) {
			if (hatLine.HatId == hatId) {
				hatLine.SetCanAffordCost(canAffordHat); 
			}
		}
	}

	public void UnlockHats(int[] hatIds) {

		// iterate through our hats, and unlocked them if our ids match. 
		foreach (HatLine hatLine in hatLines) {
			if (hatIds.Contains(hatLine.HatId)) {
				hatLine.SetState(HatLine.States.Unequipped); 
			}
		}
	}

	public void SetEquippedHat(int hatId) {

		// check each of our hats. 
		foreach (HatLine hatLine in hatLines) {

			// if their id matched, equip it...
			if (hatLine.HatId == hatId) {

				hatLine.SetState(HatLine.States.Equipped);
			}

			// ... else check if it was equipped to unequip it. 
			else {

				if (hatLine.State == HatLine.States.Equipped) {
					hatLine.SetState(HatLine.States.Unequipped); 
				}
			}
		}
	}

	//-----------------------------------------------------------------------------------------
	// Protected Methods:
	//-----------------------------------------------------------------------------------------

	protected override void OnTransitionCompleted() {

		// if we're in the selection state, set us interactable. 
		if (shopState == ShopStates.Selection) {
			SetInteractable(true);
			selectionSubView.SetInteractable(true);
		}
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void SelectionClicked() {

		DismissShop();
		BackButtonClicked?.Invoke();
	}

	private void UpgradesClicked() {

		// update our shop state.
		shopState = ShopStates.Selection;
		
		// slide our menus into position. 
		selectionSubView.SlideOnLeft();
		upgradesSubView.SlideOffRight();
	}

	private void CosmeticsClicked() {

		// update our shop state.
		shopState = ShopStates.Selection;
		
		// slide our menus into position. 
		selectionSubView.SlideOnRight();
		cosmeticsSubView.SlideOffLeft();
	}
}