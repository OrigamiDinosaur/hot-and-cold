using Apache.Core;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TreasureFoundView : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Constants:
	//-----------------------------------------------------------------------------------------

	private const string VALUE_TEXT = "+{0}";

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected TextMeshProUGUI treasureNameText;
	[SerializeField] protected Image treasureIconImage;
	[SerializeField] protected TextMeshProUGUI treasureValueText;

	[SerializeField] protected CanvasGroup canvasGroup;

	[SerializeField] protected Sprite[] treasureIcons;
	[SerializeField] protected Color[] rarityColors;

	[Header("Display")]

	[SerializeField] protected float presentDuration;
	[SerializeField] protected float dismissDelay;
	[SerializeField] protected float dismissDuration; 

	[Header("Dev")]

	[SerializeField] protected TreasureAsset asset;

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	void ShowTreasureFoundDialogue(TreasureAsset treasureAsset) {

		treasureNameText.text = treasureAsset.ItemName;
		treasureValueText.text = string.Format(VALUE_TEXT, treasureAsset.ItemValue.value);

		int currencyIndex = (int)treasureAsset.ItemValue.currency;
		treasureIconImage.sprite = treasureIcons[currencyIndex];

		int rarityIndex = (int)treasureAsset.ItemRarity;
		treasureNameText.color = rarityColors[rarityIndex];

		sequence.Tween(LeanTween.value(gameObject, SetAlpha, 0.0f, 1.0f, presentDuration)
			.setEase(LeanTweenType.easeInQuad)
			.setOnComplete(DismissAfterDelay));
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void DismissAfterDelay() {

		sequence.Do(dismissDelay, () => {
			sequence.Tween(LeanTween.value(gameObject, SetAlpha, 1.0f, 0.0f, dismissDuration)
				.setEase(LeanTweenType.easeInQuad));
		});
	}

	private void SetAlpha(float t) {
		canvasGroup.alpha = t;
	}

	//-----------------------------------------------------------------------------------------
	// Editor Methods:
	//-----------------------------------------------------------------------------------------

	[ApacheButton]
	public void ShowDialogue() {

		ShowTreasureFoundDialogue(asset);
	}
}