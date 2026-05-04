using System.Linq;
using UnityEngine;

public class UpgradeHandler : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected PlayerController playerController;

	[Header("Data")]

	[SerializeField] protected UpgradesAsset drillUpgrades;
	[SerializeField] protected UpgradesAsset engineUpgrades;
	[SerializeField] protected HatAsset[] hatsAssets;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private PlayerAttributes permanentAttributes;
	private PlayerAttributes temporaryAttributes;

	protected bool hasUnsubbedGameGuiStaticEvents; 

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {

		GameGuiController.ShopMenuView.DrillUpgradeRequested += ShopMenuView_DrillUpgradeRequested;
		GameGuiController.ShopMenuView.EngineUpgradeRequested += ShopMenuView_EngineUpgradeRequested;

		GameGuiController.ShopMenuView.HatPurchaseRequested += ShopMenuView_HatPurchaseRequested;
		GameGuiController.ShopMenuView.HatEquipRequested += ShopMenuView_HatEquipRequested;

		GameGuiController.SingletonDestroyed += GameGuiController_SingletonDestroyed;
	}

	protected void OnDisable() {
		UnsubGameGuiStaticEvents();
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void ShopMenuView_DrillUpgradeRequested() {

		// get our current drill level. 
		int currentDrillLevel = GameState.GameData.DrillLevel;

		// get our upgrade costs. 
		int goldCost = drillUpgrades.UpgradeGoldCosts[currentDrillLevel + 1];
		int scrapCost = drillUpgrades.UpgradeScrapCosts[currentDrillLevel + 1]; 

		// may as well check we can afford it and that we can increase our drill level.
		if (currentDrillLevel >= drillUpgrades.UpgradeValues.Length || (GameState.GameData.PlayerGold < goldCost) || (GameState.GameData.PlayerScrap < scrapCost)) return;

		// deduct our costs from our currencies and update our gui.
		GameState.RemoveGold(goldCost);
		GameState.RemoveScrap(scrapCost); 

		GameGuiController.ShopMenuView.SetCurrencyValues(GameState.GameData.PlayerGold, GameState.GameData.PlayerScrap);
		
		// increment our drill level
		GameState.SetDrillLevel(currentDrillLevel + 1); 

		UpdateGuiAndSaveData();
	}

	private void ShopMenuView_EngineUpgradeRequested() {

		// get our current engine level. 
		int currentEngineLevel = GameState.GameData.EngineLevel;

		// get our upgrade costs. 
		int goldCost = engineUpgrades.UpgradeGoldCosts[currentEngineLevel + 1];
		int scrapCost = engineUpgrades.UpgradeScrapCosts[currentEngineLevel + 1]; 

		// may as well check we can afford it and that we can increase our engine level.
		if (currentEngineLevel >= engineUpgrades.UpgradeValues.Length || (GameState.GameData.PlayerGold < goldCost) || (GameState.GameData.PlayerScrap < scrapCost)) return;

		// deduct our costs from our currencies and update our gui.
		GameState.RemoveGold(goldCost);
		GameState.RemoveScrap(scrapCost); 

		GameGuiController.ShopMenuView.SetCurrencyValues(GameState.GameData.PlayerGold, GameState.GameData.PlayerScrap);
		
		// increment our engine level
		GameState.SetEngineLevel(currentEngineLevel + 1); 

		UpdateGuiAndSaveData();
	}

	private void ShopMenuView_HatPurchaseRequested(int hatId) {
		if (GameState.GameData.UnlockedHatIds.Contains(hatId)) return;

		// find our hat. 
		foreach (HatAsset hatAsset in hatsAssets) {
			if (hatAsset.HatId == hatId) {

				// get our costs. 
				int goldCost = hatAsset.GoldCost;
				int scrapCost = hatAsset.ScrapCost;

				// check we can afford it.
				if (GameState.GameData.PlayerGold < goldCost || GameState.GameData.PlayerScrap < scrapCost) return;

				// deduct our costs and update our gui. 
				GameState.RemoveGold(goldCost);
				GameState.RemoveScrap(scrapCost);

				GameGuiController.ShopMenuView.SetCurrencyValues(GameState.GameData.PlayerGold, GameState.GameData.PlayerScrap);

				GameState.AddUnlockedHat(hatId);
				GameState.SetUnlockedHat(hatId); 

				UpdateGuiAndSaveData();
			}
		}
	}

	private void ShopMenuView_HatEquipRequested(int hatId) {
		if (!GameState.GameData.UnlockedHatIds.Contains(hatId)) return;

		GameState.SetUnlockedHat(hatId);

		UpdateGuiAndSaveData();
	}

	private void GameGuiController_SingletonDestroyed() {
		UnsubGameGuiStaticEvents();
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void Init() {

		// assign default attribute values. 
		permanentAttributes = new PlayerAttributes();
		temporaryAttributes = new PlayerAttributes();

		// set our attributes 
		UpdateDrillAttributes();
		UpdateEngineAttributes();

		GameGuiController.ShopMenuView.CreateHatLines(hatsAssets);

		if (GameState.GameData.UnlockedHatIds.Length > 0) {
			GameGuiController.ShopMenuView.UnlockHats(GameState.GameData.UnlockedHatIds);
			SetEquippedHat();
		}
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void UpdateDrillAttributes() {

		// get our current drill level.
		int currentDrillLevel = GameState.GameData.DrillLevel;
		
		// assign our drill value based on our level.
		permanentAttributes.DrillValue = drillUpgrades.UpgradeValues[currentDrillLevel];
		
		// set our default states (assumes max level)
		int goldCost = 0;
		int scrapCost = 0;
		bool canAffordCost = false;

		if (currentDrillLevel < drillUpgrades.UpgradeValues.Length - 1) {

			// get our upgrade costs for the next level. 
			goldCost = drillUpgrades.UpgradeGoldCosts[currentDrillLevel + 1];
			scrapCost = drillUpgrades.UpgradeScrapCosts[currentDrillLevel + 1];

			canAffordCost = ((GameState.GameData.PlayerGold >= goldCost) && (GameState.GameData.PlayerScrap >= scrapCost));
		}

		// update our shop (including adjusting our engine level from 0 index to 1 index)
		GameGuiController.ShopMenuView.SetDrillStats(currentDrillLevel + 1, goldCost, scrapCost, canAffordCost);

		playerController.SetCurrentAttributes(permanentAttributes + temporaryAttributes); 
	}

	private void UpdateEngineAttributes() {

		// get our current drill level.
		int currentEngineLevel = GameState.GameData.EngineLevel;
		
		// assign our drill value based on our level.
		permanentAttributes.MoveSpeed = engineUpgrades.UpgradeValues[currentEngineLevel];
		
		// set our default states (assumes max level)
		int goldCost = 0;
		int scrapCost = 0;
		bool canAffordCost = false;

		if (currentEngineLevel < engineUpgrades.UpgradeValues.Length - 1) {

			// get our upgrade costs for the next level. 
			goldCost = engineUpgrades.UpgradeGoldCosts[currentEngineLevel + 1];
			scrapCost = engineUpgrades.UpgradeScrapCosts[currentEngineLevel + 1];

			canAffordCost = ((GameState.GameData.PlayerGold >= goldCost) && (GameState.GameData.PlayerScrap >= scrapCost));
		}

		// update our shop (including adjusting our engine level from 0 index to 1 index)
		GameGuiController.ShopMenuView.SetEngineStats(currentEngineLevel + 1, goldCost, scrapCost, canAffordCost);

		playerController.SetCurrentAttributes(permanentAttributes + temporaryAttributes); 
	}

	private void SetEquippedHat() {
		if (GameState.GameData.CurrentlyEquippedHat == -1) return; 

		// update our gui. 
		GameGuiController.ShopMenuView.SetEquippedHat(GameState.GameData.CurrentlyEquippedHat);

		// get our hat asset.
		foreach (HatAsset hatAsset in hatsAssets) {
			if (hatAsset.HatId == GameState.GameData.CurrentlyEquippedHat) {

				// update our visuals. 
				playerController.SetEquippedHat(hatAsset.HatAssetReference);

				// update our attributes. 
				temporaryAttributes = hatAsset.AttributeBonus;

				// back out once we've found our hat. 
				break; 
			}
		}

		playerController.SetCurrentAttributes(permanentAttributes + temporaryAttributes); 
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void UpdateGuiAndSaveData() {

		// finalise our upgrades.
		// do both as we need to update costs of all upgrades
		UpdateDrillAttributes();
		UpdateEngineAttributes();

		GameGuiController.ShopMenuView.UnlockHats(GameState.GameData.UnlockedHatIds);
		SetEquippedHat();

		foreach (HatAsset hatsAsset in hatsAssets) {
			GameGuiController.ShopMenuView.UpdateCanAffordHats(hatsAsset.HatId, GameState.GameData.PlayerGold >= hatsAsset.GoldCost && GameState.GameData.PlayerScrap >= hatsAsset.ScrapCost);
		}

		GameState.SaveData();
	}

	private void UnsubGameGuiStaticEvents() {
		if (hasUnsubbedGameGuiStaticEvents) return;

		GameGuiController.SingletonDestroyed -= GameGuiController_SingletonDestroyed;

		GameGuiController.ShopMenuView.DrillUpgradeRequested -= ShopMenuView_DrillUpgradeRequested;
		GameGuiController.ShopMenuView.EngineUpgradeRequested -= ShopMenuView_EngineUpgradeRequested;

		GameGuiController.ShopMenuView.HatPurchaseRequested -= ShopMenuView_HatPurchaseRequested;
		GameGuiController.ShopMenuView.HatEquipRequested -= ShopMenuView_HatEquipRequested;

		hasUnsubbedGameGuiStaticEvents = true; 
	}
}