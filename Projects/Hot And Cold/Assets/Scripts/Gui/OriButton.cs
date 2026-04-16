using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OriButton : Button {

	//-----------------------------------------------------------------------------------------
	// Events:
	//-----------------------------------------------------------------------------------------

	public event Action PointerEntered;
	public event Action PointerExited;
	public event Action PointerDown;
	public event Action PointerUp;
	public event Action PointerClicked;

	//-----------------------------------------------------------------------------------------
	// Inspector Variables:
	//-----------------------------------------------------------------------------------------

	[SerializeField] protected AudioClip hoveredClip;
	[SerializeField] protected AudioClip unhoveredClip;
	[SerializeField] protected AudioClip pointerDownClip;
	[SerializeField] protected AudioClip pointerUpClip; 
	
	//-----------------------------------------------------------------------------------------
	// Protected Methods:
	//-----------------------------------------------------------------------------------------

	public override void OnPointerEnter(PointerEventData pointerEventData) {
		base.OnPointerEnter(pointerEventData);

		AudioHandler.PlayButtonHoveredSfx();
		PointerEntered?.Invoke();
	}

	public override void OnPointerExit(PointerEventData pointerEventData) {
		base.OnPointerExit(pointerEventData);

		AudioHandler.PlayButtonUnhoveredSfx();
		PointerExited?.Invoke();
	}

	public override void OnPointerDown(PointerEventData pointerEventData) {
		base.OnPointerDown(pointerEventData);

		AudioHandler.PlayButtonDownSfx();
		PointerDown?.Invoke();
	}

	public override void OnPointerUp(PointerEventData pointerEventData) {
		base.OnPointerUp(pointerEventData);

		AudioHandler.PlayButtonUpSfx();
		PointerUp?.Invoke();
	}

	public override void OnPointerClick(PointerEventData eventData) {
		base.OnPointerClick(eventData);
		PointerClicked?.Invoke();
	}

}