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

		sequence = new GameSequence(this); 

		startingX = rect.anchoredPosition.x;

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

	//-----------------------------------------------------------------------------------------
	// Public Methods:
	//-----------------------------------------------------------------------------------------

	public void SlideOffLeft() {

		sequence.Tween(LeanTween.value(gameObject, SetRectPositionX, startingX, startingX - rect.rect.width, slideOffLeftDuration)
			.setEase(slideOffLeftLeanType)
			.setOnComplete(() => { TransitionCompleted?.Invoke(); }));
	}

	public void SlideOnLeft() {

		sequence.Tween(LeanTween.value(gameObject, SetRectPositionX, startingX - rect.rect.width, startingX, slideOnLeftDuration)
			.setEase(slideOnLeftLeanType)
			.setOnComplete(() => { TransitionCompleted?.Invoke(); }));
	}

	public void SlideOffRight() {

		sequence.Tween(LeanTween.value(gameObject, SetRectPositionX, startingX, startingX + rect.rect.width, slideOffRightDuration)
			.setEase(slideOffRightLeanType)
			.setOnComplete(() => { TransitionCompleted?.Invoke(); }));
	}

	public void SlideOnRight() {

		sequence.Tween(LeanTween.value(gameObject, SetRectPositionX, startingX + rect.rect.width, startingX, slideOnRightDuration)
			.setEase(slideOnRightLeanType)
			.setOnComplete(() => { TransitionCompleted?.Invoke(); }));
	}

	public void ShowHideView(bool shouldShow) {
		SetAlpha(shouldShow ? 1.0f : 0.0f);
	}

	//-----------------------------------------------------------------------------------------
	// Private Methods:
	//-----------------------------------------------------------------------------------------

	private void SetRectPositionX(float x) {
		rect.anchoredPosition = rect.anchoredPosition.WithX(x); 
	}

	private void SetAlpha(float t) {
		canvasGroup.alpha = t; 
	}
}