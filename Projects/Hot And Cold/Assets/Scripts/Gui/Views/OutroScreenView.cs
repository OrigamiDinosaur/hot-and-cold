using System;
using TMPro;
using UnityEngine;

public class OutroScreenView : GuiSlidingView {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action ValuesPresented;

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected TextMeshProUGUI goldValueText;
	[SerializeField] protected TextMeshProUGUI scrapValueText;

	[SerializeField] protected AudioClip coinBlipSfx;

	[Header("Progress")]

	[SerializeField] protected float progressRate;
	[SerializeField] protected int progressAmount;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private int totalGold;
	private int totalScrap;

	private int goldProgressValue;
	private int scrapProgressValue;

	private bool shouldProgressValues;

	private float nextUpdateTime; 

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Update() {
		if (!shouldProgressValues) return;

		// check whether we should update again. 
		if (Time.time > nextUpdateTime) {

			// set our next update time. 
			nextUpdateTime = Time.time + progressRate;

			// play our blip sound. 
			AudioSource.PlayClipAtPoint(coinBlipSfx, Vector3.zero);

			// update our gold value if needed.
			if (goldProgressValue < totalGold) {

				goldProgressValue += progressAmount;
				goldProgressValue = Mathf.Clamp(goldProgressValue, 0, totalGold);

				goldValueText.text = goldProgressValue.ToString();
			}

			// update our scrap value if needed.
			if (scrapProgressValue < totalScrap) {

				scrapProgressValue += progressAmount;
				scrapProgressValue = Mathf.Clamp(scrapProgressValue, 0, totalScrap);

				scrapValueText.text = scrapProgressValue.ToString();
			}

			// if both our values have reached the totals, flag that we're done!
			if (goldProgressValue == totalGold && scrapProgressValue == totalScrap) {

				shouldProgressValues = false; 
				ValuesPresented?.Invoke();
			}
		}
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void ResetProgres() {
		goldValueText.text = "0"; 
		scrapValueText.text = "0";
	}

	public void StartProgress(int gold, int scrap) {

		// set our totals.
		totalGold = gold;
		totalScrap = scrap;

		// reset our progress.
		goldProgressValue = 0;
		scrapProgressValue = 0;

		// set our first update time and flag we can start. 
		nextUpdateTime = Time.time + progressRate;
		shouldProgressValues = true; 
	}
}