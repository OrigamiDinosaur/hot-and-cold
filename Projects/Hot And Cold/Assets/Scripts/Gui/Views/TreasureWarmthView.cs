using TMPro;
using UnityEngine;

public class TreasureWarmthView : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected TextMeshProUGUI treasureWarmthText;

	[SerializeField] protected CanvasGroup canvasGroup; 

	[Header("Display")]

	[SerializeField] protected float presentDuration;
	[SerializeField] protected float dismissDelay;
	[SerializeField] protected float dismissDuration; 

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

	public void ShowTreasureWarmthDialogue(string warmthDescription) {

		sequence.Cancel();

		canvasGroup.alpha = 0.0f; 

		treasureWarmthText.text = warmthDescription;
		
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