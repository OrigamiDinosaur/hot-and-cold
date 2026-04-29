using Apache.Core;
using TMPro;
using UnityEngine;

public class TimeView : ApacheComponent {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected TextMeshProUGUI timeRemainingText;

	[SerializeField] protected CanvasGroup canvasGroup;

	[SerializeField] protected float presentDuration;
	[SerializeField] protected float dismissDuration; 

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void PresentView() {

		sequence.Tween(LeanTween.value(gameObject, SetAlpha, 0.0f, 1.0f, presentDuration)
			.setEase(LeanTweenType.easeInQuad));
	}

	public void DismissView() {

		sequence.Tween(LeanTween.value(gameObject, SetAlpha, 1.0f, 0.0f, dismissDuration)
			.setEase(LeanTweenType.easeInQuad));
	}

	public void SetRemainingTime(float remainingTime) {

		timeRemainingText.text = remainingTime.ToString("F2"); 
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	public void SetAlpha(float t) {
		canvasGroup.alpha = t;
	}
}