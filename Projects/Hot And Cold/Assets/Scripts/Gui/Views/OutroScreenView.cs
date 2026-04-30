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

		if (Time.time > nextUpdateTime) {

			nextUpdateTime = Time.time + progressRate;

			AudioSource.PlayClipAtPoint(coinBlipSfx, Vector3.zero);

			if (goldProgressValue < totalGold) {

				goldProgressValue += progressAmount;
				goldProgressValue = Mathf.Clamp(goldProgressValue, 0, totalGold);

				goldValueText.text = goldProgressValue.ToString();
			}

			if (scrapProgressValue < totalScrap) {

				scrapProgressValue += progressAmount;
				scrapProgressValue = Mathf.Clamp(scrapProgressValue, 0, totalScrap);

				scrapValueText.text = scrapProgressValue.ToString();
			}

			if (goldProgressValue == totalGold && scrapProgressValue == totalScrap) {

				shouldProgressValues = false; 
				ValuesPresented?.Invoke();
			}
		}
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void StartProgress(int gold, int scrap) {

		totalGold = gold;
		totalScrap = scrap;

		goldProgressValue = 0;
		scrapProgressValue = 0;

		nextUpdateTime = Time.time + progressRate;
		shouldProgressValues = true; 
	}
}