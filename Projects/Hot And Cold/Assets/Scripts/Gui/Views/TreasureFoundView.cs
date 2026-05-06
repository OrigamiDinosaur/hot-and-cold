using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreasureFoundView : MonoBehaviour {

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
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private GameSequence sequence;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Start() {

		sequence = new GameSequence(this);
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void ShowTreasureFoundDialogue(TreasureAsset treasureAsset) {

		// update our treasure text. 
		treasureNameText.text = treasureAsset.ItemName;
		treasureValueText.text = string.Format(VALUE_TEXT, treasureAsset.ItemValue.value);

		// get our currency index and use it to set our treasure icon. 
		int currencyIndex = (int)treasureAsset.ItemValue.currency;
		treasureIconImage.sprite = treasureIcons[currencyIndex];

		// get our rarity index and use it to set our treasure color. 
		int rarityIndex = (int)treasureAsset.ItemRarity;
		treasureNameText.color = rarityColors[rarityIndex];

		// present our view. 
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
}