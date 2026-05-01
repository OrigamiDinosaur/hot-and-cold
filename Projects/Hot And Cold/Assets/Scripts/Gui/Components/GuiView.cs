using UnityEngine;

public class GuiView : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected CanvasGroup canvasGroup;

	[Header("Transitions")]
	
	[SerializeField] protected float presentDuration;
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

	public void PresentView() {

		sequence.Tween(LeanTween.value(gameObject, SetAlpha, 0.0f, 1.0f, presentDuration)
			.setEase(LeanTweenType.easeInQuad));
	}

	public void DismissView() {

		sequence.Tween(LeanTween.value(gameObject, SetAlpha, 1.0f, 0.0f, dismissDuration)
			.setEase(LeanTweenType.easeInQuad));
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	public void SetAlpha(float t) {
		canvasGroup.alpha = t;
	}
}