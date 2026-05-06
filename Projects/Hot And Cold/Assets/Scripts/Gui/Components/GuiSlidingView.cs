using System;
using UnityEngine;

public class GuiSlidingView : MonoBehaviour {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action TransitionCompleted;

	//-----------------------------------------------------------------------------------------
	// Type Definitions:
	//-----------------------------------------------------------------------------------------

	protected enum States {
		Left,
		Centre,
		Right
	}

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[Header("References")]

	[SerializeField] protected RectTransform rect; 
	[SerializeField] protected CanvasGroup canvasGroup;

	[SerializeField] protected TextButton[] textButtons;

	[Header("States")]

	[SerializeField] protected States startingState = States.Centre;

	[Header("Transition")]

	[SerializeField] protected float slideOffLeftDuration = 0.5f;
	[SerializeField] protected LeanTweenType slideOffLeftLeanType = LeanTweenType.linear;

	[SerializeField] protected float slideOnLeftDuration = 0.5f;
	[SerializeField] protected LeanTweenType slideOnLeftLeanType = LeanTweenType.linear;

	[SerializeField] protected float slideOffRightDuration = 0.5f;
	[SerializeField] protected LeanTweenType slideOffRightLeanType = LeanTweenType.linear;

	[SerializeField] protected float slideOnRightDuration = 0.5f;
	[SerializeField] protected LeanTweenType slideOnRightLeanType = LeanTweenType.linear;

	//-----------------------------------------------------------------------------------------
	// Private Fields:
	//-----------------------------------------------------------------------------------------

	private GameSequence sequence;
	private float startingX;

	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	protected void Start() {

		// init our sequence.
		sequence = new GameSequence(this); 

		// cache our starting x for slide transitions. 
		startingX = rect.anchoredPosition.x;

		ConfigureForStartingState();
	}

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SlideOffLeft() {

		// slide from our starting position to our left position, flagging its completion on complete.
		sequence.Tween(LeanTween.value(gameObject, SetRectPositionX, startingX, startingX - rect.rect.width, slideOffLeftDuration)
			.setEase(slideOffLeftLeanType)
			.setOnComplete(() => {
				OnTransitionCompleted();
				TransitionCompleted?.Invoke();
			}));
	}

	public void SlideOnLeft() {

		// slide from our left position to our starting position, flagging its completion on complete.
		sequence.Tween(LeanTween.value(gameObject, SetRectPositionX, startingX - rect.rect.width, startingX, slideOnLeftDuration)
			.setEase(slideOnLeftLeanType)
			.setOnComplete(() => {
				OnTransitionCompleted();
				TransitionCompleted?.Invoke();
			}));
	}

	public void SlideOffRight() {
		
		// slide from our starting position to our right position, flagging its completion on complete.
		sequence.Tween(LeanTween.value(gameObject, SetRectPositionX, startingX, startingX + rect.rect.width, slideOffRightDuration)
			.setEase(slideOffRightLeanType)
			.setOnComplete(() => {
				OnTransitionCompleted();
				TransitionCompleted?.Invoke();
			}));
	}

	public void SlideOnRight() {

		// slide from our right position to our starting position, flagging its completion on complete.
		sequence.Tween(LeanTween.value(gameObject, SetRectPositionX, startingX + rect.rect.width, startingX, slideOnRightDuration)
			.setEase(slideOnRightLeanType)
			.setOnComplete(() => {
				OnTransitionCompleted();
				TransitionCompleted?.Invoke();
			}));
	}

	public void ShowHideView(bool shouldShow) {
		SetAlpha(shouldShow ? 1.0f : 0.0f);
	}

	public void SetInteractable(bool isInteractable) {
		canvasGroup.blocksRaycasts = isInteractable;
		canvasGroup.interactable = isInteractable;

		if (!isInteractable) {
			foreach (TextButton button in textButtons) {
				button.ResetButton();
			}
		}
	}

	public void ResetState() {
		ConfigureForStartingState();
	}

	//-----------------------------------------------------------------------------------------
	// Protected Methods:
	//-----------------------------------------------------------------------------------------

	protected virtual void OnTransitionCompleted() { }

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void SetRectPositionX(float x) {
		rect.anchoredPosition = rect.anchoredPosition.WithX(x); 
	}

	private void SetAlpha(float t) {
		canvasGroup.alpha = t; 
	}

	private void ConfigureForStartingState() {

		// set our position based on our starting state. 
		switch (startingState) {
			case States.Left:
				rect.anchoredPosition = rect.anchoredPosition.WithX(startingX - rect.rect.width);
				break;
			case States.Centre:
				rect.anchoredPosition = rect.anchoredPosition.WithX(startingX);
				break;
			case States.Right:
				rect.anchoredPosition = rect.anchoredPosition.WithX(startingX + rect.rect.width);
				break;
		}
	}
}