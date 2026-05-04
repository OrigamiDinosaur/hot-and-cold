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

		Vector2 baseAnchoredPosition = hatLinePrototype.GetComponent<RectTransform>().anchoredPosition;

		hatLines = new List<HatLine>();

		for (int i = 0; i < hatAssets.Length; i++) {

			Vector2 anchoredPosition = baseAnchoredPosition;
			anchoredPosition.y -= i * distanceBetweenHatLines;

			HatLine hatLine = Instantiate(hatLinePrototype, hatLinePrototype.transform.position, hatLinePrototype.transform.rotation, hatLinePrototype.transform.parent);
			hatLine.SetAnchoredPosition(anchoredPosition);

			hatLine.SetHatId(hatAssets[i].HatId);
			hatLine.SetHatName(hatAssets[i].HatName);
			hatLine.SetCost(hatAssets[i].GoldCost, hatAssets[i].ScrapCost);
			hatLine.SetDescription(hatAssets[i].HatDescription);
			hatLine.gameObject.SetActive(true);

			hatLine.SetCanAffordCost(GameState.GameData.PlayerGold >= hatAssets[i].GoldCost && GameState.GameData.PlayerScrap >= hatAssets[i].ScrapCost);

			hatLine.HatPurchaseRequested += HatLine_HatPurchaseRequested;
			hatLine.HatEquipRequested += HatLine_HatEquipRequested; 

			hatLines.Add(hatLine); 
		}
	}

	public void UpdateCanAffordHats(int hatId, bool canAffordHat) {

		foreach (HatLine hatLine in hatLines) {

			if (hatLine.HatId == hatId) {

				hatLine.SetCanAffordCost(canAffordHat); 
			}
		}
	}

	public void UnlockHats(int[] hatIds) {

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

		switch (shopState) {
			case ShopStates.Selection:

				SetInteractable(true);
				selectionSubView.SetInteractable(true);
				break;
		}
	}
}