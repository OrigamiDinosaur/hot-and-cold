using System;
using TMPro;
using UnityEngine;

public class UpgradeLine : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action UpgradeRequested;

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected TextMeshProUGUI upgradeLevelText;

	[SerializeField] protected OriButton upgradeButton;

	[SerializeField] protected GameObject costTab;

	[SerializeField] protected TextMeshProUGUI goldCostText;
	[SerializeField] protected TextMeshProUGUI scrapCostText;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void OnEnable() {
		upgradeButton.PointerEntered += UpgradeButton_PointerEntered;
		upgradeButton.PointerExited += UpgradeButton_PointerExited; 
	}

	protected void OnDisable() {
		upgradeButton.PointerEntered -= UpgradeButton_PointerEntered;
		upgradeButton.PointerExited -= UpgradeButton_PointerExited;
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers - Inpsector:
	//-----------------------------------------------------------------------------------------

	public void UpgradeButton_Clicked() {
		UpgradeRequested?.Invoke();
	}

	//-----------------------------------------------------------------------------------------
	// Event Handlers:
	//-----------------------------------------------------------------------------------------

	private void UpgradeButton_PointerEntered() {
		costTab.SetActive(true);
	}

	private void UpgradeButton_PointerExited() {
		costTab.SetActive(false); 
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SetUpgradeLevel(int level) {
		upgradeLevelText.text = level.ToString();
	}

	public void SetUpgradeCost(int goldCost, int scrapCost) {

		goldCostText.text = goldCost.ToString();
		scrapCostText.text = scrapCost.ToString();
	}

	public void SetCanAffordCost(bool canAffordCost) {
		upgradeButton.SetInteractable(canAffordCost);
	}
}