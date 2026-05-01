using TMPro;
using UnityEngine;

public class ShopMenuView : GuiSlidingView {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected TextMeshProUGUI goldValueText;
	[SerializeField] protected TextMeshProUGUI scrapValueText;

	[SerializeField] protected OriButton[] buttons; 

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SetCurrencyValues(int goldValue, int scrapValue) {

		goldValueText.text = goldValue.ToString();
		scrapValueText.text = scrapValue.ToString();
	}

	public void SetButtonsEnabled(bool isEnabled) {

		foreach (OriButton button in buttons) {
			button.enabled = isEnabled;
		}
	}
}