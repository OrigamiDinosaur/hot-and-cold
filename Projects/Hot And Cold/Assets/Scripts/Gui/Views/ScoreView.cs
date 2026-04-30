using TMPro;
using UnityEngine;

public class ScoreView : GuiView {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected TextMeshProUGUI goldValueText;
	[SerializeField] protected TextMeshProUGUI scrapValueText;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void ResetValues() {
		goldValueText.text = "0";
		scrapValueText.text = "0";
	}

	public void SetGoldValue(int totalGold) {
		goldValueText.text = totalGold.ToString();
	}

	public void SetScrapValue(int totalScrap) {
		scrapValueText.text = totalScrap.ToString();
	}
}