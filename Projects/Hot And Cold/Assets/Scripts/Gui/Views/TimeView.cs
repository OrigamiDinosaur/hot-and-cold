using TMPro;
using UnityEngine;

public class TimeView : GuiView {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected TextMeshProUGUI timeRemainingText;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------
	
	public void SetRemainingTime(float remainingTime) {

		timeRemainingText.text = remainingTime.ToString("F2"); 
	}
}